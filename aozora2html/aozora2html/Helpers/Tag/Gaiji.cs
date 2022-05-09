using System;

namespace Aozora.Helpers.Tag;

/// <summary>
/// 外字用
/// </summary>
public class Gaiji : Inline
{
    public override CharType CharType => CharType.Kanji;

    public static string ConvertJisx0213ToUnicode(string code) => YamlValues.Jisx0213ToUnicode(code) ?? throw new ArgumentOutOfRangeException(nameof(code));
}

/// <summary>
/// 外字注記用
/// </summary>
public class EmbedGaiji : Gaiji, IHtmlProvider
{
    readonly Aozora2Html Parser;

    public EmbedGaiji(Aozora2Html parser, string? folder, string? code, string name, string gaiji_dir, string? unicode_num = null)
    {
        Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.Folder = folder ?? "";
        this.Code = code;
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.Unicode = unicode_num;
        this.GaijiDir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public string Folder { get; }
    public string? Code { get; }
    public string Name { get; }
    public string? Unicode { get; }
    public string GaijiDir { get; }

    public string ToHtml()
    {
        if (Parser.UseJisx0214EmbedGaiji && !string.IsNullOrWhiteSpace(Code))
        {
            return ConvertJisx0213ToUnicode(Code!);
        }
        else if (Parser.UseUnicodeEmbedGaiji && !string.IsNullOrEmpty(Unicode))
        {
            return $"&#x{Unicode};";
        }
        else
        {
            return $"<img src=\"{GaijiDir}{Folder}/{Code}.png\" alt=\"{Aozora2Html.GAIJI_MARK}({Name})\" class=\"gaiji\" />";
        }
    }
}

/// <summary>
/// 非埋め込み外字
/// </summary>
public class UnEmbedGaiji : Gaiji, IHtmlProvider
{
    public UnEmbedGaiji(string desc)
    {
        this.Desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    public string Desc { get; }
    public bool Escaped { get; protected set; } = false;

    public bool Escape() => Escaped = true;

    public string ToHtml() => $"<span class=\"notes\">［{Desc}］</span>";
}
