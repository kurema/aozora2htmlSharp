using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public class RubyBuffer
    {
        // `｜`が来た時に真にする。ルビの親文字のガード用。
        public bool @protected { get; set; }

        //[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(ruby_buf))]
        public void clear()
        {
            ruby_buf = new List<IRubyBufferItem>();
            @protected = false;
            char_type = null;
        }

        public List<IRubyBufferItem> ruby_buf { get; private set; } = new List<IRubyBufferItem>();
        public Tag.CharType? char_type = null;

        public RubyBuffer()
        {
            clear();
        }

        public bool empty => ruby_buf.Count == 0;
        public bool present => !empty;

        public IRubyBufferItem[] ToArray() => ruby_buf.ToArray();
        public IRubyBufferItem? last => ruby_buf.Count > 0 ? ruby_buf.Last() : null;

        public int length => ruby_buf.Count;


        // バッファ末尾にitemを追加する
        //
        // itemとバッファの最後尾がどちらもStringであれば連結したStringにし、
        // そうでなければバッファの末尾に新しい要素として追加する
        public void push(string value)
        {
            if (last is RubyBufferItemString itemString) itemString.Append(value);
            else ruby_buf.Add(new RubyBufferItemString(value));
        }

        public void push(Tag.Tag tag)
        {
            ruby_buf.Add(new RubyBufferItemTag(tag));
        }

        public IRubyBufferItem[] create_ruby(Aozora2Html parser, string ruby)
        {
            var ans = new StringBuilder();
            var notes = new List<IRubyBufferItem>();

            foreach (var token in ruby_buf)
            {
                if ((token as RubyBufferItemTag)?.tag is Tag.UnEmbedGaiji gaiji)
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

            notes.Insert(0, new RubyBufferItemTag(new Tag.Ruby(ans.ToString(), ruby)));
            clear();
            return notes.ToArray();
        }

        //kurema:第二引数は未実装のTextBuffer
        public void push_char(char @char)
        {
            throw new NotImplementedException();
        }

        public void dump_into()
        {
            throw new NullReferenceException();
        }

        public interface IRubyBufferItem : Tag.IHtmlProvider
        {
        }

        public class RubyBufferItemTag : IRubyBufferItem
        {
            public Tag.Tag tag { get; }

            public RubyBufferItemTag(Tag.Tag tag)
            {
                this.tag = tag;
            }

            public string to_html()
            {
                return Tag.Tag.GetHtml(tag);
            }
        }

        public class RubyBufferItemString : IRubyBufferItem
        {

            public RubyBufferItemString(string text)
            {
                buffer = new StringBuilder(text);
            }

            public StringBuilder buffer { get; }

            public void Append(string value)
            {
                buffer.Append(value);
            }

            public string to_html()
            {
                return buffer.ToString();
            }
        }
    }


    public class RubyBuffer_old
    {
        //kurema:
        // 中身ほぼStringBuilderまんま。
        // rubyではcharとstring入り混じったListを使ってるみたいだが、dotnetではそんな必要ない。
        // 全体的に何やってるか分かり辛かった。

        //kurema:
        // 色々おかしいので要書き直し。
        // ruby_bufはcreate_rubyを見る限りTag.UnEmbedGaijiとか入るし判定が必要なので、StringBuilderじゃ上手くいかない。
        // あとto_html()関係で色々おかしくなってる。

        //kurema:
        // コード参考用に新RubyBufferを実装完了するまでは放置します。

        private StringBuilder ruby_buf;
        public bool @protected { get; set; }
        public Tag.CharType? char_type { get; set; }


        // `｜`が来た時に真にする。ルビの親文字のガード用。
        //attr_accessor :protected

        // @ruby_buf内の文字のchar_type
        //attr_accessor :char_type


#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public RubyBuffer_old(string? item = null)
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
            //ruby_buf = new StringBuilder();
            clear(item);
        }

        // バッファの初期化。引数itemがあるときはその1要素のバッファに、
        // 引数がなければ`""`の1要素のバッファにする。
        //[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(ruby_buf))]
        public void clear(string? item = null)
        {
            ruby_buf = new StringBuilder(item ?? "");
            @protected = false;
            char_type = null;
        }

        public bool empty()
        {
            return ruby_buf.Length == 0;
        }

        public bool present()
        {
            return !empty();
        }

        public string to_a()
        {
            return ruby_buf.ToString();
        }

        public void each(Action<char> block)
        {
            foreach (var item in ruby_buf.ToString())
            {
                block(item);
            }
        }

        public void push(string item)
        {
            ruby_buf.Append(item);
        }

        public int length()
        {
            return ruby_buf.Length;
        }

        //kurema:StringBuilderなら区別する必要はない。
        public void last_concat(string item)
        {
            push(item);
        }

        // buffer management

        public StringBuilder dump_into(StringBuilder buffer)
        {
            if (@protected)
            {
                ruby_buf.Insert(0, new StringBuilder(Aozora2Html.RUBY_PREFIX));
                @protected = false;
            }
            buffer.Append(ruby_buf.ToString());
            clear();
            return buffer;
        }

        public void push_char(char @char, StringBuilder buffer)
        {
            var ctype = Utils.GetCharType(@char);
            if (ctype is Tag.CharType.HankakuTerminate && char_type is Tag.CharType.Hankaku)
            {
                push(@char.ToString());
                char_type = Tag.CharType.Else;
            }
            else if (@protected || ((ctype is not Tag.CharType.Else) && (ctype == char_type)))
            {
                push(@char.ToString());
            }
            else
            {
                dump_into(buffer);
                push(@char.ToString());
                char_type = ctype;
            }
        }

        public void create_ruby(Aozora2Html parser, string ruby)
        {
            //kurema:
            //`ans = +''` ってのがあるんだけど。`+''`ってのは何？分からん。
            var ans = new StringBuilder();

            throw new NotImplementedException();
        }

        //kurema:
        //char_type()は省略。元々例外発生しないし。
    }
}

