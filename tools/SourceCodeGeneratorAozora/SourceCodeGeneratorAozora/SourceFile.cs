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
        public class SourceFile
        {
            StreamWriter streamWriter;
            public string ClassName { get; set; }
            private int _CurrentLevel = 0;
            public int CurrentLevel { get => _CurrentLevel; private set => _CurrentLevel = Math.Max(0, value); }
            public const int SpacePerLevel = 4;

            public SourceFile(string className)
            {
                streamWriter = new StreamWriter(className + ".cs", false);
                ClassName = className;

                streamWriter.WriteLine($@"using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Aozora
{{
");
                CurrentLevel++;
            }

            public async Task AddComment(string content)
            {
                await Add($"//{content.Replace("//", "")}");
            }

            public async Task AddAnonymousFuncCall(string varName)
            {
                string header = string.IsNullOrEmpty(varName) ? "" : $"{varName} = ";
                await Add($"{header}new Func<dynamic>(() => {{");
                CurrentLevel++;
            }

            public async Task AddAnonymousFuncClose()
            {
                await Add("}).Invoke();");
                CurrentLevel--;
            }

            public async Task AddIf(string condition)
            {
                await Add($"if ({condition})");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddForeach(string varName, string from)
            {
                await Add($"foreach (var {varName} in {from})");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddTry()
            {
                await Add("try");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddCatch(string statement = null)
            {
                CurrentLevel--;
                await Add("}");
                if (string.IsNullOrWhiteSpace(statement)) await Add("catch");
                else await Add($"catch ({statement})");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddIfElseIf(string condition)
            {
                CurrentLevel--;
                await Add("}");
                await Add($"else if ({condition})");
                await Add("{");
                CurrentLevel++;
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

            public async Task AddMethod(string method, string argsText, string className = null)
            {
                var args = string.IsNullOrWhiteSpace(argsText) ? new string[0] : new Regex(@",\s*").Split(argsText);
                var sb = new StringBuilder();
                if (method.EndsWith("?")) sb.Append($"public bool {method.Replace("?", "")}(");
                else if (method == "initialize" && className != null) sb.Append($"public {className}(");
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

            public async Task AddWhile(string statement = "true")
            {
                await Add($"while({statement})");
                await Add("{");
                CurrentLevel++;
            }

            public async Task AddDeclareGlobalStaticDictionaryStart(string name)
            {
                await Add("");
                await Add($"public static Dictionary<string, string> {name} = new Dictionary<string, string>()");
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
                await Add($"private {option}Regex _{name} = null;");
                await Add($"public {option}Regex {name} => _{name} ??= new Regex(@\"{value}\");");
            }

            public async Task AddDeclareGlobalAuto(string name, string value)
            {
                string option = Regex.IsMatch(name, "^[A-Z]") ? "const " : "";
                if (name.StartsWith("PAT_")) await AddDeclareGlobalRegex(name, value, "static ");
                else if (value.Length == 1) await AddDeclareGlobalChar(name, value, option);
                else await AddDeclareGlobalString(name, value, option);
            }

            public async Task AddDeclareGlobal(string name, string type, string value, string option = "")
            {
                if (string.IsNullOrEmpty(value)) await Add($"public {option}{type} {name};");
                else await Add($"public {option}{type} {name} = {value};");
            }

            public async Task AddDeclareGlobalPrivate(string name, string type, string value, string option = "")
            {
                if (string.IsNullOrEmpty(value)) await Add($"private {option}{type} {name};");
                else await Add($"private {option}{type} {name} = {value};");
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

            public async Task AddSwitchCase(string key)
            {
                await Add($"case {key}:");
                CurrentLevel++;
            }

            public async Task AddSwitchBreak()
            {
                CurrentLevel--;
                await Add($"break;");
            }

            public async Task AddSwitchDefault()
            {
                await Add($"default:");
                CurrentLevel++;
            }

            public async Task AddBrancket()
            {
                await Add("{");
                CurrentLevel++;
            }

            public async Task Add(string text)
            {
                await streamWriter.WriteLineAsync($"{new string(' ', SpacePerLevel * Math.Max(CurrentLevel, 0))}{text}");
            }

            public async Task AddSwitch(string key)
            {
                await Add($"switch ({key})");
                await Add("{");
                CurrentLevel++;
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
