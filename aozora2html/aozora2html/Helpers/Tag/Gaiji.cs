using System;

namespace Aozora.Helpers.Tag;

/// <summary>
/// 外字用
/// </summary>
public class Gaiji : Inline
{
    public override CharType char_type => CharType.Kanji;

    public static string Jisx0213_to_unicode(string code) => JIS2UCS.Convert(code) ?? throw new ArgumentOutOfRangeException(nameof(code));
}

/// <summary>
/// 外字注記用
/// </summary>
public class EmbedGaiji : Gaiji, IHtmlProvider
{
    Aozora2Html Parser;

    public EmbedGaiji(Aozora2Html parser, string folder, string code, string name, string gaiji_dir, string unicode_num = null)
    {
        Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.folder = folder ?? throw new ArgumentNullException(nameof(folder));
        this.code = code;
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.unicode = unicode_num;
        this.gaiji_dir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public string folder { get; }
    public string? code { get; }
    public string name { get; }
    public string? unicode { get; }
    public string gaiji_dir { get; }

    public string to_html()
    {
        if (Parser.use_jisx0214_embed_gaiji && !string.IsNullOrWhiteSpace(code))
        {
            return Jisx0213_to_unicode(code!);
        }
        else if (Parser.use_unicode_embed_gaiji && !string.IsNullOrEmpty(unicode))
        {
            return $"&#x{unicode};";
        }
        else
        {
            return $"<img src=\"{gaiji_dir}{folder}/{code}.png\" alt=\"{Aozora2Html.GAIJI_MARK}({name})\" class=\"gaiji\" />";
        }
    }
}

/// <summary>
/// 非埋め込み外字
/// </summary>
public class UnEmbedGaiji : Gaiji,IHtmlProvider
{
    public UnEmbedGaiji(string desc)
    {
        this.desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    public string desc { get; }
    public bool escaped { get; protected set; } = false;

    public bool escape() => escaped = true;

    public string to_html() => $"<span class=\"notes\">［{desc}］</span>";
}
