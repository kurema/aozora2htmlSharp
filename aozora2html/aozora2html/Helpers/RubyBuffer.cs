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
        public bool IsProtected { get; set; }

        // バッファの初期化。`""`の1要素のバッファにする。
        //kurema: 下のMemeberNotNullはNullable様だけど、.NetStandard 2.0では使えない。
        //[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(ruby_buf))]
        public void Clear()
        {
            RubyBuf = new List<IBufferItem>();
            IsProtected = false;
            char_type = null;
        }

        public List<IBufferItem> RubyBuf { get; private set; } = new List<IBufferItem>();//kurema:この代入は余計。でも楽。
        // @ruby_buf内の文字のchar_type
        public Tag.CharType? char_type = null;

        public RubyBuffer()
        {
            Clear();
        }

        public bool IsEmpty => RubyBuf.Count == 0;
        public bool IsPresent => !IsEmpty;

        public IBufferItem[] ToArray() => RubyBuf.ToArray();
        public IBufferItem? Last => RubyBuf.Count > 0 ? RubyBuf.Last() : null;

        public int Length => RubyBuf.Count;

        public int Count => ((ICollection<IBufferItem>)RubyBuf).Count;

        public bool IsReadOnly => ((ICollection<IBufferItem>)RubyBuf).IsReadOnly;

        public IBufferItem this[int index] { get => ((IList<IBufferItem>)RubyBuf)[index]; set => ((IList<IBufferItem>)RubyBuf)[index] = value; }


        // バッファ末尾にitemを追加する
        //
        // itemとバッファの最後尾がどちらもStringであれば連結したStringにし、
        // そうでなければバッファの末尾に新しい要素として追加する
        public void Push(string value)
        {
            if (Last is BufferItemString itemString) itemString.Append(value);
            else RubyBuf.Add(new BufferItemString(value));
        }

        public void Push(char @char)
        {
            Push(new string(@char, 1));
        }

        public void Push(Tag.Tag tag)
        {
            RubyBuf.Add(new BufferItemTag(tag));
        }

        public IBufferItem[] CreateRuby(string ruby)
        {
            //kurema:
            //`ans = +''` ってのがあるんだけど。`+''`ってのは何？分からん。
            var ans = new StringBuilder();
            var notes = new List<IBufferItem>();

            foreach (var token in RubyBuf)
            {
                if ((token as BufferItemTag)?.Content is Tag.UnEmbedGaiji gaiji)
                {
                    ans.Append(Aozora2Html.GAIJI_MARK);
                    gaiji.Escape();
                    notes.Add(token);
                }
                else
                {
                    ans.Append(token.ToHtml());
                }
            }

            notes.Insert(0, new BufferItemTag(new Tag.Ruby(ans.ToString(), ruby)));
            Clear();
            return notes.ToArray();
        }

        //buffer management
        public TextBuffer DumpInto(TextBuffer buffer)
        {
            if (IsProtected)
            {
                RubyBuf.Insert(0, new BufferItemString(new string(Aozora2Html.RUBY_PREFIX, 1)));
                IsProtected = false;
            }
            var top = RubyBuf.Count > 0 ? RubyBuf[0] : null;
            if (top is BufferItemString && buffer.LastOrDefault() is BufferItemString lastString)
            {
                lastString.Append(top.ToHtml());
                buffer.AddRange(RubyBuf.GetRange(1, RubyBuf.Count - 1));
            }
            else
            {
                buffer.AddRange(RubyBuf);
            }
            Clear();
            return buffer;
        }


        public void PushChar(char @char, TextBuffer buffer)
        {
            var ctype = Utils.GetCharType(@char);
            if (ctype == Tag.CharType.HankakuTerminate && char_type == Tag.CharType.Hankaku)
            {
                Push(@char);
                char_type = Tag.CharType.Else;
            }
            else if (IsProtected || ((ctype != Tag.CharType.Else) && (ctype == char_type)))
            {
                Push(@char);
            }
            else
            {
                DumpInto(buffer);
                Push(@char);
                char_type = ctype;
            }
        }

        public void PushChar(Tag.Tag @char, TextBuffer buffer)
        {
            var ctype = @char.CharType;
            if (IsProtected)
            {
                Push(@char);
            }
            else
            {
                DumpInto(buffer);
                Push(@char);
                char_type = ctype;
            }
        }

        #region IList
        public int IndexOf(IBufferItem item)
        {
            return ((IList<IBufferItem>)RubyBuf).IndexOf(item);
        }

        public void Insert(int index, IBufferItem item)
        {
            ((IList<IBufferItem>)RubyBuf).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<IBufferItem>)RubyBuf).RemoveAt(index);
        }

        public void Add(IBufferItem item)
        {
            ((ICollection<IBufferItem>)RubyBuf).Add(item);
        }

        //public void Clear()
        //{
        //    ((ICollection<IBufferItem>)RubyBuf).Clear();
        //}

        public bool Contains(IBufferItem item)
        {
            return ((ICollection<IBufferItem>)RubyBuf).Contains(item);
        }

        public void CopyTo(IBufferItem[] array, int arrayIndex)
        {
            ((ICollection<IBufferItem>)RubyBuf).CopyTo(array, arrayIndex);
        }

        public bool Remove(IBufferItem item)
        {
            return ((ICollection<IBufferItem>)RubyBuf).Remove(item);
        }

        public IEnumerator<IBufferItem> GetEnumerator()
        {
            return ((IEnumerable<IBufferItem>)RubyBuf).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)RubyBuf).GetEnumerator();
        }
        #endregion

        //kurema:
        //char_type()は省略。元々例外発生しないし。
    }
}

