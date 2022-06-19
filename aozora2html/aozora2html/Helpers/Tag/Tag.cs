using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aozora.Helpers.Tag;

public enum CharType
{
    Else, Katankana, Kanji, Hankaku, Hiragana, Zenkaku, HankakuTerminate
}

public interface IClosable
{
    public string CloseTag();
}

public interface ICharTypeProvider
{
    public CharType CharType { get; }
}

public interface IHtmlProvider
{
    public string ToHtml();

}

/// <summary>
/// 変換される青空記法class
/// </summary>
public class Tag : ICharTypeProvider
{
    //debug用
    public string Inspect() { return GetHtml(this); }

    public virtual CharType CharType => CharType.Else;

    public static void ThrowSyntaxError()
    {
        throw new Exceptions.TagSyntaxException();
    }

    public static string GetHtml(Tag tag)
    {
        if (tag is IHtmlProvider provider) return provider.ToHtml();
        else return tag?.ToString() ?? "";
    }
}

/// <summary>
/// ブロックタグ用class
///
/// 各Tagクラスはこれを継承する
/// </summary>
public class Block : Tag, IClosable
{
    public Block(INewMidashiIdProvider parser) : base()
    {
        if (!parser.BlockAllowedContext) ThrowSyntaxError();
    }

    //必要に基づきmethod overrideする
    public virtual string CloseTag() => "</div>";
}

public class Indent : Block
{
    public Indent(INewMidashiIdProvider parser) : base(parser)
    {
    }
}

/// <summary>
/// 地付き記法 
/// 
/// 直接使わない。実際に使うのはサブクラス
/// </summary>
public class Chitsuki : Indent, IHtmlProvider
{
    public int Length { get; }

    public Chitsuki(INewMidashiIdProvider parser, int length) : base(parser)
    {
        this.Length = length;
    }

    public string ToHtml() => $"<div class=\"chitsuki_{Length}\" style=\"text-align:right; margin-right: {Length}em\">";
}

/// <summary>
/// 濁点つきカタカナ用
/// </summary>
public class DakutenKatakana : Inline, IHtmlProvider
{
    public DakutenKatakana(int num, string katakana, string gaiji_dir)
    {
        this.N = num;
        this.Katakana = katakana ?? throw new ArgumentNullException(nameof(katakana));
        this.GaijiDir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public int N { get; }
    public string Katakana { get; }
    public string GaijiDir { get; }

    public override CharType CharType => CharType.Katankana;

    //using StringRefinements

    public string ToHtml() =>
        //kurema:これはU+FF9Eで実現できるのでは？
        //kurema:まぁでもsjis変換で壊れるようなことはしない方が良いか。
        $"<img src=\"{GaijiDir}/1-07/1-07-8{N}.png\" alt=\"※(濁点付き片仮名「{Katakana}」、1-07-8{N})\" class=\"gaiji\" />";
}

/// <summary>
/// 装飾用
/// </summary>
public class Decorate : ReferenceMentioned, IHtmlProvider
{
    public string Close { get; }
    public string Open { get; }

    public Decorate(object? target, string html_class, string html_tag, string? openOverride = null, string? closeOverride = null) : base(target)
    {
        if (html_tag is null) throw new ArgumentNullException(nameof(html_tag));
        if (html_class is null) throw new ArgumentNullException(nameof(html_class));
        this.Close = openOverride ?? $"</{html_tag}>";
        this.Open = closeOverride ?? $"<{html_tag} class=\"{html_class}\">";
    }

    public string ToHtml() => Open + (TargetHtml) + Close;

    public override object Clone()
    {
        return new Decorate(Target, "", "", Open, Close);
    }
}

/// <summary>
/// 書字方向（LTR）の指定用
/// </summary>
public class Dir : ReferenceMentioned, IHtmlProvider
{
    public Dir(object? target) : base(target)
    {
    }

    public string ToHtml() => $"<span dir=\"ltr\">{TargetHtml}</span>";

    public override object Clone()
    {
        return new Dir(Target);
    }
}

/// <summary>
/// 編集者による訂正用
/// </summary>
public class EditorNote : Inline, IHtmlProvider
{
    public string Desc { get; }

    //kurema:元はAozora2Html型のparserも引数に取っていたけど使わないっぽい。
    public EditorNote(string desc) : base()
    {
        this.Desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    public string ToHtml() => $"<span class=\"notes\">［＃{Desc}］</span>";
}

/// <summary>
/// フォントサイズ指定用 
/// </summary>
public class FontSize : Block, IHtmlProvider, IMultiline
{
    public FontSize(INewMidashiIdProvider parser, int times, Aozora2Html.IndentTypeKey daisho) : base(parser)
    {
        Class = daisho.ToString() + times.ToString();
        Style = Utils.CreateFontSize(times, daisho);
    }

