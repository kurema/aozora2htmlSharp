using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public class AccentParser : Aozora2Html
    {
        protected bool closed = false; //改行での強制撤退チェックフラグ
        protected bool encount_accent = false;

        public AccentParser(Jstream input, char endchar, Dictionary<string, string> chuuki, List<string> images, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null) : base(input, output, warnChannel, gaiji_dir, css_files)
        {
            chuuki_table = chuuki;
            this.endchar = endchar; //改行は越えられない <br />を出力していられない
            this.images = images; //globalな環境を記録するアイテムは共有する必要あり

        }

        //kurema:ToDo: general_output他を実装。別に難しくないけど継承元が未実装。

    }
}
