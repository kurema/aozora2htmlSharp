using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers.Tag
{
    /// <summary>
    /// 1行字下げ用
    /// </summary>
    public interface IOnelineIndent
    {
        public string CloseTag();
    }

    /// <summary>
    /// 1行地付き用
    /// </summary>
    public class OnelineChitsuki : Chitsuki, IOnelineIndent
    {
        public OnelineChitsuki(Aozora2Html parser, int length) : base(parser, length)
        {
        }
    }

    /// <summary>
    /// 1行字下げ用
    /// </summary>
    public class OnelineJisage : Jisage, IOnelineIndent
    {
        public OnelineJisage(Aozora2Html parser, int width) : base(parser, width)
        {
        }
    }
}
