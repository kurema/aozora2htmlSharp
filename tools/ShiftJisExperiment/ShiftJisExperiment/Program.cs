using System;
using System.Text;
using System.Globalization;

namespace ShiftJisExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.globalization.charunicodeinfo?view=net-5.0

            EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            var encoding = provider.GetEncoding("shift-jis", new EncoderReplacementFallback("〓"), new DecoderReplacementFallback("〓"));

            byte[] bytes = new byte[2] { 0, 0 };
            while (true)
            {
                var ch = bytes[0] == 0 ? encoding.GetString(new byte[] { bytes[1] }) : encoding.GetString(bytes);
                var hex = Convert.ToHexString(bytes);
                var category = CharUnicodeInfo.GetUnicodeCategory(ch[0]);
                if (ch.Length == 1) Console.WriteLine("{0,-3} {1,-5} {2}", ch, hex, category);
                if (bytes[1] == 255)
                {
                    if (bytes[0] == 255) break;
                    bytes[0]++;
                    bytes[1] = 0;
                }
                else
                {
                    bytes[1]++;
                }
            }
        }
    }
}
