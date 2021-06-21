using System.Collections.Generic;
using System.Linq;

namespace SourceCodeGeneratorAozora
{
    partial class Program
    {
        public class CodeStructure : List<CodeStructureItem>
        {
            public void Add(CodeStructureKind kind, string title)
            {
                //Console.WriteLine(kind.ToString() + " " + title);
                this.Add(new CodeStructureItem(kind, title));
            }

            public bool IsInDef => this.Any(a => a.Kind == CodeStructureKind.def);

            public CodeStructureItem CurrentClass => GetCurrentOfKind(CodeStructureKind.@class);
            public CodeStructureItem GetCurrentOfKind(CodeStructureKind kind) => this.LastOrDefault(a => a.Kind == kind);

            public CodeStructureKind? CurrentKind => this.LastOrDefault()?.Kind;
        }

        public record CodeStructureItem(CodeStructureKind Kind, string Title)
        {
            public string[] Args { get; init; }
            public List<CodeStructureItemMember> Members { get; } = new List<CodeStructureItemMember>();

            public List<string> Methods { get; } = new List<string>();

            public Dictionary<string, string> KeyWords { get; } = new Dictionary<string, string>();

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
            @class, def, @if, elsif, @case, end, begin, hash, loop
        }
    }
}
