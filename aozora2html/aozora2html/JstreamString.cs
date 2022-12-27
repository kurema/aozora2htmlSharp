using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aozora
{
    public class JstreamString : IJstream
    {
        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        int position = 0;
        char? current_char => text[position] == Jstream.CR ? Jstream.LF : text[position];
        string text;

        public JstreamString(string input, bool strictReturnCode = false)
        {
            _line = 0;
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
            var peeked = text[position + pos];
            if (peeked == Jstream.CR)
            {
                if (StrictReturnCode && !(position + pos + 1 < text.Length && text[position + pos + 1] == Jstream.LF))
                    throw new Exceptions.UseCRLFException();
                peeked = Jstream.LF;
            }
            return peeked;
        }

        public char? ReadChar()
        {
            ReadAny = true;
            var char1 = GetCharAt(position);
            var char2 = GetCharAt(position + 1);
            if (char1 is Jstream.LF)
            {
                _line++;
                if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                position++;
                return char1;
            }
            if(char1 is Jstream.CR)
            {
                _line++;
                if (char2 is Jstream.LF)
                {
                    position += 2;
                }
                else
                {
                    if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                    position++;
                }
                return Jstream.LF;
            }
            position++;
            return char1;
        }

        public string? ReadLine() => ReadTo(Jstream.LF);

        public string? ReadTo(char endchar)
        {
            //kurema:ReadOnlySpanで返した方が良さそうな雰囲気あるけど、これが呼ばれる状況は少ないので気にしなくて良さそう。
            //kurema:なおSpanが使えるのはSystem.Text.Encoding.CodePagesがSystem.Memoryを参照しているから。
            //kurema:String.Concat()のようなものは使えない。
            ReadAny = true;
            var ros = text.AsSpan().Slice(position);
            var index = ros.IndexOfAny(Jstream.CR, Jstream.LF, endchar);
            if (index < 0)
            {
                position += ros.Length;
                if (ros.Length == 0) return null;
                else return ros.ToString();
            }
            var char1 = GetCharAt(position + index);
            var char2 = GetCharAt(position + index + 1);
            if (char1 is Jstream.CR or Jstream.LF)
            {
                //kurema:ros2にはCRもLF含まれない。
                var ros2 = ros.Slice(0, index);
                if (char1 is Jstream.LF)
                {
                    if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                    position += index + 1;
                }
                else if (char1 is Jstream.CR)
                {
                    if (char2 is Jstream.LF)
                    {
                        position += index + 2;
                    }
                    else
                    {
                        if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                        position += index + 1;
                    }
                }
                _line++;
#if NET7_0_OR_GREATER
                return string.Concat(ros2, Jstream.CRLF);
#else
                //kurema:ToString()したらReadOnlySpan使う意味がないが。
                return ros2.ToString() + Jstream.CRLF;
#endif
            }
            else
            {
                position += index + 1;
                return ros.Slice(0, index + 1).ToString();
            }
        }

        public void RunInitialTest()
        {
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == Jstream.CR)
                {
                    if (i + 1 < text.Length && text[i + 1] == Jstream.LF) return;
                    throw new Exceptions.UseCRLFException();
                }
                if (ch == Jstream.LF)
                {
                    throw new Exceptions.UseCRLFException();
                }
            }
            return;
        }

        private char? GetCharAt(int position) => position < text.Length ? text[position] : null;
    }
}
