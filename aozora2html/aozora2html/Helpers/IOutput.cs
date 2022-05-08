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

    public class OutputConsoleError : IOutput
    {
        public void close()
        {
        }

        public void print(string words)
        {
            Console.Error.Write(words);
        }

        public void println(string words)
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

        public void close()
        {
            Content?.Flush();
            Content?.Close();
            Content?.Dispose();
            Content = null;
        }

        public void print(string words)
        {
            Content?.Write(words);
        }

        public void println(string words)
        {
            Content?.Write(words);
            Content?.Write("\r\n");
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
            content.Append(words);
            content.Append("\r\n");
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}
