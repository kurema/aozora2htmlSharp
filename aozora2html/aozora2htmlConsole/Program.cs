using System.CommandLine;
using System.CommandLine.Invocation;

// See https://aka.ms/new-console-template for more information

var optionGaiji = new Option<DirectoryInfo?>("--gaiji-dir", () => null, "setting gaiji directory");
var optionCss = new Option<string[]>("--css-files", () => Array.Empty<string>(), "setting css directory") { Arity = ArgumentArity.ZeroOrMore };
var optionJisx = new Option<bool>("--use-jisx0213", () => false, "use JIS X 0213 character") { Arity = new ArgumentArity(0, 1) };
var optionUnicode = new Option<bool>("--use-unicode", () => false, "use Unicode character") { Arity = new ArgumentArity(0, 1) };
var argumentIn = new Argument<string>("text file", "input file.");
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

rootCommand.SetHandler((DirectoryInfo? gaijiDir, string[] cssFiles, bool useJisx0213, bool useUnicode, string textFile, FileInfo? htmlFile) =>
{
    Console.WriteLine($"jisx:{useJisx0213}\nunicode:{useUnicode}");
}, optionGaiji, optionCss, optionJisx, optionUnicode, argumentIn, argumentOut);

//await rootCommand.InvokeAsync(args);
await rootCommand.InvokeAsync("test --css-files aaa --use-unicode");
