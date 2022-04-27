using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Aozora;
using Aozora.Helpers;
using Aozora.Helpers.Tag;

namespace TestProject;

public static class Helper
{
    public static Aozora2Html GetAozora2HtmlPlaceholder()
    {
        using var sr = new System.IO.StringReader("test");
        var stream = new Jstream(sr);
        var output = new OutputString();
        return new Aozora2Html(stream, output, null, null, null);
    }

    public static string? ParseText(string input, Action<Aozora2Html>? action = null)
    {
        using var sr = new System.IO.StringReader(input);
        var stream = new Jstream(sr);
        var output = new OutputString();
        var parser = new Aozora2Html(stream, output, null, null, null) { section = Aozora2Html.SectionKind.tail };
        action?.Invoke(parser);
        try
        {
            while (true) parser.parse();
        }
        catch (Aozora.Exceptions.TerminateException)
        {
        }

        return output.ToString();
    }
    public class MidashiIdProviderPlaceholder : INewMidashiIdProvider
    {
        public MidashiIdProviderPlaceholder(int midashi_id, bool block_allowed_context)
        {
            this.midashi_id = midashi_id;
            this.block_allowed_context = block_allowed_context;
        }

        public int midashi_id { get; set; }

        public bool block_allowed_context { get; set; }

        public int new_midashi_id(int size) => midashi_id;

        public int new_midashi_id(string size) => midashi_id;
    }
}
