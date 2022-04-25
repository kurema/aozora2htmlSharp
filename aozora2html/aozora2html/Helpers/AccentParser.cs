using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public class AccentParser : Aozora2Html
    {
        protected bool closed = false; //改行での強制撤退チェックフラグ
        protected bool encount_accent = false;

        public AccentParser(Jstream input, char? endchar, Dictionary<chuuki_table_keys, bool> chuuki, List<string> images, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null) : base(input, output, warnChannel, gaiji_dir, css_files)
        {
            chuuki_table = chuuki;
            this.endchar = endchar; //改行は越えられない <br />を出力していられない
            this.images = images; //globalな環境を記録するアイテムは共有する必要あり

        }

        //出力は配列で返す
        public TextBuffer general_output_accent()
        {
            //kurema:返り値がAozora2Htmlのgeneral_output()と違うので名前を変えました。
            ruby_buf.dump_into(buffer);
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

        public override void parse()
        {
            while (true)
            {
                var first = read_char();
                var second = stream.peek_char(0);
                var third = stream.peek_char(1);
                var found = YamlValues.AccentTable(first, second, third);

                IBufferItem? bufferItem = null;

                if (found.code is not null && found.name is not null)
                {
                    for (int i = 1; i < found.depth; i++)
                    {
                        read_char();
                    }
                    encount_accent = true;
                    chuuki_table[chuuki_table_keys.accent] = true;
                    first = null;
                    bufferItem = new BufferItemTag(new Tag.Accent(this, found.code, found.name, gaiji_dir));
                }

                switch (first)
                {
                    case GAIJI_MARK:
                        first = null;
                        bufferItem = dispatch_gaiji();
                        break;
                    case COMMAND_BEGIN:
                        first = null;
                        bufferItem = dispatch_aozora_command();
                        break;
                    case KU:
                        first = null;
                        bufferItem = new BufferItemString(apply_ruby() ?? "");
                        break;
                }

                if (bufferItem is BufferItemString bufferString)
                {
                    var text = bufferString.to_html();
                    if (text?.Length == 0)
                    {
                        first = null;
                        text = null;
                    }
                    else if (text?.Length == 1)
                    {
                        first = text[0];
                        text = null;
                    }
                }

                if (first == '\n')
                {
                    if (encount_accent) warnChannel?.print(String.Format(I18n.MSG["warn_invalid_accent_brancket"], line_number));
                    throw new Exceptions.TerminateException();
                }
                else if (first == ACCENT_END)
                {
                    closed = true;
                    throw new Exceptions.TerminateException();
                }
                else if (first == RUBY_PREFIX)
                {
                    ruby_buf.dump_into(buffer);
                    ruby_buf.@protected = true;
                }
                else if (first is not null)
                {
                    Utils.illegal_char_check(first.Value, line_number, warnChannel);
                    push_chars(escape_special_chars(first.Value));
                }
                else if (bufferItem is not null)
                {
                    var text = bufferItem?.to_html();
                    if (!string.IsNullOrEmpty(text))
                    {
                        foreach (var item in text!) Utils.illegal_char_check(item);
                        push_chars(escape_special_chars(bufferItem!));//kurema:Nullの可能性はないのに警告される。
                    }
                }
            }
        }

        public TextBuffer process()
        {
            try
            {
                parse();
            }
            catch (Exceptions.TerminateException)
            {
                return general_output_accent();
            }
            throw new Exception();//kurema:parse()から脱出する方法がないのでここには来ない。
        }
    }
}
