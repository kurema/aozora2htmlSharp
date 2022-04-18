using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Aozora
{
    public class Jstream
    {
        public const char CR = '\r';
        public const char LF = '\n';
        public const string CRLF = "\r\n";

        public bool StrictReturnCode;
        private bool ReadAny = false;

        int _line;
        char? current_char;
        StreamReader file;

        public Jstream(StreamReader file_io, bool strictReturnCode = false)
        {
            _line = 0;
            current_char = null;
            file = file_io;
            StrictReturnCode = strictReturnCode;

            //kurema:
            //CRLFチェックはStreamReaderでは厄介なので省略しました。具体的には巻き戻せません。
            //その代わり、読みだし時にチェックします。(strictReturnCodeがtrueの場合)
        }

        //kurema:当たり前ですがStreamReaderにinspectはないので、これの実行結果はRubyの場合とは異なります。
        public string inspect => $"#<jcode-stream input {file}>";

        public char? read_char()
        {
            char? @char = IntToNullableChar(file.Read());
            ReadAny = true;
            if (@char == CR)
            {
                var char2 = IntToNullableChar(file.Peek());
                if (char2 != LF)
                {
                    //kurema:\r[\n以外]
                    if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                }
                else
                {
                    //kurema:\r\n
                    file.Read();
                }
                _line++;
                current_char = LF;//kurema:現在が仮に\rだとしても、\nとして返します。
            }
            else if (@char == LF)
            {
                //kurema:\n
                if (StrictReturnCode) throw new Exceptions.UseCRLFException();
                _line++;
                current_char = LF;
            }
            ////kurema:元々は:eofを返してましたが、nullを返すようにしたので分岐は不要になりました。
            //else if (@char == null)
            //{
            //    current_char = null;
            //}
            else
            {
                current_char = @char;
            }

            return current_char;
        }

        public char? peek_char(int pos)
        {
            //kurema:
            //複数文字peekはZipファイルとかだと無理です。
            //そもそもBaseStreamを勝手にSeekしても良いの？
            //駄目です。
            if (!file.BaseStream.CanSeek) throw new NotSupportedException();
            var original_pos = file.BaseStream.Position;
            char? @char = null;
            try
            {
                for(int i = 1; i < pos; i++)
                {
                    int result = file.Read();
                    if (result < 0) return null;
                }
            }
            finally
            {
                file.BaseStream.Seek(original_pos, SeekOrigin.Begin);
            }
            return @char;
            
        }

        public int line
        {
            get
            {
                if (ReadAny == false) return 0;
                else if (current_char == LF) return line;
                else return line + 1;
            }
        }

        private static char? IntToNullableChar(int number)
        {
            return number < 0 ? null : (char)number;
        }

        #region ReadLineCRLF
        //kurema:標準で畳まれるようにregionで囲いました。
        public static string ReadLineCRLF(StreamReader sr)
        {
            //kurema:
            // 参考
            //https://github.com/dotnet/runtime/blob/a3f0e2bebe30fd5e82518d86cefc7895127ae474/src/libraries/System.Private.CoreLib/src/System/IO/StreamReader.cs#L787
            //要するに\rか\nに当たるまで一文字づつ読み込み、\r\n以外なら例外。
            var sb = new StringBuilder();
            char current = '\0';

            while (true)
            {
                int nextResult = sr.Peek();
                if (nextResult == -1)
                {
                    if (current == '\r') throw new Exceptions.UseCRLFException();//kurema:[\r][EOF]
                    else return sb.ToString();
                }
                char next = (char)nextResult;

                if (next == '\n')
                {
                    if (current == '\r')
                    {
                        //kurema:\r\n
                        sr.Read();
                        return sb.ToString();
                    }
                    else
                    {
                        //kurema:[\r以外][\n]
                        sr.Read();
                        throw new Exceptions.UseCRLFException();
                    }
                }
                if (current == '\r')
                {
                    //kurema:[\r][\n以外]
                    throw new Exceptions.UseCRLFException();
                }
                if (next != '\r')
                {
                    sb.Append(next);
                }
                sr.Read();
                current = next;
            }
        }
        #endregion
    }


    //require "aozora2html/error"
    //require "aozora2html/i18n"

    //#
    // Stream class for reading a file.
    //
    // It's just a wrapper class of IO to read characters.
    // when finished to read IO, return a symbol :eof.
    // when found line terminator except CR+LF, exit.
    //
    //kurema:
    // :eofシンボルなんて返しようがないので、nullを返す仕組みにしました。
    // CR+LFか否かはどちらでも良いかと。
    public class Jstream_old
    {
        //attr_accessor :line

        public Jstream_old(StreamReader file_io)
        {
            line = 0;
            entry = false;
            file = file_io;
            try
            {
                store_to_buffer();
            }
            catch (Exceptions.AozoraException)
            {
                //kurema:
                // 元はキャッチしてメッセージを出力後終了。
                // dotnetで例外をそれをやるのは意味不明なので何もせず再スロー。

                //Console.Error.Write(e.Message);
                //puts e.message(1)
                throw;
            }
        }

        public string inspect()
        {
            return $"#<jcode-stream input {file}>";
        }

        public char? read_char()
        {
            int found = buffer is null || buffer_positon >= buffer.Length ? -1 : buffer[buffer_positon];
            buffer_positon++;
            if (entry)
            {
                line++;
                entry = false;
            }
            if (found != -1)
            {
                return Convert.ToChar(found);
            }

            store_to_buffer();
            if (isEof) return null;
            return '\n';
        }


        public char peek_char(int pos)
        {
            //kurema:
            //本来は\r\nを返す。C#ではcharとstringは分けた方が良いので\nにした。
            //なおファイル末尾でも\nを返すので挙動としては変。
            if (buffer?.Length > pos + buffer_positon && pos + buffer_positon >= 0) return buffer[buffer_positon];
            return '\n';
            //@buffer[pos] || "\r\n"
        }


        public void close()
        {
            file.Close();
        }

        //private

        private void store_to_buffer()
        {
            buffer = file.ReadLine();
            //kurema:
            // 元は\r\nじゃないと例外を吐くんですが、ここではどっちでも良いので例外を出していません。
            // ↓はきちんと例外吐く方(未テスト)。
            //buffer = readLineCRLF(file);
            entry = true;
        }

        public int line { get; set; }
        private bool entry;
        private readonly StreamReader file;
        private string? _buffer;
        private string? buffer { get => _buffer; set { _buffer = value; buffer_positon = 0; } }
        private int buffer_positon = 0;
        public bool isEof => _buffer is null;
    }

}

