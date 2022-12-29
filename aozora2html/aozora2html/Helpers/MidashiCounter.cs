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

        public int GenerateId(int size)
        {
            midashi_id += size;
            return midashi_id;
        }

        public int GenerateId(string size)
        {
            //kurema:IndexOfAny()にしても速度はほぼ変わらないはず。そして複数のサイズが含まれる場合の挙動が変わります(仕様外)。
            //int inc;
            //if (size.IndexOf(Aozora2Html.SIZE_SMALL) >= 0) inc = 1;
            //else if (size.IndexOf(Aozora2Html.SIZE_MIDDLE) >= 0) inc = 10;
            //else if (size.IndexOf(Aozora2Html.SIZE_LARGE) >= 0) inc = 100;
            //else throw new Exceptions.UndefinedHeaderException();

            //midashi_id += inc;
            //return midashi_id;

            int hit = size.IndexOfAny(new[] { Aozora2Html.SIZE_SMALL, Aozora2Html.SIZE_MIDDLE, Aozora2Html.SIZE_LARGE });
            if (hit < 0) throw new Exceptions.UndefinedHeaderException();
            midashi_id += size[hit] switch
            {
                Aozora2Html.SIZE_SMALL => 1,
                Aozora2Html.SIZE_MIDDLE => 10,
                Aozora2Html.SIZE_LARGE => 100,
                _ => throw new Exceptions.UndefinedHeaderException(),
            };
            return midashi_id;
        }
    }
}
