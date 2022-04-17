using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Aozora
{

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
    public class Jstream
    {
        //attr_accessor :line

        public Jstream(StreamReader file_io)
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

        private static string readLineCRLF(System.IO.StreamReader sr)
        {
            //kurema:
            // 参考
            //https://github.com/dotnet/runtime/blob/a3f0e2bebe30fd5e82518d86cefc7895127ae474/src/libraries/System.Private.CoreLib/src/System/IO/StreamReader.cs#L787
            // 要するに\rか\nに当たるまで繰り返し、\rなら次の文字が\nかチェック。
            // ここでは、現時点が\nで1字前が\rならreturn、\r以外なら例外、現時点が\n以外で1字前が\rなら例外。ファイルの末尾が\rの場合も例外。
            // StreamReaderの場所がずれるという問題がある。例外キャッチ後継続するなら問題になる。
            var sb = new StringBuilder();
            var buf = new char[1];
            var bufLast = new char[1] { '\0' };

            while (true)
            {
                sr.Read(buf, 0, 1);
                sb.Append(buf);

                if (buf[0] == '\n')
                {
                    if (bufLast[0] == '\r')
                    {
                        sb.Remove(sb.Length - 2, 2);
                        return sb.ToString();
                    }
                    else
                    {
                        throw new Exceptions.UseCRLFException();
                        //raise Aozora2Html::Error, Aozora2Html::I18n.t(:use_crlf)
                    }
                }
                if (bufLast[0] == '\r')
                {
                    throw new Exceptions.UseCRLFException();
                }
                if (sr.EndOfStream)
                {
                    if (buf[0] == '\r') throw new Exceptions.UseCRLFException();
                    return sb.ToString();
                }
                bufLast = buf;
            }
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

