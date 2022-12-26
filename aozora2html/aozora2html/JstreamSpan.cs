using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Aozora
{
#if NET7_0_OR_GREATER
    public class JstreamSpan : IJstream
    {
        public string Inspect => $"#<jcode-stream input {file}>";

        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        char? current_char;
        readonly TextReader file;

        int position = 0;
        const int bufferSizeDefault = 2048;
        char[] Buffer;
        int BufferSize = 0;
        char[] BufferPeek;
        int BufferPeekSize = 0;

        public int Line
        {
            get
            {
                if (ReadAny == false) return 0;
                else if (current_char == Jstream.LF) return _line;
                else return _line + 1;
            }
        }
        public JstreamSpan(TextReader file_io, bool strictReturnCode = false, int bufferSize = bufferSizeDefault)
        {
            _line = 0;
            current_char = null;
            file = file_io;
            StrictReturnCode = strictReturnCode;

            if (strictReturnCode) RunInitialTest();
            Buffer = new char[bufferSize];
            BufferPeek = new char[bufferSize];
        }

        public void Close()
        {
            file.Close();
            file.Dispose();
        }

        public char? PeekChar(int pos)
        {
            if (Buffer.Length == pos + position - 1)
            {

            }
            else if (Buffer.Length > pos + position)
            {
                var char1 = Buffer[pos + position];
                if (char1 == Jstream.CR && StrictReturnCode)
                {
                    var char2 = Buffer[pos + position + 1];
                    if (char2 != Jstream.LF) throw new Exceptions.UseCRLFException();
                }
                return char1;
            }

            BufferPeek = new char[BufferSize];
            //file.Read(Buffer, 0, );
            throw new NotImplementedException();

        }

        public char? ReadChar()
        {
            throw new NotImplementedException();
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
#endif
}
