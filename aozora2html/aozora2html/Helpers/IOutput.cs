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
        public void close() { }
        public void print(string words) { }
    }

    public class OutputConsole : IOutput
    {
        public void close() { }

        public void print(string words)
        {
            Console.Write(words);
        }
    }

    public class OutputString : IOutput
    {
        StringBuilder content = new StringBuilder();
        public void close()
        {
        }

        public void print(string words)
        {
            content.Append(words);
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}
