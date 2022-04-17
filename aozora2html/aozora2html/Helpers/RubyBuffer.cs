using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public class RubyBuffer
    {
        //kurema:
        // 中身ほぼStringBuilderまんま。
        // rubyではcharとstring入り混じったListを使ってるみたいだが、dotnetではそんな必要ない。
        // 全体的に何やってるか分かり辛かった。

        private StringBuilder ruby_buf;
        public bool @protected { get; set; }
        public char_type_kind? char_type { get; set; }

        public enum char_type_kind
        {
            hankaku, hankaku_terminate, @else
        }

        // `｜`が来た時に真にする。ルビの親文字のガード用。
        //attr_accessor :protected

        // @ruby_buf内の文字のchar_type
        //attr_accessor :char_type


#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public RubyBuffer(string? item = null)
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
            foreach(var item in ruby_buf.ToString())
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

        public void dump_into(StringBuilder buffer)
        {
            if (@protected)
            {
                ruby_buf.Insert(0, new StringBuilder(Aozora2Html.RUBY_PREFIX));
                @protected = false;
            }
            buffer.Append(ruby_buf.ToString());
            clear();
        }
    }
}   

