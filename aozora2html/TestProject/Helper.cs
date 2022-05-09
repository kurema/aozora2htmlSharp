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
        var parser = new Aozora2Html(stream, output, null, null, null) { Section = Aozora2Html.SectionKind.tail };
        action?.Invoke(parser);
        try
        {
            while (true) parser.Parse();
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
            this.MidashiId = midashi_id;
            this.BlockAllowedContext = block_allowed_context;
        }

        public int MidashiId { get; set; }

        public bool BlockAllowedContext { get; set; }

        public int GenerateNewMidashiId(int size) => MidashiId;

        public int GenerateNewMidashiId(string size) => MidashiId;
    }
}
