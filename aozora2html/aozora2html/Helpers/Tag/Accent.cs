using System;
using System.Text.RegularExpressions;

namespace Aozora.Helpers.Tag;

/// <summary>
/// 欧文アクセント文字用
/// </summary>
public class Accent : Inline, IHtmlProvider
{
    Aozora2Html Parser;

    public Accent(Aozora2Html parser, string code, string name, string gaiji_dir) : base()
    {
        Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.code = code ?? throw new ArgumentNullException(nameof(code));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.gaiji_dir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public string code { get; }
    public string name { get; }
    public string gaiji_dir { get; }

    //kurema:Jisx0213_to_unicodeはEmbedGaijiとAccent内にありましたが、合わせてGaijiに移動しました。

    public override CharType char_type => CharType.Hankaku;

    public string to_html()
    {
        if (Parser.use_jisx0213_accent)
        {
            var regex = new Regex(".*/");
            return Gaiji.Jisx0213_to_unicode(regex.Replace(code, "", 1));
        }
        else
        {
            return $"<img src=\"{gaiji_dir}{code}.png\" alt=\"{Aozora2Html.GAIJI_MARK}({name})\" class=\"gaiji\" />";
        }
    }

}