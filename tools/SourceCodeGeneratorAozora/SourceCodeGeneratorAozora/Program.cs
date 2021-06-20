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
            var regCommentPartial = new Regex(@"#([^""]*)$");

            var structure = new CodeStructure();

            while ((line = await sr.ReadLineAsync()) != null)
            {
                string lineOrg = line;
                line = line.Trim();
                string lineComment = "";
                {
                    var matchCommentP = regCommentPartial.Match(line);
                    lineComment = matchCommentP.Success ? matchCommentP.Groups[1].Value : "";
                    line = regCommentPartial.Replace(line, "");
                }
                line = line.Replace(".to_sjis", "");
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
                    await source.Add("");
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
                                var last = structure.LastOrDefault();
                                if (!(last is null)) structure.RemoveAt(structure.Count - 1);
                                if (last?.Kind == CodeStructureKind.@class)
                                {
                                    foreach (var item in last.Members)
                                    {
                                        await source.AddDeclareGlobal(item.name, item.type, item.initial);
                                    }
                                }
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

                var matchDec = regDec.Match(line);

                if (matchDec.Success)
                {
                    var varName = matchDec.Groups[1].Value;
                    var varValue = matchDec.Groups[2].Value.Trim();
                    varValue = Regex.Replace(varValue, @"^\((.+)\)$", "$1");
                    varValue = Regex.Replace(varValue, @"""\s*\+\s*""", "");
                    var matchText = regText.Match(varValue);
                    var matchReg = regReg.Match(varValue);
                    var matchReg2 = regReg2.Match(varValue + lineComment);
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
                            var guessedType = GuessType(varValue);
                            if (varName.StartsWith("@"))
                            {
                                varName = varName.Substring(1);
                                structure.CurrentClass?.AddMember(varName, guessedType.type);
                                await source.AddSubstitution(varName, guessedType.value);
                                continue;
                            }
                            {
                                var success = structure.LastOrDefault()?.AddMember(varName, guessedType.type) ?? false;
                                if(success) await source.AddDeclare(varName, guessedType.value, guessedType.type);
                                else await source.AddSubstitution(varName, guessedType.value);
                            }
                            continue;
                        }
                    }
                    else
                    {
                        if (matchText.Success)
                        {
                            await source.AddDeclareGlobalAuto(varName, matchText.Groups[1].Value);
                            continue;
                        }
                        else if (matchReg2.Success)
                        {
                            await source.AddDeclareGlobalAuto(varName, matchReg2.Groups[1].Value);
                            continue;
                        }
                        else if (matchReg.Success)
                        {
                            await source.AddDeclareGlobalAuto(varName, matchReg.Groups[1].Value);
                            continue;
                        }
                        else if (matchJikauchi.Success)
                        {
                            await source.AddDeclareGlobalChar(varName,
                                GetStringFromByteStringNibbleReverse(matchJikauchi.Groups[1].Value)
                                , "const "
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

        public static (string type, string value) GuessType(string statement)
        {
            Match m;
            if (statement == "nil") return ("dynamic", "null");
            if (statement == "[]") return ("List<string>", "new List<string>()");
            if (statement == "{}") return ("Dictionary<string,string>", "new Dictionary<string,string>()");
            if (statement == "true" || statement == "false") return ("bool", statement);
            if (Regex.IsMatch(statement, @"^\d+$")) return ("int", statement);
            if ((m = Regex.Match(statement, @"^:(\w+)$")).Success) return ("string", $"\"{m.Groups[1].Value}\"");
            return ("dynamic", statement);
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

            public CodeStructureItem CurrentClass => this.LastOrDefault(a => a.Kind == CodeStructureKind.@class);
        }

        public record CodeStructureItem(CodeStructureKind Kind, string Title)
        {
            public string[] Args { get; init; }
            public List<CodeStructureItemMember> Members { get; } = new List<CodeStructureItemMember>();

            public bool AddMember(string name, string type, string initial = null)
            {
                if (Members.Any(a => a.name == name)) return false;
                Members.Add(new CodeStructureItemMember(name, type, initial));
                return true;
            }
        };

        public record CodeStructureItemMember(string name, string type, string initial);

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
                await Add($"//{content.Replace("//", "")}");
            }

            public async Task AddIf(string condition)
            {
                await Add($"if ({condition})");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddIfElseIf(string condition)
            {
                await Add("}");
                await Add($"else if ({condition})");
                await Add("{");
            }

            public async Task AddIfElse()
            {
                CurrentLevel--;
                await Add("}");
                await Add($"else");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddClass(string className)
            {
                await Add($"public class {className}");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddMethod(string method, string argsText)
            {
                var args = string.IsNullOrWhiteSpace(argsText) ? new string[0] : new Regex(@",\s*").Split(argsText);
                var sb = new StringBuilder();
                if (method.EndsWith("?")) sb.Append($"public bool {method.Replace("?", "")}(");
                else sb.Append($"public dynamic {method}(");
                sb.Append(string.Join(", ", args.Select(a => $"dynamic {a}")));
                sb.Append(")");
                await Add("");
                await Add(sb.ToString());
                await Add(@"{");
                CurrentLevel++;

                var val = new Dictionary<string, string>()
                {
                    {"","" },

                };
            }

            public async Task AddDeclareGlobalConstDictionaryStart(string name)
            {
                await Add("");
                await Add($"public const Dictionary<string, string>() {name} = new Dictionary<string, string>()");
                await Add($"{{");
                CurrentLevel++;
            }

            public async Task AddDeclareGlobalConstDictionaryEntry(string key, string value)
            {
                await Add($"{{\"{key}\", \"{value}\"}},");
            }

            public async Task AddDeclareGlobalConstDictionaryEnd()
            {
                CurrentLevel--;
                await Add("};");
            }


            public async Task AddDeclareGlobalChar(string name, string value, string option = "")
            {
                await AddDeclareGlobal(name, "char", $"'{value}'", option);
            }

            public async Task AddDeclareGlobalString(string name, string value, string option = "")
            {
                await AddDeclareGlobal(name, "string", $"\"{value}\"", option);
            }

            public async Task AddDeclareGlobalRegex(string name, string value, string option = "")
            {
                await AddDeclareGlobal(name, "Regex", $"new Regex(@\"{value}\")", option);
            }

            public async Task AddDeclareGlobalAuto(string name, string value)
            {
                string option = Regex.IsMatch(name, "^[A-Z]") ? "const " : "";
                if (name.StartsWith("PAT_")) await AddDeclareGlobalRegex(name, value, option);
                else if (value.Length == 1) await AddDeclareGlobalChar(name, value, option);
                else await AddDeclareGlobalString(name, value, option);
            }

            public async Task AddDeclareGlobal(string name, string type, string value, string option = "")
            {
                if (string.IsNullOrEmpty(value)) await Add($"public {option}{type} {name};");
                else await Add($"public {option}{type} {name} = {value};");
            }

            public async Task AddDeclareString(string name, string value)
            {
                await AddDeclare(name, $"\"{value}\"", "string");
            }

            public async Task AddDeclare(string name, string value, string type)
            {
                await Add($"{type} {name} = {value};");
            }

            public async Task AddSubstitution(string name, string value)
            {
                await Add($"{name} = {value};");
            }

            public async Task AddEnd()
            {
                CurrentLevel--;
                await Add("}");
            }

            public async Task Add(string text)
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
