using System;
using System.Text.RegularExpressions;

namespace Aozora.Helpers.Tag;

/// <summary>
/// 欧文アクセント文字用
/// </summary>
public partial class Accent : Inline, IHtmlProvider
{
    readonly Aozora2Html Parser;

    public Accent(Aozora2Html parser, string code, string name, string gaiji_dir) : base()
    {
        Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.Code = code ?? throw new ArgumentNullException(nameof(code));
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
        this.GaijiDir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public string Code { get; }
    public string Name { get; }
    public string GaijiDir { get; }

    //kurema:Jisx0213_to_unicodeはEmbedGaijiとAccent内にありましたが、合わせてGaijiに移動しました。

    public override CharType CharType => CharType.Hankaku;

#if NET7_0_OR_GREATER
    [GeneratedRegex(".*/")]
    private static partial Regex PAT_ToHtml_GEN();
#endif

    public string ToHtml()
    {
        if (Parser.UseJisx0213Accent)
        {
#if NET7_0_OR_GREATER
            var regex = PAT_ToHtml_GEN();
#else
            var regex = new Regex(".*/");
#endif
            return Gaiji.ConvertJisx0213ToUnicode(regex.Replace(Code, "", 1));
        }
        else
        {
            return $"<img src=\"{GaijiDir}{Code}.png\" alt=\"{Aozora2Html.GAIJI_MARK}({Name})\" class=\"gaiji\" />";
        }
    }

}