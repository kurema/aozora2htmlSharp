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
        void Print(ReadOnlySpan<char> words);
        void PrintLine(string words);
        void PrintLine(ReadOnlySpan<char> words);
        void Close();
    }

    public class OutputDummy : IOutput
    {
        public void Close() { }
        public void Print(string words) { }

        public void Print(ReadOnlySpan<char> words)
        { }

        public void PrintLine(string words) { }

        public void PrintLine(ReadOnlySpan<char> words)
        { }
    }

    public class OutputConsole : IOutput
    {
        public void Close() { }

        public void Print(string words)
        {
            Console.Write(words);
        }

        public void Print(ReadOnlySpan<char> words)
        {
            Console.Write(words.ToString());
        }

        public void PrintLine(string words)
        {
            Console.WriteLine(words);
        }

        public void PrintLine(ReadOnlySpan<char> words)
        {
            Console.WriteLine(words.ToString());
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

        public void Print(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            Console.Error.Write(words);
#else
            Console.Error.Write(words.ToString()); 
#endif
        }

        public void PrintLine(string words)
        {
            Console.Error.WriteLine(words);
        }

        public void PrintLine(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            Console.Error.WriteLine(words);
#else
            Console.Error.WriteLine(words.ToString()); 
#endif
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

        public void Print(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            Content?.Write(words);
#else
            Content?.Write(words.ToString());
#endif
        }

        public void PrintLine(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            Content?.WriteLine(words);
#else
            Content?.WriteLine(words.ToString());
#endif
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

        public void Print(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            content.Append(words);
#else
content.Append(words.ToString());
#endif
        }

        public void PrintLine(string words)
        {
            content.Append(words);
            content.Append("\r\n");
        }

        public void PrintLine(ReadOnlySpan<char> words)
        {
#if NET7_0_OR_GREATER
            content.Append(words);
#else
content.Append(words.ToString());
#endif
            content.Append("\r\n");
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}
