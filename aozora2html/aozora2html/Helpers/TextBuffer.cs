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
        public TextBuffer()
        {
        }

        public TextBuffer(IEnumerable<IBufferItem> collection) : base(collection)
        {
        }

        public TextBuffer(string text) : base(new IBufferItem[] { new BufferItemString(text) })
        {
        }

        public string ToHtml()
        {
            var result = new StringBuilder();
            foreach (var item in this)
            {
                result.Append(item.ToHtml());
            }
            return result.ToString();
        }

        public enum BlankTypeResult
        {
            @true, @false, inline
        }

        /// <summary>
        /// 行出力時に@bufferが空かどうか調べる
        /// 
        /// @bufferの中身によって行末の出力が異なるため
        /// </summary>
        /// <returns>[true, false, :inline] 空文字ではない文字列が入っていればfalse、1行注記なら:inline、それ以外しか入っていなければtrue</returns>
        public BlankTypeResult BlankType()
        {
            foreach (var token in this)
            {
                if (token is BufferItemString text && text.Length > 0) return BlankTypeResult.@false;
                if (token is BufferItemTag tag && tag.Content is Tag.IOnelineIndent) return BlankTypeResult.inline;
            }
            return BlankTypeResult.@true;
        }

        /// <summary>
        /// 行末で&lt;br /&gt;を出力するべきかどうかの判別用
        /// </summary>
        /// <returns>[true, false] Multilineの注記しか入っていなければfalse、Multilineでも空文字でもない要素が含まれていればtrue</returns>
        public bool Terpri()
        {
            //kurema: terpriは"TERminate PRInt line"を意味するLispの関数だそう。要するにConsole.WriteLine();
            bool flag = true;
            foreach (var x in this)
            {
                switch (x)
                {
                    case BufferItemTag tag when tag.Content is Tag.IMultiline:
                        flag = false;
                        break;
                    case BufferItemString @string when @string.Length == 0: break;
                    default: return true;
                }
            }
            return flag;
        }
    }
}
