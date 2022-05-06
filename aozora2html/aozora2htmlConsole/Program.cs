using System.CommandLine;
using System.CommandLine.Invocation;

// See https://aka.ms/new-console-template for more information

var optionGaiji = new Option<DirectoryInfo?>("--gaiji-dir", () => null, "setting gaiji directory");
var optionCss = new Option<string[]>("--css-files", () => Array.Empty<string>(), "setting css directory") { Arity = ArgumentArity.ZeroOrMore };
var optionJisx = new Option<bool>("--use-jisx0213", () => false, "use JIS X 0213 character") { Arity = new ArgumentArity(0, 1) };
var optionUnicode = new Option<bool>("--use-unicode", () => false, "use Unicode character") { Arity = new ArgumentArity(0, 1) };
var argumentIn = new Argument<string>("text file",/* () => null,*/ "input file.");
var argumentOut = new Argument<FileInfo?>("html file", () => null, "output file").LegalFilePathsOnly();
//new Option<bool>("--error-utf8","show error messages in UTF-8, not Shift_JIS"),

var rootCommand = new RootCommand()
{
    optionGaiji,
    optionCss,
    optionJisx,
    optionUnicode,
    argumentIn,
    argumentOut,
};

rootCommand.SetHandler(async (DirectoryInfo? gaijiDir, string[] cssFiles, bool useJisx0213, bool useUnicode, string textFile, FileInfo? htmlFile) =>
{
    bool strictReturnCode = true;

    Aozora.Jstream jstream;
    if (textFile is null)
    {
        jstream = new Aozora.Jstream(Console.In, strictReturnCode);
    }
    else
    {
        if (File.Exists(textFile))
        {
            if (Path.GetExtension(textFile)?.ToUpper() == ".ZIP")
            {
                //System.IO.Compression.ZipArchive archive=new System.IO.Compression.ZipArchive(
            }
            else
            {
                jstream = new Aozora.Jstream(new StreamReader(textFile), strictReturnCode);
            }
        }
        else if (Uri.TryCreate(textFile, new UriCreationOptions(), out Uri? uri))
        {
            if (uri.Scheme.ToUpperInvariant() is "HTTP" or "HTTPS")
            {
                HttpResponseMessage response;
                try
                {
                    var wc = new System.Net.Http.HttpClient();
                    response = await wc.GetAsync(textFile);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Download Error: {e.Message}");
                    return;
                }
                if (!response.IsSuccessStatusCode)
                {
                    Console.Error.WriteLine($"Download Error: {response.StatusCode}");
                    return;
                }
                if (response.Content.Headers.ContentType?.MediaType?.ToUpperInvariant().StartsWith("APPLICATION/ZIP") == true)
                {
                    System.IO.Compression.ZipArchive archive = new(await response.Content.ReadAsStreamAsync(), System.IO.Compression.ZipArchiveMode.Read);
                    var text = archive.Entries.FirstOrDefault(a => a.Name.ToUpperInvariant().EndsWith(".TXT") && a.CompressedLength > 0);//Use first .txt file
                    if (text is null)
                    {
                        Console.Error.WriteLine($"The zip file is empty.");
                        return;
                    }
                    jstream = new Aozora.Jstream(new StreamReader(text.Open(), Aozora.Aozora2Html.ShiftJis), strictReturnCode);//Assume the encoding is shiftJis.
                }
                else
                {
                    jstream = new Aozora.Jstream(new StringReader(await response.Content.ReadAsStringAsync()), strictReturnCode);
                }
            }
            else
            {
                Console.Error.WriteLine($"Scheme not supported.");
                return;
            }
        }
        else
        {
            Console.Error.WriteLine($"file not found: {textFile}");
            return;
        }
    }

    Console.WriteLine($"jisx:{useJisx0213}\nunicode:{useUnicode}");
}, optionGaiji, optionCss, optionJisx, optionUnicode, argumentIn, argumentOut);

//await rootCommand.InvokeAsync(args);
await rootCommand.InvokeAsync("http://www.aozora.com/ --css-files aaa --use-unicode");
await rootCommand.InvokeAsync("-?");
//await rootCommand.InvokeAsync("");