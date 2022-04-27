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
    public string close_tag();
}

public interface ICharTypeProvider
{
    public CharType char_type { get; }
}

public interface IHtmlProvider
{
    public string to_html();

}

/// <summary>
/// 変換される青空記法class
/// </summary>
public class Tag : ICharTypeProvider
{
    //debug用
    public string inspect() { return GetHtml(this); }

    public virtual CharType char_type => CharType.Else;

    public void syntax_error()
    {
        throw new Exceptions.TagSyntaxException();
    }

    public static string GetHtml(Tag tag)
    {
        if (tag is IHtmlProvider provider) return provider.to_html();
        else return tag.ToString();
    }
}

/// <summary>
/// ブロックタグ用class
///
/// 各Tagクラスはこれを継承する
/// </summary>
public class Block : Tag, IClosable
{
    public Block(Aozora2Html parser) : base()
    {
        if (!parser.block_allowed_context) syntax_error();
    }

    //必要に基づきmethod overrideする
    public virtual string close_tag() => "</div>";
}

public class Indent : Block
{
    public Indent(Aozora2Html parser) : base(parser)
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
    public int length { get; }

    public Chitsuki(Aozora2Html parser, int length) : base(parser)
    {
        this.length = length;
    }

    public string to_html() => $"<div class=\"chitsuki_{length}\" style=\"text-align:right; margin-right: {length}em\">";
}

/// <summary>
/// 濁点つきカタカナ用
/// </summary>
public class DakutenKatakana : Inline, IHtmlProvider
{
    public DakutenKatakana(int num, string katakana, string gaiji_dir)
    {
        this.n = num;
        this.katakana = katakana ?? throw new ArgumentNullException(nameof(katakana));
        this.gaiji_dir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public int n { get; }
    public string katakana { get; }
    public string gaiji_dir { get; }

    public override CharType char_type => CharType.Katankana;

    //using StringRefinements

    public string to_html() =>
        //kurema:これはU+FF9Eで実現できるのでは？
        //kurema:まぁでもsjis変換で壊れるようなことはしない方が良いか。
        $"<img src=\"{gaiji_dir}/1-07/1-07-8{n}.png\" alt=\"※(濁点付き片仮名「{katakana}」、1-07-8{n})\" class=\"gaiji\" />";
}

/// <summary>
/// 装飾用
/// </summary>
public class Decorate : ReferenceMentioned, IHtmlProvider
{
    public string close { get; }
    public string open { get; }

    public Decorate(object? target, string html_class, string html_tag, string? openOverride = null, string? closeOverride = null) : base(target)
    {
        if (html_tag is null) throw new ArgumentNullException(nameof(html_tag));
        if (html_class is null) throw new ArgumentNullException(nameof(html_class));
        this.close = openOverride ?? $"</{html_tag}>";
        this.open = closeOverride ?? $"<{html_tag} class=\"{html_class}\">";
    }

    public string to_html() => open + (target_html) + close;

    public override object Clone()
    {
        return new Decorate(target, "", "", open, close);
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

    public string to_html() => $"<span dir=\"ltr\">{target_html}</span>";

    public override object Clone()
    {
        return new Dir(target);
    }
}

/// <summary>
/// 編集者による訂正用
/// </summary>
public class EditorNote : Inline, IHtmlProvider
{
    public string desc { get; }

    //kurema:元はAozora2Html型のparserも引数に取っていたけど使わないっぽい。
    public EditorNote(string desc) : base()
    {
        this.desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    public string to_html() => $"<span class=\"notes\">［＃{desc}］</span>";
}

/// <summary>
/// フォントサイズ指定用 
/// </summary>
public class FontSize : Block, IHtmlProvider, IMultiline
{
    public FontSize(Aozora2Html parser, int times, Aozora2Html.IndentTypeKey daisho) : base(parser)
    {
        @class = daisho.ToString() + times.ToString();
        style = Utils.create_font_size(times, daisho);
    }

    //kurema:下のMultilineは空なので無視で良さげ。
    //include Aozora2Html::Tag::Multiline

    public string @class { get; }
    public string style { get; }

