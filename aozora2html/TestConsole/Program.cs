// See https://aka.ms/new-console-template for more information
Test1();

void Test1()
{
    var str = "〔e'tiquette\r\n";
    using (var sr = new System.IO.StringReader(str))
    {
        var stream = new Aozora.Jstream(sr);
        var output = new Aozora.Helpers.OutputConsole();
        var warn = new Aozora.Helpers.OutputString();
        var parsed = new Aozora.Helpers.AccentParser(stream, '〕', new(), new(), output, warnChannel: warn, gaiji_dir: "g_dir/").process().to_html();
        //kurema:元のテストでは行末は"\n"でしたが、こちらでは"\r\n"にしています。
        var result = warn.ToString();
    }
}