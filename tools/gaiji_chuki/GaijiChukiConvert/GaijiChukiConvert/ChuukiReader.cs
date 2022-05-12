using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace GaijiChukiConvert;

public static class ChuukiReader
{
    public async static Task<Schemas.dictionary> LoadDictionary(TextReader reader)
    {
        Schemas.entry? current = null;
        List<Schemas.entry> entries = new();
        Schemas.page? page = null;
        List<Schemas.page> pages = new();

        int pageCnt = 1;

        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (line == null) break;

            while (line.Contains('［') && !line.Contains('］'))
            {
                line = Regex.Replace(line, @"[\s　]+$", "");
                line += await reader.ReadLineAsync();
            }
            while (line.Contains('【') && !line.Contains('】'))
            {
                line = Regex.Replace(line, @"[\s　]+$", "");
                line += await reader.ReadLineAsync();
            }

            if (line.Contains('\f')) pageCnt++;

            {
                var match = Regex.Match(line, @"^(.+)【その他】に戻る[\s　]*$");
                if (match.Success)
                {
                    if (page is not null)
                    {
                        if (current is not null) entries.Add(current);
                        page.entries = entries.ToArray();
                        pages.Add(page);
                    }
                    break;
                }
            }

            {
                var match = Regex.Match(line, @"^(\d+)．");
                if (match.Success)
                {
                    if (current is not null)
                    {
                        entries.Add(current);
                    }
                    current = new Schemas.entry() { docPage = pageCnt.ToString() };
                    current.strokes = match.Groups[1].Value;
                }
            }

            {
                var match = Regex.Match(line, @"^(.+)【(.+)】[\s　]*部首・読み索引に戻る[\s　]*部首・画数索引に戻る[\s　]*$");
                if (match.Success)
                {
                    string r = match.Groups[1].Value;
                    r = new Regex(@"[\s　]").Replace(r, "");
                    string c = match.Groups[2].Value;
                    if (page is not null)
                    {
                        if (current is not null) entries.Add(current);
                        page.entries = entries.ToArray();
                        entries = new List<Schemas.entry>();
                        pages.Add(page);
                        current = null;
                    }
                    page = new Schemas.page();
                    page.radical = new Schemas.pageRadical()
                    {
                        readings = new Schemas.pageRadicalReadings() { reading = r.Split('・') },
                        characters = new Schemas.pageRadicalCharacters() { character = EnumerateCharacters(c) }
                    };
                    continue;
                }
            }

            if (current is null) continue;

            {
                var match = Regex.Match(line, @"([^\d+．]+)※［＃([^］]+)］");
                if (match.Success)
                {
                    {
                        var chars = match.Groups[1].Value;
                        chars = Regex.Replace(chars, @"[\s　]", "");
                        current.characters = new Schemas.entryCharacters()
                        {
                            character = EnumerateCharacters(chars),
                        };
                    }

                    {
                        var texts = match.Groups[2].Value.Split('、');

                        var note = new Schemas.note()
                        {
                            full = match.Groups[2].Value,
                            description = texts[0],
                        };
                        if (texts.Length >= 2) note.Item = ConvertCharCode(texts[1]);
                        current.note = note;
                    }
                }
            }

            {
                if (line.Contains('★')) current.duplicate = true;
                if (line.Contains("補助のみ")) current.supplement = Schemas.entrySupplement.supplementOnly;
                if (line.Contains("補助漢字と共通")) current.supplement = Schemas.entrySupplement.supplementCommon;
            }

        }

        return new Schemas.dictionary()
        {
            kanji = new Schemas.dictionaryKanji()
            {
                page = pages.ToArray(),
            }
        };
    }

    public static void WriteDictionary(string path, Schemas.dictionary dictionary)
    {
        using var writer = new StreamWriter(path, false);
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(Schemas.dictionary));
        xs.Serialize(writer, dictionary);
        writer.Close();
    }

    public static object? ConvertCharCode(string text)
    {
        {
            var match = Regex.Match(text, @"^U\+([\d+a-fA-F]+)");
            if (match.Success)
            {
                //var code = match.Groups[1].Value;//Span使えそう。
                //byte[] codeByte = new byte[code.Length / 2];
                //for (int i = 0; i < code.Length; i += 2)
                //{
                //    codeByte[i] = Convert.ToByte(code.Substring(i, 2), 16);
                //}
                return new Schemas.noteUnicode()
                {
                    code = match.Groups[1].Value,
                };
            }
        }
        {
            var match = Regex.Match(text, @"^第(\d)水準(\d)+\-(\d+)-(\d+)");
            if (match.Success)
            {
                return new Schemas.noteJisx0213()
                {
                    levelSpecified = true,
                    level = int.Parse(match.Groups[1].Value),
                    menSpecified = true,
                    men = int.Parse(match.Groups[2].Value),
                    kuSpecified = true,
                    ku = int.Parse(match.Groups[3].Value),
                    tenSpecified = true,
                    ten = int.Parse(match.Groups[4].Value),
                };
            }
        }
        return null;
    }

    public static string[] EnumerateCharacters(string text)
    {
        //サロゲートペアに配慮して文字を分割。
        //https://qiita.com/koara-local/items/95e07949021a5a87fed8
        var result = new List<string>();
        var charEnum = System.Globalization.StringInfo.GetTextElementEnumerator(text);
        while (charEnum.MoveNext())
        {
            result.Add(charEnum.GetTextElement());
        }
        return result.ToArray();
    }
}
