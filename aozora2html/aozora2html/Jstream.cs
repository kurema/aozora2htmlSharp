using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aozora
{

    //Stream class for reading a file.
    //
    //It's just a wrapper class of IO to read characters.
    //when finished to read IO, return a symbol :eof.
    //kurema: No. It just returns null.
    //when found line terminator except CR+LF, exit.
    public class Jstream : IJstream
    {
        public const char CR = '\r';
        public const char LF = '\n';
        public const string CRLF = "\r\n";

        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        char? current_char;
        readonly TextReader file;
        readonly List<int> peekBuffer = new();

        // 初期化と同時に、いったん最初の行をscanして、改行コードがCR+LFかどうか調べる。
        // CR+LFでない場合はエラーメッセージを出力してexitする(!)
        // 
        // TODO: 将来的にはさすがにexitまではしないよう、仕様を変更する?
        //kurema: ここではstrictReturnCodeした場合、例外を吐きます。Exitはしません。
        public Jstream(TextReader file_io, bool strictReturnCode = false)
        {
            _line = 0;
            current_char = null;
            file = file_io;
            StrictReturnCode = strictReturnCode;

            if (strictReturnCode) RunInitialTest();
        }

        //kurema:
        //当たり前ですがStreamReaderにinspectはないので、これの実行結果はRubyの場合とは異なります。
        //どこからも参照されてないのでどうでも良さそう。
        public string Inspect => $"#<jcode-stream input {file}>";

        private int ReadFromFile()
        {
            ReadAny = true;
            if (peekBuffer.Count > 0)
            {
                int result = peekBuffer[0];
                peekBuffer.RemoveAt(0);
                return result;
            }
            else
            {
                return file.Read();
            }
        }

        private int PeekFromFile()
        {
            if (peekBuffer.Count == 0) return file.Peek();
            return peekBuffer[0];
        }

        /// <summary>
        /// 1文字読み込んで返す
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exceptions.UseCRLFException"></exception>
        //kurema:以下のコメントはruby版のものです。こちらでは行末ではLFを、EOFならnullを返します。
        //行末の場合は(1文字ではなく)CR+LFを返す
        //EOFまで到達すると :eof というシンボルを返す
        //
        //TODO: EOFの場合はnilを返すように変更する?
        public char? ReadChar()
        {
            var @char = ReadFromFile();
            ReadAny = true;
            switch (@char)
            {
                case CR:
                    {
                        var char2 = PeekFromFile();
                        if (char2 != LF)
                        {
                            //kurema:\r[\n以外]
                            if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                        }
                        else
                        {
                            //kurema:\r\n
                            ReadFromFile();
                        }
                        _line++;
                        current_char = LF;//kurema:現在が仮に\rだとしても、\nとして返します。
                        break;
                    }

                case LF:
                    //kurema:\n
                    if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                    _line++;
                    current_char = LF;
                    break;
                case -1:
                    current_char = null;
                    break;
                default:
                    current_char = (char)@char;
                    break;
            }
            return current_char;
        }

        public Helpers.ITextFragment? ReadCharAsTextFragment()
        {
            var @char= ReadChar();
            return @char is null ? null : new Helpers.TextFragmentChar(@char.Value);
        }

        //kurema:あってるかなぁ？
        /// <summary>
        /// pos個分の文字を先読みし、最後の文字を返す
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        /// <exception cref="Exceptions.UseCRLFException"></exception>
        //kurema:以下のコメントはRuby版のものです。こちらでは単にバッファに取り込んでいます。
        /*
         * ファイルディスクリプタは移動しない（実行前の位置まで戻す）
         * 行末の場合は(1文字ではなく)CR+LFを返す
         * 行末の先に進んだ場合の挙動は未定義になる
         */
        public char? PeekChar(int pos)
        {
            while (pos >= peekBuffer.Count)
            {
                int result = file.Read();
                if (result < 0)
                {
                    if (peekBuffer.Count == 0 || peekBuffer[peekBuffer.Count - 1] != -1) peekBuffer.Add(result);
                    return null;
                }
                peekBuffer.Add(result);
            }
            var @char = peekBuffer[pos];
            if (@char == CR)
            {
                if (StrictReturnCode)
                {
                    var char2 = pos + 1 < peekBuffer.Count ? peekBuffer[pos + 1] : file.Peek();
                    if (char2 != LF) throw new Exceptions.UseCRLFException();
                }
                @char = LF;
            }
            return IntToNullableChar(@char);
        }

        public void RunInitialTest()
        {
            while (true)
            {
                int @char = file.Read();
                peekBuffer.Add(@char);
                if (@char < 0) return;
                if (@char == CR)
                {
                    int char2 = file.Peek();
                    if (char2 == LF) return;
                    throw new Exceptions.UseCRLFException();
                }
                if (@char == LF)
                {
                    throw new Exceptions.UseCRLFException();
                }
            }
        }

        /// <summary>
        /// 現在の行数を返す
        /// 
        /// 何も読み込む前は0、読み込み始めの最初の文字から\r\nまでが1、その次の文字から次の\r\nは2、……といった値になる
        /// </summary>
        public int Line
        {
            get
            {
                if (ReadAny == false) return 0;
                else if (current_char == LF) return _line;
                else return _line + 1;
            }
        }

        /// <summary>
        /// 指定された終端文字(1文字のStringかCRLF)まで読み込む
        /// </summary>
        /// <param name="endchar">[String] endchar 終端文字</param>
        /// <returns></returns>
        //kurema:終端文字自体は含まない。
        public ReadOnlyMemory<char>? ReadTo(char endchar)
        {
            var buf = new StringBuilder();
            while (true)
            {
                var @char = ReadCharAsTextFragment()?.Char;
                if (@char == endchar) return buf.ToString().AsMemory();
                //kurema:
                //これは何をやってるのか分からない。
                //if char.is_a?(Symbol)
                //  print endchar
                //end
                if (@char == null)
                {
                    var result = buf.ToString();
                    if (string.IsNullOrEmpty(result)) return null;
                    return result.AsMemory();
                }
                if (@char == LF) buf.Append(CRLF);
                else buf.Append(@char);
            }
        }

        /// <summary>
        /// 1行読み込み
        /// </summary>
        /// <returns>[String] 読み込んだ文字列を返す</returns>
        public ReadOnlyMemory<char>? ReadLine() => ReadTo(LF);

        public void Close()
        {
            file.Close();
            file.Dispose();
        }

        private static char? IntToNullableChar(int number) => number < 0 ? null : (char)number;

        public static (StringBuilder result, int count, bool replaced) ReplaceReturnCode(string text, char returnCode)
        {
            return ReplaceReturnCode(text.AsSpan(), returnCode);
        }

        public static (StringBuilder result, int count, bool replaced) ReplaceReturnCode(ReadOnlySpan<char> ros, char returnCode)
        {
            return ReplaceReturnCode(ros, new string(returnCode, 1));
        }

        public static (StringBuilder result, int count, bool replaced) ReplaceReturnCode(string text, string returnCode = CRLF)
        {
            return ReplaceReturnCode(text.AsSpan(), returnCode);
        }

        public static (StringBuilder result, int count, bool replaced) ReplaceReturnCode(ReadOnlySpan<char> ros, string returnCode = CRLF)
        {
            var sb = new StringBuilder();
            int count = 0;
            bool replaced = false;
            while (true)
            {
                var index = ros.IndexOfAny(CR, LF);
                if (index < 0)
                {
#if NET7_0_OR_GREATER
                    sb.Append(ros);
#else
                    sb.Append(ros.ToString());
#endif
                    return (sb, count, replaced);
                }
                count++;
                var ros2 = ros.Slice(0, index);
#if NET7_0_OR_GREATER
                sb.Append(ros2);
#else
                sb.Append(ros2.ToString());
#endif
                sb.Append(returnCode);
                bool isCRLF = (ros[index] == CR && index + 1 < ros.Length && ros[index + 1] == LF);
                replaced = replaced || returnCode switch
                {
                    CRLF => !isCRLF,
                    _ => returnCode.Length != 1 || ros[index] != returnCode[0]
                };
                ros = ros.Slice(index + (isCRLF ? 2 : 1));
            }
        }

        #region ReadLineCRLF
        //kurema:標準で畳まれるようにregionで囲いました。
        //public static string ReadLineCRLF(StreamReader sr)
        //{
        //    //kurema:
        //    // 参考
        //    //https://github.com/dotnet/runtime/blob/a3f0e2bebe30fd5e82518d86cefc7895127ae474/src/libraries/System.Private.CoreLib/src/System/IO/StreamReader.cs#L787
        //    // 要するに\rか\nに当たるまで一文字づつ読み込み、\r\n以外なら例外。
        //    var sb = new StringBuilder();
        //    char current = '\0';

        //    while (true)
        //    {
        //        int nextResult = sr.Peek();
        //        if (nextResult == -1)
        //        {
        //            if (current == '\r') throw new Exceptions.UseCRLFException();//kurema:[\r][EOF]
        //            else return sb.ToString();
        //        }
        //        char next = (char)nextResult;

        //        if (next == '\n')
        //        {
        //            if (current == '\r')
        //            {
        //                //kurema:\r\n
        //                sr.Read();
        //                return sb.ToString();
        //            }
        //            else
        //            {
        //                //kurema:[\r以外][\n]
        //                sr.Read();
        //                throw new Exceptions.UseCRLFException();
        //            }
        //        }
        //        if (current == '\r')
        //        {
        //            //kurema:[\r][\n以外]
        //            throw new Exceptions.UseCRLFException();
        //        }
        //        if (next != '\r')
        //        {
        //            sb.Append(next);
        //        }
        //        sr.Read();
        //        current = next;
        //    }
        //}
        #endregion
    }
}

