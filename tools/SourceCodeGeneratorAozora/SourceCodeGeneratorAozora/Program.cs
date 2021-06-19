using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace SourceCodeGeneratorAozora
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("shift_jis");
#if DEBUG
            using var sr = new StreamReader("t2hs.rb");
#else
            if (args.Length == 0) return;
            using var sr = new StreamReader(args[0]);
#endif
            string line;
            string lineLast = "";

            var source = new SourceFile("Aozora2Html");

            var regCombined = new Regex(@"^(\w+) (\w+\??)$");
            var regFirst = new Regex(@"^(\w+)");
            var regDef = new Regex(@"^def (\w+\??)\s*\(([^()]+)\)");
            var regDec = new Regex(@"^(@?\w+)\s*\=\s*(.+)$");
            var regText = new Regex(@"^\s*""((?:[^""]*|\"")*)""\s*$");
            //["18e5"].pack("h*").force_encoding("shift_jis")
            var regJikauchi = new Regex(@"^\s*\[""(\w{4})""\]\.pack\(\s*""h\*""\s*\)\.force_encoding\(\s*""shift_jis""\s*\)\s*$");
            var regReg = new Regex(@"^\s*/([^/]+)/\s*$");
            var regReg2 = new Regex(@"^\s*\/#\{""([^""]+)""\}\/\s*$");
            var regContinueLine = new Regex(@"or$|\+$");
            var regHashDeclare1 = new Regex(@"^:(\w+)\s*\=\>\s*""([^""]*)""\s*,?$");
            var regHashDeclare2 = new Regex(@"^""(\w+)""\s*\=\>\s*""([^""]*)""\s*,?$");
            var regComment = new Regex("^#(.*)$");

            var structure = new CodeStructure();

            while ((line = await sr.ReadLineAsync()) != null)
            {
                string lineOrg = line;
                line = Regex.Replace(line.Trim(), @"#.*$", "").Replace(".to_sjis", "");
                line = lineLast + line;
                lineLast = "";

                var matchComment = regComment.Match(lineOrg.Trim());
                if (matchComment.Success)
                {
                    await source.AddComment(matchComment.Groups[1].Value);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    await source.AddWithIndent("");
                    continue;
                }

                if (regContinueLine.Match(line).Success)
                {
                    lineLast = line + " ";
                    continue;
                }

                var matchCombined = regCombined.Match(line);
                var matchStart = regFirst.Match(line);

                if (structure.IsLastHash)
                {
                    var matchHash = regHashDeclare1.Match(line);
                    matchHash = matchHash.Success ? matchHash : regHashDeclare2.Match(line);

                    if (matchHash.Success)
                    {
                        await source.AddDeclareGlobalConstDictionaryEntry(matchHash.Groups[1].Value, matchHash.Groups[2].Value);
                        continue;
                    }
                    else if (line == "}")
                    {
                        await source.AddDeclareGlobalConstDictionaryEnd();
                        continue;
                    }
                }

                if (matchStart.Success)
                {
                    switch (matchStart.Groups[1].Value)
                    {
                        case "class":
                            {
                                if (!matchCombined.Success) throw new Exception();
                                structure.Add(CodeStructureKind.@class, matchCombined.Groups[2].Value);
                                await source.AddClass(matchCombined.Groups[2].Value);
                                continue;
                            }
                        case "def":
                            {
                                var matchDef = regDef.Match(line);
                                if (matchDef.Success)
                                {
                                    structure.Add(CodeStructureKind.def, matchDef.Groups[1].Value);
                                    await source.AddMethod(matchDef.Groups[1].Value, matchDef.Groups[2].Value);
                                }
                                else if (matchCombined.Success)
                                {
                                    structure.Add(CodeStructureKind.def, matchCombined.Groups[2].Value);
                                    await source.AddMethod(matchCombined.Groups[2].Value, "");
                                }
                                else
                                {
                                    await source.AddComment(line);
                                    throw new Exception();
                                }
                                continue;
                            }
                        case "end":
                            {
                                if (structure.Count > 0) structure.RemoveAt(structure.Count - 1);
                                await source.AddEnd();
                                continue;
                            }
                        case "if":
                        case "elsif":
                            {
                                var condition = regFirst.Replace(line, "");
                                condition = condition.Trim().Replace(" or ", " || ").Replace(" and ", " && ").Replace("::", ".");
                                condition = Regex.Replace(condition, @"([\w\.]+)\?\(", "$1(");
                                condition = Regex.Replace(condition, @"\(\s*:([^\)]+)\s*\)", @"(""$1"")");

                                if (matchStart.Groups[1].Value == "if")
                                {
                                    await source.AddIf(condition);
                                    structure.Add(CodeStructureKind.@if, condition);
                                }
                                else await source.AddIfElseIf(condition);

                                continue;
                            }
                        case "else":
                            {
                                await source.AddIfElse();
                                continue;
                            }
                    }
                }

                var matchDec = regDec.Match(lineOrg.Trim().Replace(".to_sjis", ""));
                matchDec = matchDec.Success ? matchDec : regDec.Match(line);

                if (matchDec.Success)
                {
                    var varName = matchDec.Groups[1].Value;
                    var varValue = matchDec.Groups[2].Value;
                    varValue = Regex.Replace(varValue, @"^\((.+)\)$", "$1");
                    varValue = Regex.Replace(varValue, @"""\s*\+\s*""", "");
                    var matchText = regText.Match(varValue);
                    var matchReg = regReg.Match(varValue);
                    var matchReg2 = regReg2.Match(varValue);
                    var matchJikauchi = regJikauchi.Match(varValue);

                    if (varValue == "{")
                    {
                        structure.Add(CodeStructureKind.hash, varName);
                        await source.AddDeclareGlobalConstDictionaryStart(varName);
                        continue;
                    }
                    else if (structure.IsInDef)
                    {
                        if (matchText.Success)
                        {
                            var varValueContent = matchText.Groups[1].Value;
                            await source.AddDeclareString(varName, varValueContent);
                            continue;
                        }
                        else
                        {
                            await source.AddComment(line);
                            continue;
                        }
                    }
                    else
                    {
                        if (matchText.Success)
                        {
                            await source.AddDeclareGlobalConstAuto(varName, matchText.Groups[1].Value);
                            continue;
                        }
                        else if (matchReg2.Success)
                        {
                            await source.AddDeclareGlobalConstAuto(varName, matchReg2.Groups[1].Value);
                            continue;
                        }
                        else if (matchReg.Success)
                        {
                            await source.AddDeclareGlobalConstAuto(varName, matchReg.Groups[1].Value);
                            continue;
                        }
                        else if (matchJikauchi.Success)
                        {
                            await source.AddDeclareGlobalConstChar(varName,
                                GetStringFromByteStringNibbleReverse(matchJikauchi.Groups[1].Value)
                                );
                            continue;
                        }
                        else
                        {
                            await source.AddComment(line);
                            continue;
                        }
                    }
                }
                await source.AddComment(line);
            }
            await source.Close();
        }

        public static string GetStringFromByteStringNibbleReverse(string text)
        {
            byte[] result = new byte[text.Length / 2];
            for (int i = 0; i < text.Length; i += 2)
            {
                string w = text.Substring(i, 2);
                //.pack("h*")は「16進文字列(下位ニブルが先)」。普通は逆じゃない？
                //https://docs.ruby-lang.org/ja/latest/doc/pack_template.html
                result[i / 2] = Convert.ToByte(string.Concat(w[1], w[0]), 16);
            }
            var sjis = Encoding.GetEncoding("shift_jis");
            return sjis.GetString(result);
        }

        public class CodeStructure : List<CodeStructureItem>
        {
            public void Add(CodeStructureKind kind, string title)
            {
                //Console.WriteLine(kind.ToString() + " " + title);
                this.Add(new CodeStructureItem(kind, title));
            }

            public bool IsInDef => this.Any(a => a.Kind == CodeStructureKind.def);
            public bool IsLastHash => this.Count > 0 && this.Last().Kind == CodeStructureKind.hash;
        }

        public record CodeStructureItem(CodeStructureKind Kind, string Title);

        public enum CodeStructureKind
        {
            @class, def, @if, elsif, @case, end, begin, hash
        }

        public class SourceFile
        {
            StreamWriter streamWriter;
            public string ClassName { get; set; }
            public int CurrentLevel { get; private set; } = 0;
            public const int SpacePerLevel = 4;

            public SourceFile(string className)
            {
                streamWriter = new StreamWriter(className + ".cs", false);
                ClassName = className;

                streamWriter.WriteLine($@"using System;
using System.Text.RegularExpressions;

namespace aozora2html
{{
");
                CurrentLevel++;
            }

            public async Task AddComment(string content)
            {
                await AddWithIndent($"//{content.Replace("//", "")}");
            }

            public async Task AddIf(string condition)
            {
                await AddWithIndent($"if ({condition})");
                await AddWithIndent("{");
                CurrentLevel++;
            }

            public async Task AddIfElseIf(string condition)
            {
                await AddWithIndent("}");
                await AddWithIndent($"else if ({condition})");
                await AddWithIndent("{");
            }

            public async Task AddIfElse()
            {
                CurrentLevel--;
                await AddWithIndent("}");
                await AddWithIndent($"else");
                await AddWithIndent("{");
                CurrentLevel++;
            }

            public async Task AddClass(string className)
            {
                await AddWithIndent($"public class {className}");
                await AddWithIndent("{");
                CurrentLevel++;
            }

            public async Task AddMethod(string method, string argsText)
            {
                var args = string.IsNullOrWhiteSpace(argsText) ? new string[0] : new Regex(@",\s*").Split(argsText);
                var sb = new StringBuilder();
                if (method.EndsWith("?")) sb.Append($"public bool {method.Replace("?", "")}(");
                else sb.Append($"public dynamic {method}(");
                sb.Append(string.Join(",", args.Select(a => $"dynamic {a}")));
                sb.Append(")");
                await AddWithIndent("");
                await AddWithIndent(sb.ToString());
                await AddWithIndent(@"{");
                CurrentLevel++;

                var val = new Dictionary<string, string>()
                {
                    {"","" },

                };
            }

            public async Task AddDeclareGlobalConstDictionaryStart(string name)
            {
                await AddWithIndent("");
                await AddWithIndent($"public const Dictionary<string, string>() {name} = new Dictionary<string, string>()");
                await AddWithIndent($"{{");
                CurrentLevel++;
            }

            public async Task AddDeclareGlobalConstDictionaryEntry(string key, string value)
            {
                await AddWithIndent($"{{\"{key}\", \"{value}\"}},");
            }

            public async Task AddDeclareGlobalConstDictionaryEnd()
            {
                CurrentLevel--;
                await AddWithIndent("};");
            }


            public async Task AddDeclareGlobalConstChar(string name, string value)
            {
                await AddWithIndent($"public const char {name} = '{value}';");
            }

            public async Task AddDeclareGlobalConstString(string name, string value)
            {
                await AddWithIndent($"public const string {name} = \"{value}\";");
            }

            public async Task AddDeclareGlobalConstRegex(string name, string value)
            {
                await AddWithIndent($"public const Regex {name} = new Regex(@\"{value}\");");
            }

            public async Task AddDeclareGlobalConstAuto(string name, string value)
            {
                if (name.StartsWith("PAT_")) await AddDeclareGlobalConstRegex(name, value);
                else if (value.Length == 1) await AddDeclareGlobalConstChar(name, value);
                else await AddDeclareGlobalConstString(name, value);

            }

            public async Task AddDeclareString(string name, string value)
            {
                await AddWithIndent($"string {name} = \"{value}\";");
            }


            public async Task AddEnd()
            {
                CurrentLevel--;
                await AddWithIndent("}");
            }

            public async Task AddWithIndent(string text)
            {
                await streamWriter.WriteLineAsync($"{new string(' ', SpacePerLevel * Math.Max(CurrentLevel, 0))}{text}");
            }

            public async Task Close()
            {
                await streamWriter.WriteLineAsync($@"
}}
");
                await streamWriter.FlushAsync();
                streamWriter.Close();
                await streamWriter.DisposeAsync();
                streamWriter = null;
            }
        }
    }
}
