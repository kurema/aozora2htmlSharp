using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information

args = new[] { "gaiji_chuki.txt" };

var gaiji = await GaijiChukiConvert.ChuukiReader.LoadDictionary(new StreamReader(args[0]));
GaijiChukiConvert.ChuukiReader.WriteDictionary("out.xml", gaiji);

