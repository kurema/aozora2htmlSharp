using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    /// <summary>
    /// 本文テキスト用バッファ
    /// 
    /// 要素はString以外も含む
    /// to_html()すると文字列化できる
    /// </summary>
    public class TextBuffer : List<IBufferItem>, Tag.IHtmlProvider
    {
        public string to_html()
        {
            var result = new StringBuilder();
            foreach(var item in this)
            {
                result.Append(item.to_html());
            }
            return result.ToString();
        }

        public enum blank_type_result
        {
            @true, @false, inline
        }

        /// <summary>
        /// 行出力時に@bufferが空かどうか調べる
        /// 
        /// @bufferの中身によって行末の出力が異なるため
        /// </summary>
        /// <returns>[true, false, :inline] 空文字ではない文字列が入っていればfalse、1行注記なら:inline、それ以外しか入っていなければtrue</returns>
        public blank_type_result blank_type()
        {
            foreach (var token in this)
            {
                if (token is BufferItemString text && text.Length > 0) return blank_type_result.@false;
                if (token is BufferItemTag tag && tag.tag is Tag.IOnelineIndent) return blank_type_result.inline;
            }
            return blank_type_result.@true;
        }

        /// <summary>
        /// 行末で<br />を出力するべきかどうかの判別用
        /// </summary>
        /// <returns>[true, false] Multilineの注記しか入っていなければfalse、Multilineでも空文字でもない要素が含まれていればtrue</returns>
        public bool terpri()
        {
            bool flag = true;
            foreach(var x in this)
            {
                switch (x)
                {
                    case BufferItemTag tag when tag.tag is Tag.IMultiline:
                        flag = false;
                        break;
                    case BufferItemString @string when @string.Length == 0:break;
                    default:return true;
                }
            }
            return flag;
        }
    }
}
