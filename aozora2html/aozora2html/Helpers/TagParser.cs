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

        readonly TextFragmentStringBuilder _raw = new();//外字変換前の生テキストを残したいことがあるらしい

        public TagParser(IJstream input, char? endchar, Dictionary<ChuukiTableKeys, bool> chuuki, List<(string, List<string>)> images, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null) : base(input, output, warnChannel, gaiji_dir, css_files)
        {
            Section = SectionKind.tail; //末尾処理と記法内はインデントをしないので等価
            chuuki_table = chuuki;
            this.endchar = endchar; //改行を越えるべきか否か…
            this.images = images; //globalな環境を記録するアイテムは共有する必要あり
        }

        //method override!
        public override Helpers.ITextFragment? ReadCharAsTextFragment()
        {
            var c = base.ReadCharAsTextFragment();
            //if(_raw_fragment?.TryAppend(c,ref _raw_fragment))
            _raw.Append(c);
            return c;
        }

        public override char? ReadChar()
        {
            var c = base.ReadChar();
            if (c is not null) _raw.Append(new string(c.Value, 1));
            return c;
        }

        protected override (string, string) ReadToNest(char? endchar)
        {
            var ans = base.ReadToNest(endchar);
            _raw.Append(ans.Item2);
            return ans;
        }

        //出力は[String,String]返しで！
        public (string, string) GeneralOutputTagParser()
        {
            //kurema:こちらも返り値が違うので名前変えてます。
            ruby_buf.DumpInto(buffer);
            var ans = new StringBuilder();
            foreach (var s in buffer)
            {
                if ((s as BufferItemTag)?.Content is Tag.UnEmbedGaiji gaiji && !gaiji.Escaped)
                {
                    //消してあった※を復活させ
                    ans.Append(GAIJI_MARK);
                }
                ans.Append(s.ToHtml());
            }
            return (ans.ToString(), _raw.ToString());
        }

        public (string, string) ProcessTag()
        {
            try
            {
                Parse();
            }
            catch (Exceptions.TerminateException)
            {
                return GeneralOutputTagParser();
            }
            throw new Exception();//kurema:parse()から脱出する方法がないのでここには来ない。
        }
    }
}
