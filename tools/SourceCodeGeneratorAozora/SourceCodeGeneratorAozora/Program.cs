using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace SourceCodeGeneratorAozora
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("shift_jis");
            string rbPath;
#if DEBUG
            rbPath = "t2hs.rb";
#else
            if (args.Length == 0) return;
            rbPath = args[0];
#endif
            using var sr = new StreamReader(rbPath);
            string line;
            string lineLast = "";

            var source = new SourceFile(args.Length >= 2 ? args[1] : Path.GetFileNameWithoutExtension(rbPath));

            var regCombined = new Regex(@"^(\w+) (@?\w+\??)$");
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
            var regCommentPartial = new Regex(@"#([^""/]*)$");
            var regRescue = new Regex(@"^rescue\s*([^=>]*)\s*=>\s*(\w)$");
            var regCatch = new Regex(@"^catch\(([^\)]+)\) do$");
            var regEach = new Regex(@"(\w+)\.(?:each|each_char) do \|(\w+)\|");
            var regIndex = new Regex(@"(\w+)\.index do \|(\w+)\|");

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

                if (structure.CurrentKind == CodeStructureKind.hash)
                {
                    var matchHash = regHashDeclare1.Match(line);
                    matchHash = matchHash.Success ? matchHash : regHashDeclare2.Match(line);

                    if (matchHash.Success)
                    {
                        structure.CurrentClass.KeyWords.Add(matchHash.Groups[1].Value, structure.Last().Title);
                        await source.AddDeclareGlobalConstDictionaryEntry(matchHash.Groups[1].Value, matchHash.Groups[2].Value);
                        continue;
                    }
                    else if (line == "}")
                    {
                        var last = structure.LastOrDefault();
                        if (!(last is null)) structure.RemoveAt(structure.Count - 1);
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
                                    structure.CurrentClass?.Methods.Add(matchDef.Groups[1].Value);
                                    await source.AddMethod(matchDef.Groups[1].Value, matchDef.Groups[2].Value);
                                }
                                else if (matchCombined.Success)
                                {
                                    structure.Add(CodeStructureKind.def, matchCombined.Groups[2].Value);
                                    structure.CurrentClass?.Methods.Add(matchCombined.Groups[2].Value);
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
                                //await source.AddComment("Current Structure:" + String.Join("/", structure.Select(a => a.Kind.ToString()).ToArray()));

                                var last = structure.LastOrDefault();
                                if (!(last is null)) structure.RemoveAt(structure.Count - 1);
                                switch (last?.Kind)
                                {
                                    case CodeStructureKind.@class:
                                        {
                                            foreach (var item in last.Members)
                                            {
                                                await source.AddDeclareGlobal(item.name, item.type, item.initial);
                                            }

                                            break;
                                        }

                                    case CodeStructureKind.@case when last.Members.Count > 0:
                                        await source.AddSwitchBreak();
                                        break;
                                    case CodeStructureKind.@catch:
                                        await source.AddCatch(last.Title);
                                        break;
                                    case CodeStructureKind.ifStatement:
                                        await source.AddEnd();
                                        await source.AddAnonymousFuncClose();
                                        continue;
                                }
                                await source.AddEnd();
                                continue;
                            }
                        case "if":
                        case "elsif":
                        case "unless":
                            {
                                var condition = regFirst.Replace(line, "");
                                condition = condition.Trim().Replace(" or ", " || ").Replace(" and ", " && ").Replace("::", ".");
                                condition = Regex.Replace(condition, @"([\w\.]+)\?\(", "$1(");
                                condition = Regex.Replace(condition, @"\(\s*:([^\)]+)\s*\)", @"(""$1"")");

                                switch (matchStart.Groups[1].Value)
                                {
                                    case "if":
                                        await source.AddIf(condition);
                                        structure.Add(CodeStructureKind.@if, condition);
                                        break;
                                    case "unless":
                                        await source.AddIf($"!({condition})");
                                        structure.Add(CodeStructureKind.@if, condition);
                                        break;
                                    case "elsif":
                                        await source.AddIfElseIf(condition);
                                        break;
                                }

                                continue;
                            }
                        case "else" when structure.CurrentKind == CodeStructureKind.@case:
                            {
                                if (structure.Last().Members.Count > 0) { await source.AddSwitchBreak(); }
                                await source.AddSwitchDefault();
                                continue;
                            }
                        case "else":
                            {
                                await source.AddIfElse();
                                continue;
                            }
                        case "case":
                            {
                                var rest = regFirst.Replace(line, "").Trim();
                                structure.Add(CodeStructureKind.@case, "");
                                await source.AddSwitch(rest);
                                continue;
                            }
                        case "when" when structure.CurrentKind == CodeStructureKind.@case:
                            {
                                var key = regFirst.Replace(line, "").Trim();
                                if (structure.Last().Members.Count > 0) { await source.AddSwitchBreak(); }
                                structure.Last().AddMember(key, "");
                                await source.AddSwitchCase(ConvertKeyword(key, structure));
                                continue;
                            }
                        case "begin":
                            {
                                structure.Add(CodeStructureKind.begin, "");
                                await source.AddTry();
                                continue;
                            }
                        case "rescue":
                            {
                                var match = regRescue.Match(line);
                                if (match.Success && string.IsNullOrWhiteSpace(match.Groups[1].Value)) await source.AddCatch($"Exception {match.Groups[2].Value.Trim()}");
                                else if (match.Success) await source.AddCatch($"{ConvertCall(match.Groups[1].Value.Trim())} {match.Groups[2].Value.Trim()}");
                                else await source.AddCatch();
                                continue;
                            }
                        case "loop":
                            {
                                structure.Add(CodeStructureKind.loop, "");
                                await source.AddWhile();
                                continue;
                            }
                    }
                }

                var matchCatch = regCatch.Match(line);
                if (matchCatch.Success)
                {
                    structure.Add(CodeStructureKind.@catch, ConvertKeyword(matchCatch.Groups[1].Value, structure));
                    await source.AddTry();
                    continue;
                }

                var matchEach = regEach.Match(line);
                if (matchEach.Success)
                {
                    structure.Add(CodeStructureKind.@foreach, "");
                    await source.AddForeach(matchEach.Groups[2].Value, matchEach.Groups[1].Value);
                    continue;
                }
                var matchIndex = regIndex.Match(line);
                if (matchIndex.Success)
                {
                    structure.Add(CodeStructureKind.index, "");
                    await source.AddComment("INDEX, FIX THIS!");
                    await source.AddComment(line);
                    await source.AddBrancket();
                    continue;
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
                    var matchFirst = regFirst.Match(varValue);

                    if (varValue == "{")
                    {
                        structure.Add(CodeStructureKind.hash, varName);
                        await source.AddDeclareGlobalStaticDictionaryStart(varName);
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
                        else if (matchFirst.Success && matchFirst.Groups[1].Value == "if")
                        {
                            var statement = ConvertCall(regFirst.Replace(varValue, "")).Trim();
                            structure.Add(CodeStructureKind.ifStatement, "");
                            await source.AddAnonymousFuncCall(varName);
                            await source.AddIf(statement);
                            continue;
                        }
                        else
                        {
                            var guessedType = GuessType(varValue, structure);
                            if (varName.StartsWith("@"))
                            {
                                varName = varName.Substring(1);
                                structure.CurrentClass?.AddMember(varName, guessedType.type);
                                await source.AddSubstitution(varName, guessedType.value);
                                continue;
                            }
                            {
                                var success = structure.LastOrDefault()?.AddMember(varName, guessedType.type) ?? false;
                                if (success) await source.AddDeclare(varName, guessedType.value, guessedType.type);
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

        public static (string type, string value) GuessType(string statement, CodeStructure structure)
        {
            Match m;
            if (statement == "nil") return ("dynamic", "null");
            if (statement == "[]") return ("List<string>", "new List<string>()");
            if (statement == "{}") return ("Dictionary<string,string>", "new Dictionary<string,string>()");
            if (statement == "true" || statement == "false") return ("bool", statement);
            if (Regex.IsMatch(statement, @"^\d+$")) return ("int", statement);
            if ((m = Regex.Match(statement, @"^:(\w+)$")).Success)
            {
                return ("string", ConvertKeyword(statement, structure));
                //var keyword = m.Groups[1].Value;
                //if (structure.CurrentClass?.KeyWords.ContainsKey(keyword) == true) return ("string", $"\"{structure.CurrentClass.KeyWords[keyword]}[\"{m.Groups[1].Value}\"]\"");
                //else return ("string", $"\"{statement}\"");
            }
            return ("dynamic", statement);
        }

        public static string ConvertKeyword(string statement, CodeStructure structure)
        {
            Match m;
            if ((m = Regex.Match(statement, @"^:(\w+)$")).Success)
            {
                var keyword = m.Groups[1].Value;
                if (structure.CurrentClass?.KeyWords.ContainsKey(keyword) == true) return $"\"{structure.CurrentClass.KeyWords[statement]}[\"{m.Groups[1].Value}\"]\"";
                else return $"\"{statement}\"";
            }
            return statement;
        }

        public static string ConvertCall(string word)
        {
            word = word.Trim();
            word = word.Replace("::", ".");
            word = word.Replace("?(", "(");
            word = Regex.Replace(word, @"\?$", "");
            var reg = new Regex(@"\.new\(");
            if (reg.IsMatch(word))
            {
                reg.Replace(word, @"\(");
                word = "new " + word;
            }
            return word;
        }
    }
}
