using System.CommandLine;
using System.CommandLine.Invocation;

// See https://aka.ms/new-console-template for more information

var rootCommand = new RootCommand("青空文庫をどうのこうのするコマンドです。")
{
    new Option("--gaiji-dir","setting gaiji directory"),
};

await rootCommand.InvokeAsync(args);