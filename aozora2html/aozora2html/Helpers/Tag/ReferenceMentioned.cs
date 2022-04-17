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
        if (target is null || block_element(target)) return;
        syntax_error();
    }

    public object? target { get; set; }

    //kurema: to_s()をto_html()に下関係で、targetが適切に変換されるように以下の関数を追加しました。
    public string target_to_html()
    {
        return (target as IHtmlProvider)?.to_html() ?? target?.ToString() ?? "";
    }

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
                return true;
            default: return elt is ReferenceMentioned;
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
    public string target_string
    {
        get
        {
            switch (target)
            {
                case string text: return text;
                case ReferenceMentioned reference: return reference.target_string;
                case System.Collections.IEnumerable enumerable:
                    {
                        var sb = new StringBuilder();
                        foreach (var item in enumerable)
                        {
                            switch (item)
                            {
                                case ReferenceMentioned reference:
                                    sb.Append(reference.target_string);
                                    break;
                                default:
                                    sb.Append(item.ToString() ?? "");
                                    break;
                            }
                        }
                        return sb.ToString();
                    }
                default: return target?.ToString() ?? "";
            }
        }
    }
}