using System.CommandLine;
using System.CommandLine.Invocation;

// See https://aka.ms/new-console-template for more information

var rootCommand = new RootCommand("Usage: aozora2html [options] <text file> [<html file>]")
{
    new Option<DirectoryInfo>("--gaiji-dir","setting gaiji directory"),
    new Option<DirectoryInfo>("--css-files","setting css directory"),
    new Option<bool>("--use-jisx0213","use JIS X 0213 character"),
    new Option<bool>("--use-unicode","use Unicode character"),
    new Option<bool>("--error-utf8","show error messages in UTF-8, not Shift_JIS"),
};

rootCommand.AddArgument(new Argument<FileInfo>("text file", "input file."));
rootCommand.AddArgument(new Argument("html file", "output file.") );

rootCommand.SetHandler(() =>
{

});

await rootCommand.InvokeAsync(args);