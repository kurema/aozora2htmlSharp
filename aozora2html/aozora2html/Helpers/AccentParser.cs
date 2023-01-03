using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public class AccentParser : Aozora2Html
    {
        protected bool closed = false; //改行での強制撤退チェックフラグ
        protected bool encount_accent = false;

        public AccentParser(IJstream input, char? endchar, Dictionary<ChuukiTableKeys, bool> chuuki, List<(string, List<string>)> images, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null) : base(input, output, warnChannel, gaiji_dir, css_files)
        {
            chuuki_table = chuuki;
            this.endchar = endchar; //改行は越えられない <br />を出力していられない
            this.images = images; //globalな環境を記録するアイテムは共有する必要あり

        }

        //出力は配列で返す
        public TextBuffer GeneralOutputAccent()
        {
            //kurema:返り値がAozora2Htmlのgeneral_output()と違うので名前を変えました。
            ruby_buf.DumpInto(buffer);
            if (!encount_accent) buffer.Insert(0, new BufferItemString(new string(ACCENT_BEGIN, 1)));
            if (closed && !encount_accent)
            {
                buffer.Add(new BufferItemString(new string(ACCENT_END, 1)));
            }
            else if (!closed)
            {
                buffer.Add(new BufferItemString("<br />\r\n"));
            }
            return buffer;
        }

        public override bool Parse()
        {
            while (true)
            {
                var first = ReadChar();
                var second = stream.PeekChar(0);
                var third = stream.PeekChar(1);
                var (code, name, depth) = YamlValues.AccentTable(first, second, third);

                IBufferItem? bufferItem = null;

                if (code is not null && name is not null)
                {
                    for (int i = 1; i < depth; i++)
                    {
                        ReadChar();
                    }
                    encount_accent = true;
                    chuuki_table[ChuukiTableKeys.accent] = true;
                    first = null;
                    bufferItem = new BufferItemTag(new Tag.Accent(this, code, name, gaiji_dir));
                }

                switch (first)
                {
                    case GAIJI_MARK:
                        first = null;
                        bufferItem = DispatchGaiji();
                        break;
                    case COMMAND_BEGIN:
                        first = null;
                        bufferItem = DispatchAozoraCommand();
                        break;
                    case KU:
                        AssignKunoji();
                        break;
                    case RUBY_BEGIN_MARK:
                        first = null;
                        bufferItem = new BufferItemString(ApplyRuby() ?? "");
                        break;
                }

                if (bufferItem is BufferItemString bufferString)
                {
                    var text = bufferString.ToHtml();
                    if (text?.Length == 0)
                    {
                        first = null;
                        bufferItem = null;
                    }
                    else if (text?.Length == 1)
                    {
                        first = text[0];
                        bufferItem = null;
                    }
                }

                if (first == '\n')
                {
                    if (encount_accent) warnChannel?.PrintLine(String.Format(Resources.Resource.WarnInvalidAccentBrancket, LineNumber));
                    return false;
                    //throw new Exceptions.TerminateException();
                }
                else if (first == ACCENT_END)
                {
                    closed = true;
                    return false;
                    //throw new Exceptions.TerminateException();
                }
                else if (first == RUBY_PREFIX)
                {
                    ruby_buf.DumpInto(buffer);
                    ruby_buf.IsProtected = true;
                }
                else if (first is not null)
                {
                    Utils.IllegalCharCheck(first.Value, LineNumber, warnChannel);
                    PushChars(EscapeSpecialChars(first.Value));
                }
                else if (bufferItem is not null)
                {
                    var text = bufferItem.ToHtml();
                    if (!string.IsNullOrEmpty(text))
                    {
                        Utils.IllegalCharCheck(bufferItem, LineNumber, warnChannel);
                        PushChars(EscapeSpecialChars(bufferItem));
                    }
                }
            }
        }

        public TextBuffer ProcessAccent()
        {
            try
            {
                Parse();
            }
            catch (Exceptions.TerminateException)
            {
                return GeneralOutputAccent();
            }
            return GeneralOutputAccent();
            //throw new Exception();//kurema:parse()から脱出する方法がないのでここには来ない。
        }
    }
}
