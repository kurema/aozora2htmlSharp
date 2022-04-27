using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Aozora.Helpers
{
    /// <summary>
    /// ルビ文字列解析用バッファ
    /// </summary>
    public class RubyBuffer : IList<IBufferItem>
    {
        // `｜`が来た時に真にする。ルビの親文字のガード用。
        public bool @protected { get; set; }

        // バッファの初期化。`""`の1要素のバッファにする。
        //kurema: 下のMemeberNotNullはNullable様だけど、.NetStandard 2.0では使えない。
        //[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(ruby_buf))]
        public void clear()
        {
            ruby_buf = new List<IBufferItem>();
            @protected = false;
            char_type = null;
        }

        public List<IBufferItem> ruby_buf { get; private set; } = new List<IBufferItem>();//kurema:この代入は余計。でも楽。
        // @ruby_buf内の文字のchar_type
        public Tag.CharType? char_type = null;

        public RubyBuffer()
        {
            clear();
        }

        public bool empty => ruby_buf.Count == 0;
        public bool present => !empty;

        public IBufferItem[] ToArray() => ruby_buf.ToArray();
        public IBufferItem? last => ruby_buf.Count > 0 ? ruby_buf.Last() : null;

        public int length => ruby_buf.Count;

        public int Count => ((ICollection<IBufferItem>)ruby_buf).Count;

        public bool IsReadOnly => ((ICollection<IBufferItem>)ruby_buf).IsReadOnly;

        public IBufferItem this[int index] { get => ((IList<IBufferItem>)ruby_buf)[index]; set => ((IList<IBufferItem>)ruby_buf)[index] = value; }


        // バッファ末尾にitemを追加する
        //
        // itemとバッファの最後尾がどちらもStringであれば連結したStringにし、
        // そうでなければバッファの末尾に新しい要素として追加する
        public void push(string value)
        {
            if (last is BufferItemString itemString) itemString.Append(value);
            else ruby_buf.Add(new BufferItemString(value));
        }

        public void push(char @char)
        {
            push(new string(@char, 1));
        }

        public void push(Tag.Tag tag)
        {
            ruby_buf.Add(new BufferItemTag(tag));
        }

        public IBufferItem[] create_ruby(string ruby)
        {
            //kurema:
            //`ans = +''` ってのがあるんだけど。`+''`ってのは何？分からん。
            var ans = new StringBuilder();
            var notes = new List<IBufferItem>();

            foreach (var token in ruby_buf)
            {
                if ((token as BufferItemTag)?.tag is Tag.UnEmbedGaiji gaiji)
                {
                    ans.Append(Aozora2Html.GAIJI_MARK);
                    gaiji.escape();
                    notes.Add(token);
                }
                else
                {
                    ans.Append(token.to_html());
                }
            }

            notes.Insert(0, new BufferItemTag(new Tag.Ruby(ans.ToString(), ruby)));
            clear();
            return notes.ToArray();
        }

        //buffer management
        public TextBuffer dump_into(TextBuffer buffer)
        {
            if (@protected)
            {
                ruby_buf.Insert(0, new BufferItemString(new string(Aozora2Html.RUBY_PREFIX, 1)));
                @protected = false;
            }
            var top = ruby_buf.Count > 0 ? ruby_buf[0] : null;
            if (top is BufferItemString && buffer.LastOrDefault() is BufferItemString lastString)
            {
                lastString.Append(top.to_html());
                buffer.AddRange(ruby_buf.GetRange(1, ruby_buf.Count - 1));
            }
            else
            {
                buffer.AddRange(ruby_buf);
            }
            clear();
            return buffer;
        }


        public void push_char(char @char, TextBuffer buffer)
        {
            var ctype = Utils.GetCharType(@char);
            if (ctype == Tag.CharType.HankakuTerminate && char_type == Tag.CharType.Hankaku)
            {
                push(@char);
                char_type = Tag.CharType.Else;
            }
            else if (@protected || ((ctype != Tag.CharType.Else) && (ctype == char_type)))
            {
                push(@char);
            }
            else
            {
                dump_into(buffer);
                push(@char);
                char_type = ctype;
            }
        }

        public void push_char(Tag.Tag @char, TextBuffer buffer)
        {
            var ctype = @char.char_type;
            if (@protected)
            {
                push(@char);
            }
            else
            {
                dump_into(buffer);
                push(@char);
                char_type = ctype;
            }
        }

        #region IList
        public int IndexOf(IBufferItem item)
        {
            return ((IList<IBufferItem>)ruby_buf).IndexOf(item);
        }

        public void Insert(int index, IBufferItem item)
        {
            ((IList<IBufferItem>)ruby_buf).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<IBufferItem>)ruby_buf).RemoveAt(index);
        }

        public void Add(IBufferItem item)
        {
            ((ICollection<IBufferItem>)ruby_buf).Add(item);
        }

        public void Clear()
        {
            ((ICollection<IBufferItem>)ruby_buf).Clear();
        }

        public bool Contains(IBufferItem item)
        {
            return ((ICollection<IBufferItem>)ruby_buf).Contains(item);
        }

        public void CopyTo(IBufferItem[] array, int arrayIndex)
        {
            ((ICollection<IBufferItem>)ruby_buf).CopyTo(array, arrayIndex);
        }

        public bool Remove(IBufferItem item)
        {
            return ((ICollection<IBufferItem>)ruby_buf).Remove(item);
        }

        public IEnumerator<IBufferItem> GetEnumerator()
        {
            return ((IEnumerable<IBufferItem>)ruby_buf).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)ruby_buf).GetEnumerator();
        }
        #endregion

        //kurema:
        //char_type()は省略。元々例外発生しないし。
    }
}

