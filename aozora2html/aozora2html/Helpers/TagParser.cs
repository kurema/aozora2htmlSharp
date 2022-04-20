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

        //kurema:ToDo: read_char他を実装。別に難しくないけど継承元が未実装。
    }
}
