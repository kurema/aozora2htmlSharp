using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers.Tag;

//kurema:moduleをclassにincludeする意味はよく分からないが、多分識別用だと思うのでinterfaceで代用。
/// <summary>
/// 複数行記法用
/// </summary>
public interface IMultiline
{
}

/// <summary>
/// 複数行キャプション用
/// </summary>
public class MultilineCaption : Block, IHtmlProvider, IMultiline
{
    public MultilineCaption(Aozora2Html parser) : base(parser)
    {
    }

    public string to_html() => "<div class=\"caption\">";
}

/// <summary>
/// ブロックでの地付き指定用
/// </summary>
public class MultilineChitsuki : Chitsuki, IMultiline
{
    public MultilineChitsuki(Aozora2Html parser, int length) : base(parser, length)
    {
    }
}

/// <summary>
/// ブロックでの字下げ指定用
/// </summary>
public class MultilineJisage : Jisage, IMultiline
{
    public MultilineJisage(Aozora2Html parser, int width) : base(parser, width)
    {
    }
}

/// <summary>
/// ブロックでの見出し指定用
/// </summary>
public class MultilineMidashi : Block, IHtmlProvider, IMultiline
{
    public string tag { get; }
    public int id { get; }
    public string @class { get; }

    public MultilineMidashi(Aozora2Html parser, char size, Utils.MidashiType type) : base(parser)
    {
        tag = Utils.create_midashi_tag(size);
        id = parser.new_midashi_id(size);
        @class = Utils.create_midashi_class(type, tag);
    }

    public string to_html() => $"<{tag} class=\"{@class}\"><a class=\"midashi_anchor\" id=\"midashi{id}\">";

    public override string close_tag() => $"</a></{tag}>";
}

/// <summary>
/// ブロックでのスタイル指定用
/// </summary>
public class MultilineStyle : Block, IHtmlProvider, IMultiline
{
    public string style { get; }

    public MultilineStyle(Aozora2Html parser, string style) : base(parser)
    {
        this.style = style;
    }

    public string to_html() => $"<div class=\"{style}\">";
}

/// <summary>
/// ブロックでの横組指定用
/// </summary>
public class MultilineYokogumi : Block, IHtmlProvider, IMultiline
{
    public MultilineYokogumi(Aozora2Html parser) : base(parser)
    {
    }

    public string to_html() => "<div class=\"yokogumi\">";
}