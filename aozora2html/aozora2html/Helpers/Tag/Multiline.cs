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
    public MultilineCaption(INewMidashiIdProvider parser) : base(parser)
    {
    }

    public string ToHtml() => "<div class=\"caption\">";
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
    public string Tag { get; }
    public int Id { get; }
    public string Class { get; }

    public MultilineMidashi(INewMidashiIdProvider parser, string size, Utils.MidashiType type) : base(parser)
    {
        Tag = Utils.CreateMidashiTag(size);
        Id = parser.GenerateNewMidashiId(size);
        Class = Utils.CreateMidashiClass(type, Tag);
    }

    public string ToHtml() => $"<{Tag} class=\"{Class}\"><a class=\"midashi_anchor\" id=\"midashi{Id}\">";

    public override string CloseTag() => $"</a></{Tag}>";
}

/// <summary>
/// ブロックでのスタイル指定用
/// </summary>
public class MultilineStyle : Block, IHtmlProvider, IMultiline
{
    public string Style { get; }

    public MultilineStyle(INewMidashiIdProvider parser, string style) : base(parser)
    {
        this.Style = style;
    }

    public string ToHtml() => $"<div class=\"{Style}\">";
}

/// <summary>
/// ブロックでの横組指定用
/// </summary>
public class MultilineYokogumi : Block, IHtmlProvider, IMultiline
{
    public MultilineYokogumi(INewMidashiIdProvider parser) : base(parser)
    {
    }

    public string ToHtml() => "<div class=\"yokogumi\">";
}