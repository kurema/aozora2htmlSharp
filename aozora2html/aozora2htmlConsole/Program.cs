﻿using System.CommandLine;
// See https://aka.ms/new-console-template for more information

var rootCommand = Aozora.Console.Functions.GetCommand();
#if DEBUG
await rootCommand.InvokeAsync(args);
#else
await rootCommand.InvokeAsync(args);
#endif