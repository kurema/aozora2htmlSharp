using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aozora
{
    public partial class JstreamString : IJstream
    {
        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        int position = 0;
        char? current_char => position < text.Length && position >= 0 ? text.Span[position] == Jstream.CR ? Jstream.LF : text.Span[position] : null;
        ReadOnlyMemory<char> text;

        public JstreamString(string input, bool strictReturnCode = false) : this(input.AsMemory(), strictReturnCode)
        {
        }

        public JstreamString(ReadOnlyMemory<char> input, bool strictReturnCode = false)
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
                //else if (current_char == Jstream.LF) return _line;
                else return _line + 1;
            }
        }

        public void Close()
        {
            text = ReadOnlyMemory<char>.Empty;
            position = 0;
            _line = 0;
        }

        public char? PeekChar(int pos)
        {
            if (position + pos >= text.Length) return null;
            var peeked = text.Span[position + pos];
            if (peeked == Jstream.CR)
            {
                if (StrictReturnCode && !(position + pos + 1 < text.Length && text.Span[position + pos + 1] == Jstream.LF))
                    throw new Exceptions.UseCRLFException();
                peeked = Jstream.LF;
            }
            return peeked;
        }

        public Helpers.ITextFragment? ReadCharAsTextFragment()
        {
            ReadAny = true;
            var char1 = GetCharAt(position);
            var char2 = GetCharAt(position + 1);
            if (char1 is Jstream.LF)
            {
                _line++;
                if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                return new Helpers.TextFragmentMemory(text, position++, 1);
            }
            if (char1 is Jstream.CR)
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
                return new Helpers.TextFragmentChar(Jstream.LF);
            }

            return new Helpers.TextFragmentMemory(text, position++, 1);
        }

        public char? ReadChar()
        {
            return ReadCharAsTextFragment()?.Char;
        }


        public ReadOnlyMemory<char>? ReadLine() => ReadTo(Jstream.LF);

        public ReadOnlyMemory<char>? ReadTo(char endchar)
        {
            //kurema:一応ReadOnlyMemory<char>で返すようにしたけど、互換性の点で不安。でも.ToString()で済むからReadToMemory()を足す必要は感じない。
            //kurema:なおSpanが使えるのはSystem.Text.Encoding.CodePagesがSystem.Memoryを参照しているから。
            //kurema:正直パフォーマンス上のメリットも特にないと思う。
            ReadAny = true;
            if (text.Length <= position) return null;
            var ros = text.Slice(position);
            var index = endchar is Jstream.CR or Jstream.LF ? ros.Span.IndexOfAny(Jstream.CR, Jstream.LF) : ros.Span.IndexOf(endchar);
            ReadOnlyMemory<char> ros2;
            if (index < 0)
            {
                position += ros.Length;
                if (ros.Length == 0) return null;
                else ros2 = ros;
            }
            else
            {
                position += index;
                ReadIfReturn();
                ros2 = ros.Slice(0, index);
            }
            if (ros2.Span.IndexOfAny(Jstream.CR, Jstream.LF) < 0)
            {
                return ros2;
            }
            else
            {
                //kurema:実際は現時点でここが呼ばれることはないはずです。
                //kurema:逆にこの部分は十分にテストがされないことになります(一応テストは追加しました)。
                var (result, line, replaced) = Jstream.ReplaceReturnCode(ros2.Span, Jstream.CRLF);
                if (StrictReturnCode && replaced) throw new Exceptions.UseCRLFException();
                _line += line;
                return result.ToString().AsMemory();
            }
        }

        private void ReadIfReturn()
        {
            char? char1 = GetCharAt(position);
            char? char2 = GetCharAt(position + 1);

            if (StrictReturnCode)
            {
                // /^\r[^\n]|^\r/
                if (char1 is Jstream.CR && char2 is not Jstream.LF || char1 is Jstream.LF)
                {
                    position++;
                    throw new Exceptions.UseCRLFException();
                }
                else
                {
                    position += 2;
                }
            }
            else
            {
                if (char1 is Jstream.CR && char2 is Jstream.LF)
                {
                    _line++;
                    position += 2;
                }
                else if (char1 is Jstream.CR or Jstream.LF)
                {
                    _line++;
                    position++;
                }
            }
        }

        public void RunInitialTest()
        {
            //kurema:下手するとファイル全部確認する落ちになる危険なコード
            //int indexCR = text.Span.IndexOf(Jstream.CR);
            //int indexLF = text.Span.IndexOf(Jstream.LF);
            //if (indexCR == -1 && indexLF == -1) return;
            //if(indexLF!=indexCR) throw new Exceptions.UseCRLFException();
            int index1 = text.Span.IndexOfAny(Jstream.CR, Jstream.LF);
            var span = text.Span.Slice(index1);
            if (span[0] is not Jstream.CR || span.Length == 0 || span[1] is not Jstream.LF) throw new Exceptions.UseCRLFException();
            return;
        }

        private char? GetCharAt(int position) => position < text.Length && position >= 0 ? text.Span[position] : null;
    }
}
