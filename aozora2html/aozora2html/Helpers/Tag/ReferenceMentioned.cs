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
        this.target = target;
        if (target is null || !block_element(target)) return;
        syntax_error();
    }

    public object? target { get; set; }

    public static bool block_element(object elt)
    {
        switch (elt)
        {
            case string text: return text.Contains("<div");
            //kurema:ここは要修正？
            case System.Collections.IEnumerable enumerable:
                foreach (var item in enumerable)
                {
                    if (block_element(item)) return true;
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
    public string target_html
    {
        get
        {
            switch (target)
            {
                case string text: return text;
                case ReferenceMentioned reference: return reference.target_html;
                case System.Collections.IEnumerable enumerable:
                    {
                        var sb = new StringBuilder();
                        foreach (var item in enumerable)
                        {
                            switch (item)
                            {
                                case ReferenceMentioned reference:
                                    sb.Append(reference.target_html);
                                    break;
                                case IHtmlProvider htmlProvider:
                                    sb.Append(htmlProvider.to_html() ?? "");
                                    break;
                                default:
                                    sb.Append(item?.ToString() ?? "");
                                    break;
                            }
                        }
                        return sb.ToString();
                    }
                case IHtmlProvider html: return html.to_html();
                default: return target?.ToString() ?? "";
            }
        }
    }
}