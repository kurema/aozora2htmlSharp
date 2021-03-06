using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aozora.Helpers
{
    public interface IOutput
    {
        void Print(string words);
        void PrintLine(string words);
        void Close();
    }

    public class OutputDummy : IOutput
    {
        public void Close() { }
        public void Print(string words) { }

        public void PrintLine(string words) { }
    }

    public class OutputConsole : IOutput
    {
        public void Close() { }

        public void Print(string words)
        {
            Console.Write(words);
        }

        public void PrintLine(string words)
        {
            Console.WriteLine(words);
        }
    }

    public class OutputConsoleError : IOutput
    {
        public void Close()
        {
        }

        public void Print(string words)
        {
            Console.Error.Write(words);
        }

        public void PrintLine(string words)
        {
            Console.Error.WriteLine(words);
        }
    }

    public class OutputStreamWriter : IOutput
    {
        public StreamWriter? Content { get; private set; }

        public OutputStreamWriter(StreamWriter content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public void Close()
        {
            Content?.Flush();
            Content?.Close();
            Content?.Dispose();
            Content = null;
        }

        public void Print(string words)
        {
            Content?.Write(words);
        }

        public void PrintLine(string words)
        {
            Content?.Write(words);
            Content?.Write("\r\n");
        }
    }

    public class OutputString : IOutput
    {
        readonly StringBuilder content = new();
        public void Close()
        {
        }

        public void Print(string words)
        {
            content.Append(words);
        }

        public void PrintLine(string words)
        {
            content.Append(words);
            content.Append("\r\n");
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}
