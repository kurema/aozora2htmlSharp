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
    public static Aozora.Aozora2Html GetAozora2HtmlPlaceholder()
    {
        using var sr = new System.IO.StringReader("test");
        var stream = new Jstream(sr);
        var output = new OutputString();
        return new Aozora2Html(stream, output, null, null, null);
    }

}
