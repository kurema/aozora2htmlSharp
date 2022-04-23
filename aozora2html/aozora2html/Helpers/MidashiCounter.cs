using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    /// <summary>
    /// 見出しIDカウンター
    /// 
    /// 主にmidashi_idを管理する
    /// </summary>
    public class MidashiCounter
    {
        int midashi_id = 0;

        public MidashiCounter(int current_id)
        {
            midashi_id = current_id;
        }

        public int generate_id(int size)
        {
            midashi_id += size;
            return midashi_id;
        }

        public int generate_id(string size)
        {
            int inc;
            if (size.Contains(new string(Aozora2Html.SIZE_SMALL, 1))) inc = 1;
            else if (size.Contains(new string(Aozora2Html.SIZE_MIDDLE, 1))) inc = 10;
            else if (size.Contains(new string(Aozora2Html.SIZE_LARGE, 1))) inc = 100;
            else throw new Exceptions.UndefinedHeaderException();

            midashi_id += inc;
            return midashi_id;
        }
    }
}
