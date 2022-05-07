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
    string? textFileDirectory = null;
    if (string.IsNullOrWhiteSpace(textFile))
    {
        jstream = new Aozora.Jstream(Console.In, strictReturnCode);
    }
    else
    {
        if (File.Exists(textFile))
        {
            textFileDirectory = Path.GetDirectoryName(Path.GetFullPath(textFile));

            if (Path.GetExtension(textFile)?.ToUpper() == ".ZIP")
            {
                var sr = Aozora.Console.Functions.GetFirstEntryZip(new FileStream(textFile, FileMode.Open, FileAccess.Read));
                if (sr is null)
                {
                    Console.Error.WriteLine($"The zip file is empty.");
                    return;
                }
                jstream = new Aozora.Jstream(sr, strictReturnCode);
            }
            else
            {
                jstream = new Aozora.Jstream(new StreamReader(textFile, Aozora.Aozora2Html.ShiftJis), strictReturnCode);
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
                    var sr = Aozora.Console.Functions.GetFirstEntryZip(await response.Content.ReadAsStreamAsync());
                    if (sr is null)
                    {
                        Console.Error.WriteLine($"The zip file is empty.");
                        return;
                    }
                    jstream = new Aozora.Jstream(sr, strictReturnCode);
                }
                else
                {
                    jstream = new Aozora.Jstream(new StringReader(await response.Content.ReadAsStringAsync()), strictReturnCode);
                }
            }
            else
            {
                Console.Error.WriteLine($"The scheme is not supported: {uri.Scheme}");
                return;
            }
        }
        else
        {
            Console.Error.WriteLine($"file not found: {textFile}");
            return;
        }
    }

    Aozora.Helpers.IOutput output;
    if (htmlFile is null)
    {
        output = new Aozora.Helpers.OutputConsole();
    }
    else
    {
        output = new Aozora.Helpers.OutputStreamWriter(new StreamWriter(htmlFile.OpenWrite(), Aozora.Aozora2Html.ShiftJis));
    }

    string? gaijiDirRelative = null;
    if (gaijiDir is not null)
    {
        if (textFileDirectory is not null) gaijiDirRelative = Path.GetRelativePath(textFileDirectory, gaijiDir.FullName);
        else gaijiDirRelative = gaijiDir.FullName;
    }

    string[] cssFilesRelative = cssFiles.Select(a => Path.IsPathRooted(a) && textFileDirectory is not null ? Path.GetRelativePath(textFileDirectory, a) : a).ToArray();

    var aozora = new Aozora.Aozora2Html(jstream, output, new Aozora.Helpers.OutputConsoleError(), gaijiDirRelative, cssFilesRelative.Length == 0 ? null : cssFilesRelative)
    {
        use_jisx0213_accent = useJisx0213,
        use_jisx0214_embed_gaiji = useJisx0213,
        use_unicode_embed_gaiji = useUnicode,
    };
    try
    {
        aozora.process();
    }
    catch (Exception e)
    {
        throw;
    }
}, optionGaiji, optionCss, optionJisx, optionUnicode, argumentIn, argumentOut);

//await rootCommand.InvokeAsync("chukiichiran_kinyurei.txt");
await rootCommand.InvokeAsync("chukiichiran_kinyurei.txt output.html");
//await rootCommand.InvokeAsync("test.txt output2.html");
//await rootCommand.InvokeAsync(args);
//await rootCommand.InvokeAsync("test --css-files aaa --use-unicode");
//await rootCommand.InvokeAsync("-?");
//await rootCommand.InvokeAsync("");