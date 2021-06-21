using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aozora.Helpers
{
    public interface IOutput
    {
        void print(string words);
        void close();
    }

    public class OutputDummy : IOutput
    {
        public void close() {}
        public void print(string words) { }
    }
}
