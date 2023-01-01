using System;

namespace Aozora
{
    public interface IJstream
    {
        string Inspect { get; }
        int Line { get; }

        void Close();
        char? PeekChar(int pos);
        Helpers.ITextFragment? ReadChar();
        ReadOnlyMemory<char>? ReadLine();
        ReadOnlyMemory<char>? ReadTo(char endchar);
        void RunInitialTest();
    }
}