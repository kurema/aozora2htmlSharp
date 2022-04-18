using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    /// <summary>
    /// ルビ文字列解析用バッファ
    /// </summary>
    public class RubyBuffer
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
            push(@char.ToString());
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
                ruby_buf.Insert(0, new BufferItemString(Aozora2Html.RUBY_PREFIX.ToString()));
                @protected = false;
            }
            var top = ruby_buf[0];
            if(top is BufferItemString && buffer.Last() is BufferItemString lastString)
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


        //kurema:第二引数は未実装のTextBuffer
        public void push_char(char @char,TextBuffer buffer)
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

        //kurema:
        //char_type()は省略。元々例外発生しないし。
    }
}

