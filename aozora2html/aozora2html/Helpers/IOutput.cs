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
        void println(string words);
        void close();
    }

    public class OutputDummy : IOutput
    {
        public void close() { }
        public void print(string words) { }

        public void println(string words) { }
    }

    public class OutputConsole : IOutput
    {
        public void close() { }

        public void print(string words)
        {
            Console.Write(words);
        }

        public void println(string words)
        {
            Console.WriteLine(words);
        }
    }

    public class OutputString : IOutput
    {
        readonly StringBuilder content = new();
        public void close()
        {
        }

        public void print(string words)
        {
            content.Append(words);
        }

        public void println(string words)
        {
            content.AppendLine(words);
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}
