using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.CommandLine;

namespace Aozora.Console;

public static class Functions
{
    public static RootCommand GetCommand()
    {
        //kurema:DirectoryInfoを使った場合、C:\で../../gaiji_dirとかの指定ができないのでstringに。suggestionが機能しなくなる？
        var optionGaiji = new Option<string?>("--gaiji-dir", () => null, Resources.Resource.GaijiDir_Desc);
        var optionCss = new Option<string[]>("--css-files", () => Array.Empty<string>(), Resources.Resource.CssFiles_Desc) { Arity = ArgumentArity.ZeroOrMore };
        var optionJisx = new Option<bool>("--use-jisx0213", () => false, Resources.Resource.UseJisx0213_Desc) { Arity = new ArgumentArity(0, 1) };
        var optionUnicode = new Option<bool>("--use-unicode", () => false, Resources.Resource.UseUnicode_Desc) { Arity = new ArgumentArity(0, 1) };
        var argumentIn = new Argument<string>(Resources.Resource.TextFile_Title,/* () => null,*/ Resources.Resource.TextFile_Desc);
        var argumentOut = new Argument<FileInfo?>(Resources.Resource.HtmlFile_Title, () => null, Resources.Resource.HtmlFile_Desc).LegalFilePathsOnly();
        //kurema:--error-utf8はC#環境下で余り意味ないと思ったので削除しました。
        //new Option<bool>("--error-utf8","show error messages in UTF-8, not Shift_JIS"),

        var rootCommand = new RootCommand(Resources.Resource.Command_Desc) { optionGaiji, optionCss, optionJisx, optionUnicode, argumentIn, argumentOut, };

        rootCommand.SetHandler((string? gaijiDir, string[] cssFiles, bool useJisx0213, bool useUnicode, string textFile, FileInfo? htmlFile)
            => Handle(gaijiDir, cssFiles, useJisx0213, useUnicode, textFile, htmlFile)
            , optionGaiji, optionCss, optionJisx, optionUnicode, argumentIn, argumentOut);

        return rootCommand;
    }

    public static async Task Handle(string? gaijiDir, string[] cssFiles, bool useJisx0213, bool useUnicode, string textFile, FileInfo? htmlFile)
    {
        bool strictReturnCode = true;

        Aozora.Jstream jstream;
        string? textFileDirectory = null;
        if (string.IsNullOrWhiteSpace(textFile))
        {
            jstream = new Aozora.Jstream(System.Console.In, strictReturnCode);
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
                        System.Console.Error.WriteLine($"The zip file is empty.");
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
                        var wc = new HttpClient();
                        //kurema:
                        //青空文庫はUser-Agentを普通のブラウザの様に偽装しないとミラーサイト(停止済み)にリダイレクトされるという謎の仕様がある。
                        //https://twitter.com/agtc/status/522892380626628609
                        wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                        response = await wc.GetAsync(textFile);
                    }
                    catch (Exception e)
                    {
                        System.Console.Error.WriteLine($"Download Error: {e.Message}");
                        return;
                    }
                    if (!response.IsSuccessStatusCode)
                    {
                        System.Console.Error.WriteLine($"Download Error: {response.StatusCode}");
                        return;
                    }
                    if (response.Content.Headers.ContentType?.MediaType?.ToUpperInvariant().StartsWith("APPLICATION/ZIP") == true)
                    {
                        var sr = GetFirstEntryZip(await response.Content.ReadAsStreamAsync());
                        if (sr is null)
                        {
                            System.Console.Error.WriteLine($"The zip file is empty.");
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
                    System.Console.Error.WriteLine($"The scheme is not supported: {uri.Scheme}");
                    return;
                }
            }
            else
            {
                System.Console.Error.WriteLine($"file not found: {textFile}");
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
            output = new Aozora.Helpers.OutputStreamWriter(new StreamWriter(htmlFile.Open(FileMode.Create), Aozora.Aozora2Html.ShiftJis));
        }

        string? gaijiDirRelative = null;
        if (gaijiDir is not null)
        {
            if (!Path.IsPathRooted(gaijiDir)) gaijiDirRelative = gaijiDir;
            else if (textFileDirectory is not null) gaijiDirRelative = Path.GetRelativePath(textFileDirectory, gaijiDir);
            else gaijiDirRelative = Path.GetRelativePath(Directory.GetCurrentDirectory(), gaijiDir);
        }

        string[] cssFilesRelative = cssFiles.SelectMany(a => a.Split(',')).Select(a => Path.IsPathRooted(a) && textFileDirectory is not null ? Path.GetRelativePath(textFileDirectory, a) : a).ToArray();

        var aozora = new Aozora.Aozora2Html(jstream, output, new Aozora.Helpers.OutputConsoleError(), gaijiDirRelative, cssFilesRelative.Length == 0 ? null : cssFilesRelative)
        {
            UseJisx0213Accent = useJisx0213,
            UseJisx0214EmbedGaiji = useJisx0213,
            UseUnicodeEmbedGaiji = useUnicode,
        };
        aozora.Process();
    }


    //public static StreamReader? GetFirstEntryZip(Stream stream, bool forceShiftJis = false, Action? notShiftJisCallback = null)
    public static StreamReader? GetFirstEntryZip(Stream stream)
    {
        System.IO.Compression.ZipArchive archive = new(stream, System.IO.Compression.ZipArchiveMode.Read);
        var text = archive.Entries.FirstOrDefault(a => a.Name.ToUpperInvariant().EndsWith(".TXT") && a.CompressedLength > 0);
        if (text is null) return null;
        return new StreamReader(text.Open(), Aozora2Html.ShiftJis);
        //if (forceShiftJis) return new StreamReader(text.Open(), Aozora2Html.ShiftJis);
        //return DetectEncodingAndGetStreamReader(() => text.Open(), notShiftJisCallback);
    }

    ////Install ReadJEnc for this to function.
    //public static StreamReader DetectEncodingAndGetStreamReader(Func<Stream> func, Action? notShiftJisCallback = null)
    //{
    //    int codePage;
    //    {
    //        const int assumptionLength = 4096;
    //        using var streamZip = func();
    //        byte[] buffer = new byte[assumptionLength];
    //        streamZip.Read(buffer, 0, assumptionLength);
    //        var charCode = Hnx8.ReadJEnc.ReadJEnc.JP.GetEncoding(buffer, assumptionLength, out string _);
    //        codePage = charCode?.CodePage ?? 932;
    //    }
    //    if (codePage is not 932) notShiftJisCallback?.Invoke();
    //    return new StreamReader(func(), codePage is 932 ? Aozora2Html.ShiftJis : CodePagesEncodingProvider.Instance.GetEncoding(codePage) ?? Aozora2Html.ShiftJis);
    //}
}
