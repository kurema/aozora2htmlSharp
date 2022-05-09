using System.Text;
using System.Text.RegularExpressions;
// See https://aka.ms/new-console-template for more information

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
using var sr = new StreamReader(args[0],Encoding.GetEncoding("Shift_JIS"));
var content = sr.ReadToEnd();

var result= new Regex(@"([^\r\n]+[^\r])(\n)|([^\r\n]+)(\r)[^\n]").Matches(content);
foreach(Match item in result)
{
    Console.WriteLine(item.Groups[2].Value.Replace("\r", @"\r").Replace("\n", @"\n") + " found");
    Console.WriteLine($"{item.Groups[1].Value} <- Here");
}
