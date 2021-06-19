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
#if DEBUG
            using var sr = new StreamReader("t2hs.rb");
#else
            if (args.Length == 0) return;
            using var sr = new StreamReader(args[0]);
#endif
            string line;

            var source = new SourceFile("Aozora2Html");

            var regCombined = new Regex(@"^\s*(\w+) (\w+\??)\s*$");
            var regFirst = new Regex(@"^\s*(\w+)");
            var regDef = new Regex(@"^\s*def (\w+\??)\s*\(([^()]+)\)");
            var regDec = new Regex(@"^\s*\w+\s*\=\s*.+$");

            var structure = new CodeStructure();

            while ((line = await sr.ReadLineAsync()) != null)
            {
                var matchCombined = regCombined.Match(line);
                var matchStart = regFirst.Match(line);

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
                    }
                }

                var matchDec = regDec.Match(line);
                if (matchDec.Success)
                {
                    if (structure.IsInDef)
                    {

                    }
                    else
                    {

                    }
                }
            }
            await source.Close();
        }

        public class CodeStructure : List<CodeStructureItem>
        {
            public void Add(CodeStructureKind kind, string title)
            {
                Console.WriteLine(kind.ToString() + " " + title);
                this.Add(new CodeStructureItem(kind, title));
            }

            public bool IsInDef => this.Any(a => a.Kind == CodeStructureKind.def);
        }

        public record CodeStructureItem(CodeStructureKind Kind, string Title);

        public enum CodeStructureKind
        {
            @class, def, @if, elsif, @case, end, begin
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

                streamWriter.WriteLine($@"namespace aozora2html
{{
");
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
                if (method.EndsWith("?")) sb.Append($"public bool {method.Replace("?","")}(");
                else sb.Append($"public dynamic {method}(");
                sb.Append(string.Join(",", args.Select(a => $"dynamic {a}")));
                sb.Append(")");
                await AddWithIndent(sb.ToString());
                await AddWithIndent(@"{");
                CurrentLevel++;
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
