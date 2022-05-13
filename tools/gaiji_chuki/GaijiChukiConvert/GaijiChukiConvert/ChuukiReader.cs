using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace GaijiChukiConvert;

public static class ChuukiReader
{
    private const string NotePattern = @"※［＃([^］]+)］";

    public async static Task<Schemas.dictionary> LoadDictionary(TextReader reader)
    {
        Schemas.entry? current = null;
        List<Schemas.entry> entries = new();
        Schemas.page? page = null;
        List<Schemas.page> pages = new();

        int pageCnt = 1;

        while (true)
        {
            //先頭読み飛ばし
            var line = await reader.ReadLineAsync();

            if (line == null) break;
            if (line.Contains('\f')) pageCnt++;
            if (line.Contains("【十五・十六・十七画】")) break;
        }

        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (line == null) break;

            //while (line.Contains('［') && !line.Contains('］'))
            while (line.Count(a => a == '［') != line.Count(a => a == '］'))
            {
                line = Regex.Replace(line, @"[\s　]+$", "");
                line += await reader.ReadLineAsync();
            }

            //while (line.Contains('【') && !line.Contains('】'))
            while (line.Count(a => a == '【') != line.Count(a => a == '】'))
            {
                line = Regex.Replace(line, @"[\s　]+$", "");
                line += await reader.ReadLineAsync();
            }
            if (line.Contains("包摂適用"))
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
                    current = new Schemas.entry() { docPage = pageCnt.ToString() ,characters=new Schemas.entryCharacters()};
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
                var match = Regex.Match(line, @"([^］．]+)→[\s\t　]*［");
                if (match.Success)
                {
                    var text = Regex.Replace(match.Groups[1].Value, @"[\s　]", "");
                    if (text.Length > 0)
                    {
                        current.characters = new Schemas.entryCharacters() { character = EnumerateCharacters(text) };
                    }
                }
            }

            {
                // language=regex
                var regex = @"［包摂適用[\s　]+(.+)］[\s　]*([\d、]*)";
                var match = Regex.Match(line, regex);
                if (match.Success)
                {
                    var result = new Schemas.entryInclusionApplication();
                    var match2 = Regex.Match(match.Groups[1].Value, NotePattern);
                    if (match2.Success)
                    {
                        result.Item = GetNoteSerializable(match2.Groups[1].Value);
                    }
                    else
                    {
                        result.Item = match.Groups[1].Value;
                    }

                    {
                        result.reference = match.Groups[2].Value.Split("、").Where(a=>!string.IsNullOrWhiteSpace(a))
                            .Select(a => new Schemas.entryInclusionApplicationReference() { page = a }).ToArray();
                    }

                    line = line.Replace(match.Value, "");
                    current.Item = result;
                }
            }

            {
                // language=regex
                var regex = @"［統合適用[\s　]+(.+)］";
                var match = Regex.Match(line, regex);
                if (match.Success)
                {
                    var result = new Schemas.entryIntegrationApplication();
                    var match2 = Regex.Match(match.Groups[1].Value, NotePattern);
                    if (match2.Success)
                    {
                        result.Item = GetNoteSerializable(match2.Groups[1].Value);
                    }
                    else
                    {
                        result.Item = match.Groups[1].Value;
                    }

                    line = line.Replace(match.Value, "");
                    current.Item = result;
                }
            }

            {
                // language=regex
                var regex = @"［78互換包摂[\s　]+(.+)］";
                var match = Regex.Match(line, regex);
                if (match.Success)
                {
                    var result = new Schemas.entryCompatible78Inclusion
                    {
                        @ref = match.Groups[1].Value,
                    };
                    line = line.Replace(match.Value, "");
                    current.Item = result;
                }
            }

            {
                // language=regex
                var regex = @"［デザイン差[\s　]+(.+)］";
                var match = Regex.Match(line, regex);
                if (match.Success)
                {
                    var result = new Schemas.entryDesignVariant
                    {
                        @ref = match.Groups[1].Value
                    };
                    line = line.Replace(match.Value, "");
                    current.Item = result;
                }
            }

            {
                // language=regex
                var regex = @"([^］．\s　]+)[\s　]+入力可能";
                var match = Regex.Match(line, regex);
                if (match.Success)
                {
                    line = line.Replace(match.Value, "");
                    current.Item = new object();//意味不明だけど、objectがinputableになるっぽい。
                }

                {
                    var text = Regex.Replace(match.Groups[1].Value, @"[\s　]", "");
                    if (text.Length > 0)
                    {
                        current.characters = new Schemas.entryCharacters() { character = EnumerateCharacters(text) };
                    }
                }
            }

            {
                var match = Regex.Match(line, $@"([^\d+．]+){NotePattern}");
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
                        current.note = GetNoteSerializable(match.Groups[2].Value);
                    }
                }
            }

            {
                var matches = Regex.Matches(line, @"UCV(\d+)");
                var list = current.UCV?.ToList() ?? new List<Schemas.entryUCV>();
                foreach (Match match in matches)
                {
                    list.Add(new Schemas.entryUCV() { number=match.Groups[1].Value });
                }
                current.UCV = list.ToArray();
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

    public static Schemas.note GetNoteSerializable(string text)
    {
        var texts = text.Split('、');

        var note = new Schemas.note()
        {
            full = text,
            description = texts[0],
        };
        if (texts.Length >= 2) note.Item = GetSerializableFromCharCode(texts[1]);
        return note;
    }

    public static void WriteDictionary(string path, Schemas.dictionary dictionary)
    {
        using var writerXml = System.Xml.XmlWriter.Create(path, new System.Xml.XmlWriterSettings() { Indent = true });
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(Schemas.dictionary));
        xs.Serialize(writerXml, dictionary);
        writerXml.Close();
    }

    public static object? GetSerializableFromCharCode(string text)
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
