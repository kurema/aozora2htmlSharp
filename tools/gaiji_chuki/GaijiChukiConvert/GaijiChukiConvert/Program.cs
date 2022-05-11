using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information

args = new[] { "gaiji_chuki.txt" };

var gaiji = new GaijiChukiConvert.ChuukiReader(new StreamReader(args[0]));
gaiji.Process();
