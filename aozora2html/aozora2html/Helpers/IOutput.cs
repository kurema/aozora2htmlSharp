using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aozora2html.Helpers
{
    public interface IOutput
    {
        void print(string words);
        void close();
    }
}
