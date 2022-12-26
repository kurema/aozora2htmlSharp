namespace Aozora
{
    public interface IJstream
    {
        string Inspect { get; }
        int Line { get; }

        void Close();
        char? PeekChar(int pos);
        char? ReadChar();
        string? ReadLine();
        string? ReadTo(char endchar);
        void RunInitialTest();
    }
}