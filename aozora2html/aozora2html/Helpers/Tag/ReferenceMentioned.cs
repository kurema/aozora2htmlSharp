using System.Text;

namespace Aozora.Helpers.Tag;

/*
    # 参照先用
    #
    # 前方参照でこいつだけは中身をチェックする
    # 子要素を持つAozora2Html::Tag::Inlineは全てこいつのサブクラス
*/
public abstract class ReferenceMentioned : Inline, System.ICloneable
{
    //rubocop:disable Lint/MissingSuper
    public ReferenceMentioned(object? target)
    {
        this.Target = target;
        if (target is null || !BlockElement(target)) return;
        ThrowSyntaxError();
    }

    public object? Target { get; set; }

    public static bool BlockElement(object elt)
    {
        switch (elt)
        {
            case string text: return text.Contains("<div");
            //kurema:ここは要修正？
            case System.Collections.IEnumerable enumerable:
                foreach (var item in enumerable)
                {
                    if (BlockElement(item)) return true;
                }
                return false;
            default: return elt is Block;
        }
    }

    //kurema:
    //~~Ruby.csでdup(clone()相当)が使われているので実装。ただし、targetがクローン可能な場合のみ。大丈夫かコレ？~~
    public abstract object Clone();
    //{
    //    var tmp = target is System.ICloneable original ? original.Clone() : target;
    //    return new ReferenceMentioned(tmp);
    //}

    //kurema:挙動に自信がない。
    public string TargetHtml
    {
        get
        {
            switch (Target)
            {
                case string text: return text;
                case ReferenceMentioned reference: return reference.TargetHtml;
                case System.Collections.IEnumerable enumerable:
                    {
                        var sb = new StringBuilder();
                        foreach (var item in enumerable)
                        {
                            switch (item)
                            {
                                case ReferenceMentioned reference:
                                    sb.Append(reference.TargetHtml);
                                    break;
                                case IHtmlProvider htmlProvider:
                                    sb.Append(htmlProvider.ToHtml() ?? "");
                                    break;
                                default:
                                    sb.Append(item?.ToString() ?? "");
                                    break;
                            }
                        }
                        return sb.ToString();
                    }
                case IHtmlProvider html: return html.ToHtml();
                default: return Target?.ToString() ?? "";
            }
        }
    }
}