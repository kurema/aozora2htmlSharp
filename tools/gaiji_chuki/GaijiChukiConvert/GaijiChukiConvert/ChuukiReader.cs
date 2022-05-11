using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;


namespace GaijiChukiConvert;

public class ChuukiReader
{
    StreamReader Reader;


    public ChuukiReader(StreamReader reader)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async void Process()
    {
        bool star = false;
         while (true)
        {
            var line = await Reader.ReadLineAsync();

            if (line == null) break;

            while (line.Contains('［') && line.Contains('］'))
            {
                line += await Reader.ReadLineAsync();
            }

            {
                var match = Regex.Match(line, @"^(.+)【その他】に戻る[\s　]*$");
                if (match.Success)
                {
                    break;
                }
            }

            if (line.Contains('★')) star = true;

            {
                var match = Regex.Match(line, @"^(.+)【(.+)】[\s　]*部首・読み索引に戻る[\s　]*部首・画数索引に戻る[\s　]*$");
                if (match.Success)
                {
                    Console.WriteLine($"{match.Groups[1].Value} : {match.Groups[2].Value}");
                    continue;
                }
            }
        }
    }
}