    public string to_html() => $"<div class=\"{@class}\" style=\"font-size: {style};\">";
}

/// <summary>
/// 画像用
/// </summary>
public class Img : Inline, IHtmlProvider
{
    public Img(string filename, string css_class, string alt, string width, string height)
    {
        this.filename = filename ?? throw new ArgumentNullException(nameof(filename));
        this.css_class = css_class ?? throw new ArgumentNullException(nameof(css_class));
        this.alt = alt ?? throw new ArgumentNullException(nameof(alt));
        this.width = width ?? throw new ArgumentNullException(nameof(width));
        this.height = height ?? throw new ArgumentNullException(nameof(height));
    }

    public string filename { get; }
    public string css_class { get; }
    public string alt { get; }
    //kurema: widthはdoubleかなintかなと思ったけど、pxとか付くかもしれないと思ってstringにしました。
    public string width { get; }
    public string height { get; }

    public string to_html() => $"<img class=\"{css_class}\" width=\"{width}\" height=\"{height}\" src=\"{filename}\" alt=\"{alt}\" />";
}

/// <summary>
/// 字下げ用
/// </summary>
public class Jisage : Indent, IHtmlProvider
{
    public int width { get; }

    public Jisage(Aozora2Html parser, int width) : base(parser)
    {
        this.width = width;
    }

    public string to_html() => $"<div class=\"jisage_{width}\" style=\"margin-left: {width}em\">";
}

/// <summary>
/// 字詰め用
/// </summary>
public class Jizume : Indent, IHtmlProvider, IMultiline
{
    //kurema: 元はw。wだと分からないのでwidthにした。
    public int width { get; }

    public Jizume(Aozora2Html parser, int width) : base(parser)
    {
        this.width = width;
    }

    public string to_html() => $"<div class=\"jizume_{width}\" style=\"width: {width}em\">";
}

/// <summary>
/// 訓点用
/// </summary>
public class Kunten : Inline
{
    //kurema: 次行のコメント"just remove this line"はよく分からない。
    public override CharType char_type => CharType.Else; //just remove this line
}

/// <summary>
/// 返り点用
/// </summary>
public class Kaeriten : Kunten, IHtmlProvider
{
    public Kaeriten(string @string)
    {
        this.@string = @string ?? throw new ArgumentNullException(nameof(@string));
    }

    public string @string { get; }

    public string to_html() => $"<sub class=\"kaeriten\">{@string}</sub>";
}

/// <summary>
/// 罫囲み用
/// </summary>
public class Keigakomi : Block, IHtmlProvider
{
    //kurema: intの方が正しい？
    public double size { get; }

    public Keigakomi(Aozora2Html parser, double size = 1) : base(parser)
    {
        this.size = size;
    }

    public string to_html() => $"<div class=\"keigakomi\" style=\"border: solid {size}px\">";
}

/// <summary>
/// 見出し用
/// </summary>
public class Midashi : ReferenceMentioned, IHtmlProvider
{
    public string tag { get; }
    public int id { get; }
    public string @class { get; }

    public Midashi(Aozora2Html parser, object? target, string size, Utils.MidashiType type) : base(target)
    {
        tag = Utils.create_midashi_tag(size);
        id = parser.new_midashi_id(size);
        @class = Utils.create_midashi_class(type, tag);
    }

    public Midashi(object? target, string tag, int id, string @class) : base(target)
    {
        this.tag = tag ?? throw new ArgumentNullException(nameof(tag));
        this.id = id;
        this.@class = @class ?? throw new ArgumentNullException(nameof(@class));
    }

    public string to_html()
    {
        return $"<{tag} class=\"{@class}\"><a class=\"midashi_anchor\" id=\"midashi{id}\">{target}</a></{tag}>";
    }

    public override object Clone()
    {
        return new Midashi(target, tag, id, @class);
    }
}

/// <summary>
/// 訓点送り仮名用
/// </summary>
public class Okurigana : Kunten, IHtmlProvider
{
    public Okurigana(string @string)
    {
        this.@string = @string ?? throw new ArgumentNullException(nameof(@string));
    }

    public string @string { get; }

    public string to_html() => $"<sup class=\"okurigana\">{@string}</sup>";
}
