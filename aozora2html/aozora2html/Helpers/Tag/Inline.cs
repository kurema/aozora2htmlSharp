using System;

namespace Aozora.Helpers.Tag;

/// <summary>
/// インラインタグ用class
/// 
/// 全ての青空記法はHTML elementに変換される
/// したがって、block/inlineの区別がある
/// 全ての末端青空classはどちらかのclassのサブクラスになる必要がある
/// </summary>
public class Inline : Tag { }

/// <summary>
/// インラインキャプション
/// </summary>
public class InlineCaption : ReferenceMentioned, IHtmlProvider
{
    public InlineCaption(object? target) : base(target) { }

    public string ToHtml() => $"<span class=\"caption\">{TargetHtml}</span>";

    public override object Clone()
    {
        return new InlineCaption(Target);
    }
}

/// <summary>
/// インラインフォントサイズ指定用
/// </summary>
public class InlineFontSize : ReferenceMentioned, IHtmlProvider
{
    public string Class { get; }
    public string Style { get; }

    public InlineFontSize(object? target, int times, Aozora2Html.IndentTypeKey daisho) : base(target)
    {
        Class = daisho.ToString() + times.ToString();
        Style = Utils.CreateFontSize(times, daisho);
    }

    public InlineFontSize(object? target, string @class, string style) : base(target)
    {
        this.Class = @class ?? throw new ArgumentNullException(nameof(@class));
        this.Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public string ToHtml() => $"<span class=\"{Class}\" style=\"font-size: {Style};\">{TargetHtml}</span>";

    public override object Clone()
    {
        return new InlineFontSize(Target, Class, Style);
    }
}

/// <summary>
/// インライン罫囲み用
/// </summary>
public class InlineKeigakomi : ReferenceMentioned, IHtmlProvider
{
    public InlineKeigakomi(object? target) : base(target)
    {
    }

    public string ToHtml() => $"<span class=\"keigakomi\">{TargetHtml}</span>";

    public override object Clone()
    {
        return new InlineKeigakomi(Target);
    }
}

/// <summary>
/// インライン横組み用
/// </summary>
public class InlineYokogumi : ReferenceMentioned, IHtmlProvider
{
    public InlineYokogumi(object? target) : base(target)
    {
    }

    public string ToHtml() => $"<span class=\"yokogumi\">{TargetHtml}</span>";

    public override object Clone()
    {
        return new InlineYokogumi(Target);
    }
}

