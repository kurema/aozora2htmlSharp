using System.CommandLine;
using System.CommandLine.Invocation;

// See https://aka.ms/new-console-template for more information


var rootCommand = Aozora.Console.Functions.GetCommand();

#if DEBUG
await rootCommand.InvokeAsync("https://www.aozora.gr.jp/cards/001030/files/4815_ruby_14375.zip ziptest.html --use-jisx0213 --gaiji-dir ../rarara");
//await rootCommand.InvokeAsync("chukiichiran_kinyurei.txt output.html");
//await rootCommand.InvokeAsync("test.txt test.html");
//await rootCommand.InvokeAsync("test.txt output2.html");

//await rootCommand.InvokeAsync("test --css-files aaa --use-unicode");
//await rootCommand.InvokeAsync("-?");
//await rootCommand.InvokeAsync("");
#else
await rootCommand.InvokeAsync(args);
#endif