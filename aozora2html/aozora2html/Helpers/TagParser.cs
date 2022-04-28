using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    /// <summary>
    /// 注記記法parser
    /// </summary>
    //青空記法の入れ子に対応（？）
    public class TagParser : Aozora2Html
    {
        public string Raw => _raw.ToString();
        StringBuilder _raw = new();//外字変換前の生テキストを残したいことがあるらしい

        public TagParser(Jstream input, char? endchar, Dictionary<chuuki_table_keys, bool> chuuki, List<string> images, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null) : base(input, output, warnChannel, gaiji_dir, css_files)
        {
            section = SectionKind.tail; //末尾処理と記法内はインデントをしないので等価
            chuuki_table = chuuki;
            this.endchar = endchar; //改行を越えるべきか否か…
            this.images = images; //globalな環境を記録するアイテムは共有する必要あり
        }

        //method override!
        public override char? read_char()
        {
            var c= base.read_char();
            _raw.Append(c);
            return c;
        }

        protected override (string, string) read_to_nest(char? endchar)
        {
            var ans = base.read_to_nest(endchar);
            _raw.Append(ans.Item2);
            return ans;
        }

        //出力は[String,String]返しで！
        public (string,string) general_output_TagParser()
        {
            //kurema:こちらも返り値が違うので名前変えてます。
            ruby_buf.dump_into(buffer);
            var ans = new StringBuilder();
            foreach(var s in buffer)
            {
                if((s as BufferItemTag)?.tag is Tag.UnEmbedGaiji gaiji && !gaiji.escaped)
                {
                    //消してあった※を復活させ
                    ans.Append(GAIJI_MARK);
                }
                ans.Append(s.to_html());
            }
            return (ans.ToString(), _raw.ToString());
        }

        public (string, string) process()
        {
            try
            {
                parse();
            }
            catch (Exceptions.TerminateException){
                return general_output_TagParser();
            }
            throw new Exception();//kurema:parse()から脱出する方法がないのでここには来ない。
        }
    }
}
