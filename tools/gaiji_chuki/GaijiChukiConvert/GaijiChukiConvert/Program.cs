using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information

#if DEBUG
args = new[] { "gaiji_chuki.txt" , "out.xml" };
#endif

var gaiji = await GaijiChukiConvert.ChuukiReader.LoadDictionary(new StreamReader(args[0]));
GaijiChukiConvert.ChuukiReader.WriteDictionary(args[1], gaiji);

