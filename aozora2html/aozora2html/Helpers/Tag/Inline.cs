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

    public string to_html() => $"<span class=\"caption\">{target_html}</span>";

    public override object Clone()
    {
        return new InlineCaption(target);
    }
}

/// <summary>
/// インラインフォントサイズ指定用
/// </summary>
public class InlineFontSize : ReferenceMentioned, IHtmlProvider
{
    public string @class { get; }
    public string style { get; }

    public InlineFontSize(object? target, int times, Aozora2Html.IndentTypeKey daisho) : base(target)
    {
        @class = daisho.ToString() + times.ToString();
        style = Utils.create_font_size(times, daisho);
    }

    public InlineFontSize(object? target, string @class, string style) : base(target)
    {
        this.@class = @class ?? throw new ArgumentNullException(nameof(@class));
        this.style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public string to_html() => $"<span class=\"{@class}\" style=\"font-size: {style};\">{target}</span>";

    public override object Clone()
    {
        return new InlineFontSize(target, @class, style);
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

    public string to_html() => $"<span class=\"keigakomi\">{target_html}</span>";

    public override object Clone()
    {
        return new InlineKeigakomi(target);
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

    public string to_html() => $"<span class=\"yokogumi\">{target}</span>";

    public override object Clone()
    {
        return new InlineYokogumi(target);
    }
}