    //kurema:下のMultilineは空なので無視で良さげ。
    //include Aozora2Html::Tag::Multiline

    public string Class { get; }
    public string Style { get; }

    public string ToHtml() => $"<div class=\"{Class}\" style=\"font-size: {Style};\">";
}

/// <summary>
/// 画像用
/// </summary>
public class Img : Inline, IHtmlProvider
{
    public Img(string filename, string css_class, string alt, string width, string height)
    {
        this.Filename = filename ?? throw new ArgumentNullException(nameof(filename));
        this.CssClass = css_class ?? throw new ArgumentNullException(nameof(css_class));
        this.Alt = alt ?? throw new ArgumentNullException(nameof(alt));
        this.Width = width ?? throw new ArgumentNullException(nameof(width));
        this.Height = height ?? throw new ArgumentNullException(nameof(height));
    }

    public string Filename { get; }
    public string CssClass { get; }
    public string Alt { get; }
    //kurema: widthはdoubleかなintかなと思ったけど、pxとか付くかもしれないと思ってstringにしました。
    public string Width { get; }
    public string Height { get; }

    public string ToHtml() => $"<img class=\"{CssClass}\" width=\"{Width}\" height=\"{Height}\" src=\"{Filename}\" alt=\"{Alt}\" />";
}

/// <summary>
/// 字下げ用
/// </summary>
public class Jisage : Indent, IHtmlProvider
{
    public int Width { get; }

    public Jisage(INewMidashiIdProvider parser, int width) : base(parser)
    {
        this.Width = width;
    }

    public string ToHtml() => $"<div class=\"jisage_{Width}\" style=\"margin-left: {Width}em\">";
}

/// <summary>
/// 字詰め用
/// </summary>
public class Jizume : Indent, IHtmlProvider, IMultiline
{
    //kurema: 元はw。wだと分からないのでwidthにした。
    public int Width { get; }

    public Jizume(INewMidashiIdProvider parser, int width) : base(parser)
    {
        this.Width = width;
    }

    public string ToHtml() => $"<div class=\"jizume_{Width}\" style=\"width: {Width}em\">";
}

/// <summary>
/// 訓点用
/// </summary>
public class Kunten : Inline
{
    //kurema: 次行のコメント"just remove this line"はよく分からない。
    public override CharType CharType => CharType.Else; //just remove this line
}

/// <summary>
/// 返り点用
/// </summary>
public class Kaeriten : Kunten, IHtmlProvider
{
    public Kaeriten(string @string)
    {
        this.String = @string ?? throw new ArgumentNullException(nameof(@string));
    }

    public string String { get; }

    public string ToHtml() => $"<sub class=\"kaeriten\">{String}</sub>";
}

/// <summary>
/// 罫囲み用
/// </summary>
public class Keigakomi : Block, IHtmlProvider, IMultiline
{
    //kurema: intの方が正しい？
    public double Size { get; }

    public Keigakomi(INewMidashiIdProvider parser, double size = 1) : base(parser)
    {
        this.Size = size;
    }

    public string ToHtml() => $"<div class=\"keigakomi\" style=\"border: solid {Size}px\">";
}

/// <summary>
/// 見出し用
/// </summary>
public class Midashi : ReferenceMentioned, IHtmlProvider
{
    public string Tag { get; }
    public int Id { get; }
    public string Class { get; }

    public Midashi(INewMidashiIdProvider parser, object? target, string size, Utils.MidashiType type) : base(target)
    {
        Tag = Utils.CreateMidashiTag(size);
        Id = parser.GenerateNewMidashiId(size);
        Class = Utils.CreateMidashiClass(type, Tag);
    }

    public Midashi(object? target, string tag, int id, string @class) : base(target)
    {
        this.Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        this.Id = id;
        this.Class = @class ?? throw new ArgumentNullException(nameof(@class));
    }

    public string ToHtml()
    {
        return $"<{Tag} class=\"{Class}\"><a class=\"midashi_anchor\" id=\"midashi{Id}\">{TargetHtml}</a></{Tag}>";
    }

    public override object Clone()
    {
        return new Midashi(Target, Tag, Id, Class);
    }
}

/// <summary>
/// 訓点送り仮名用
/// </summary>
public class Okurigana : Kunten, IHtmlProvider
{
    public Okurigana(string @string)
    {
        this.String = @string ?? throw new ArgumentNullException(nameof(@string));
    }

    public string String { get; }

    public string ToHtml() => $"<sup class=\"okurigana\">{String}</sup>";
}
