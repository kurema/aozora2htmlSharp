using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aozora
{
    public class JstreamString : IJstream
    {
        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        int position = 0;
        char? current_char;
        string text;

        public JstreamString(string input, bool strictReturnCode = false)
        {
            _line = 0;
            current_char = null;
            text = input;
            StrictReturnCode = strictReturnCode;

            if (strictReturnCode) RunInitialTest();
        }

        public string Inspect => $"#<jcode-stream input>";

        public int Line
        {
            get
            {
                if (ReadAny == false) return 0;
                else if (current_char == Jstream.LF) return _line;
                else return _line + 1;
            }
        }

        public void Close()
        {
            text = string.Empty;
        }

        public char? PeekChar(int pos)
        {
            if (position + pos >= text.Length) return null;
            var peeked= text[position + pos];
            if (peeked == Jstream.CR)
            {
                if (StrictReturnCode && (position + pos + 1 >= text.Length || text[position + pos + 1] != Jstream.LF))
                    throw new Exceptions.UseCRLFException();
                peeked = Jstream.LF;
            }
            return peeked;
        }

        public char? ReadChar()
        {
            throw new NotImplementedException();
            ReadAny = true;
            if (position >= text.Length) return null;
            var result = text[position];
            position++;
            return result;
        }

        public string? ReadLine()
        {
            throw new NotImplementedException();
        }

        public string? ReadTo(char endchar)
        {
            throw new NotImplementedException();
        }

        public void RunInitialTest()
        {
            throw new NotImplementedException();
        }
    }
}
