using System.CommandLine;
// See https://aka.ms/new-console-template for more information

var rootCommand = Aozora.Console.Functions.GetCommand();
#if DEBUG
//kurema:「吾輩は猫である」をデバッグ用に使う場合。bin/以下のフォルダに該当ファイルを配置してください。
//await rootCommand.InvokeAsync("789_ruby_5639.zip out.html");
await rootCommand.InvokeAsync(args);
#else
await rootCommand.InvokeAsync(args);
#endif