using System.Text;

namespace Aozora.Helpers.Tag;

/*
    # 参照先用
    #
    # 前方参照でこいつだけは中身をチェックする
    # 子要素を持つAozora2Html::Tag::Inlineは全てこいつのサブクラス
*/
public class ReferenceMentioned : Inline
{
    //rubocop:disable Lint/MissingSuper
    public ReferenceMentioned(object? target)
    {
        if (target is null || block_element(target)) return;
        syntax_error();
    }

    public object? target { get; }

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
                                default: sb.Append(item.ToString() ?? "");
                                    break;
                            }
                        }
                        return sb.ToString();
                    }
                default:return target?.ToString() ?? "";
            }
        }
    }
}