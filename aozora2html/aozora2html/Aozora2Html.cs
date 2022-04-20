using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Aozora
{
    // -*- coding:utf-8 -*-
    // 青空文庫形式のテキストファイルを html に整形する ruby スクリプト
    //require "extensions"
    //require "aozora2html/error"
    //require "jstream"
    //require "aozora2html/tag"
    //require "aozora2html/tag_parser"
    //require "aozora2html/accent_parser"
    //require "aozora2html/style_stack"
    //require "aozora2html/header"
    //require "aozora2html/ruby_buffer"
    //require "aozora2html/yaml_loader"
    //require "aozora2html/utils"

    //$gaiji_dir = "../../../gaiji/"

    //$css_files = Array["../../aozora.css"]

    // 変換器本体
    public class Aozora2Html
    {
        //kurema: C#では普通に出せるので下のコメントは無視してください。
        // 全角バックスラッシュが出せないから直打ち
        public const char KU = '／';
        public const char NOJI = '＼';
        public const char DAKUTEN = '″';
        public const char GAIJI_MARK = '※';
        public const char IGETA_MARK = '＃';
        public const char RUBY_BEGIN_MARK = '《';
        public const char RUBY_END_MARK = '》';
        public const char PAREN_BEGIN_MARK = '（';
        public const char PAREN_END_MARK = '）';
        public const char SIZE_SMALL = '小';
        public const char SIZE_MIDDLE = '中';
        public const char SIZE_LARGE = '大';
        public const string TEIHON_MARK = "底本：";
        public const char COMMAND_BEGIN = '［';
        public const char COMMAND_END = '］';
        public const char ACCENT_BEGIN = '〔';
        public const char ACCENT_END = '〕';
        public const string AOZORABUNKO = "青空文庫";
        //PAT_EDITOR = /[校訂|編|編集|編集校訂|校訂編集]$/
        private static Regex? _PAT_EDITOR = null;
        public static Regex PAT_EDITOR => _PAT_EDITOR ??= new Regex(@"(校訂|編|編集)$");
        private static Regex? _PAT_HENYAKU = null;
        public static Regex PAT_HENYAKU => _PAT_HENYAKU ??= new Regex(@"編訳$");
        private static Regex? _PAT_TRANSLATOR = null;
        public static Regex PAT_TRANSLATOR => _PAT_TRANSLATOR ??= new Regex(@"訳$");
        public const char RUBY_PREFIX = '｜';
        private static Regex? _PAT_RUBY = null;
        public static Regex PAT_RUBY => _PAT_RUBY ??= new Regex(@"《.*?》");
        private static Regex? _PAT_DIRECTION = null;
        public static Regex PAT_DIRECTION => _PAT_DIRECTION ??= new Regex(@"(右|左|上|下)に(.*)");
        private static Regex? _PAT_REF = null;
        public static Regex PAT_REF => _PAT_REF ??= new Regex(@"^「.+」");
        public const string CHUUKI_COMMAND = "注記付き";
        public const string TCY_COMMAND = "縦中横";
        public const string KEIGAKOMI_COMMAND = "罫囲み";
        public const string YOKOGUMI_COMMAND = "横組み";
        public const string CAPTION_COMMAND = "キャプション";
        public const string WARIGAKI_COMMAND = "割書";
        public const string KAERITEN_COMMAND = "返り点";
        public const string KUNTEN_OKURIGANA_COMMAND = "訓点送り仮名";
        public const string MIDASHI_COMMAND = "見出し";
        public const string OMIDASHI_COMMAND = "大見出し";
        public const string NAKAMIDASHI_COMMAND = "中見出し";
        public const string KOMIDASHI_COMMAND = "小見出し";
        public const string DOGYO_OMIDASHI_COMMAND = "同行大見出し";
        public const string DOGYO_NAKAMIDASHI_COMMAND = "同行中見出し";
        public const string DOGYO_KOMIDASHI_COMMAND = "同行小見出し";
        public const string MADO_OMIDASHI_COMMAND = "窓大見出し";
        public const string MADO_NAKAMIDASHI_COMMAND = "窓中見出し";
        public const string MADO_KOMIDASHI_COMMAND = "窓小見出し";
        public const char LEFT_MARK = '左';
        public const char UNDER_MARK = '下';
        public const char OVER_MARK = '上';
        public const string MAIN_MARK = "本文";
        public const string END_MARK = "終わり";
        public const char TEN_MARK = '点';
        public const char SEN_MARK = '線';
        public const string OPEN_MARK = "ここから";
        public const string CLOSE_MARK = "ここで";
        public const string MADE_MARK = "まで";
        public const string DOGYO_MARK = "同行";
        public const char MADO_MARK = '窓';
        public const string JIAGE_COMMAND = "字上げ";
        public const string JISAGE_COMMAND = "字下げ";
        public const string PHOTO_COMMAND = "写真";
        public const string ORIKAESHI_COMMAND = "折り返して";
        public const string ONELINE_COMMAND = "この行";
        public const string NON_0213_GAIJI = "非0213外字";
        public const string WARICHU_COMMAND = "割り注";
        public const string TENTSUKI_COMMAND = "天付き";
        private static Regex? _PAT_REST_NOTES = null;
        public static Regex PAT_REST_NOTES => _PAT_REST_NOTES ??= new Regex(@"(左|下)に「(.*)」の(ルビ|注記|傍記)");
        private static Regex? _PAT_KUTEN = null;
        public static Regex PAT_KUTEN => _PAT_KUTEN ??= new Regex(@"「※」[は|の]");
        private static Regex? _PAT_KUTEN_DUAL = null;
        public static Regex PAT_KUTEN_DUAL => _PAT_KUTEN_DUAL ??= new Regex(@"※.*※");
        private static Regex? _PAT_GAIJI = null;
        public static Regex PAT_GAIJI => _PAT_GAIJI ??= new Regex(@"(?:＃)(.*)(?:、)(.*)");
        private static Regex? _PAT_KAERITEN = null;
        public static Regex PAT_KAERITEN => _PAT_KAERITEN ??= new Regex(@"^([一二三四五六七八九十レ上中下甲乙丙丁天地人]+)$");
        private static Regex? _PAT_OKURIGANA = null;
        public static Regex PAT_OKURIGANA => _PAT_OKURIGANA ??= new Regex(@"^（(.+)）$");
        private static Regex? _PAT_REMOVE_OKURIGANA = null;
        public static Regex PAT_REMOVE_OKURIGANA => _PAT_REMOVE_OKURIGANA ??= new Regex(@"[（）]");
        private static Regex? _PAT_CHITSUKI = null;
        public static Regex PAT_CHITSUKI => _PAT_CHITSUKI ??= new Regex(@"(地付き|字上げ)(終わり)*$");
        private static Regex? _PAT_ORIKAESHI_JISAGE = null;
        public static Regex PAT_ORIKAESHI_JISAGE => _PAT_ORIKAESHI_JISAGE ??= new Regex(@"折り返して(\\d*)字下げ");
        private static Regex? _PAT_ORIKAESHI_JISAGE2 = null;
        public static Regex PAT_ORIKAESHI_JISAGE2 => _PAT_ORIKAESHI_JISAGE2 ??= new Regex(@"(\\d*)字下げ、折り返して(\\d*)字下げ");
        private static Regex? _PAT_JI_LEN = null;
        public static Regex PAT_JI_LEN => _PAT_JI_LEN ??= new Regex(@"([0-9]+)字");
        private static Regex? _PAT_INLINE_RUBY = null;
        public static Regex PAT_INLINE_RUBY => _PAT_INLINE_RUBY ??= new Regex(@"「(.*)」の注記付き");
        private static Regex? _PAT_IMAGE = null;
        public static Regex PAT_IMAGE => _PAT_IMAGE ??= new Regex(@"(.*)（(fig.+\\.png)(、横([0-9]+)×縦([0-9]+))*）入る");
        private static Regex? _PAT_FRONTREF = null;
        public static Regex PAT_FRONTREF => _PAT_FRONTREF ??= new Regex(@"「([^「」]*(?:「.+」)*[^「」]*)」[にはの](「.+」の)*(.+)");
        private static Regex? _PAT_RUBY_DIR = null;
        public static Regex PAT_RUBY_DIR => _PAT_RUBY_DIR ??= new Regex(@"(左|下)に「([^」]*)」の(ルビ|注記)");
        private static Regex? _PAT_CHUUKI = null;
        public static Regex PAT_CHUUKI => _PAT_CHUUKI ??= new Regex(@"「(.+?)」の注記");
        private static Regex? _PAT_BOUKI = null;
        public static Regex PAT_BOUKI => _PAT_BOUKI ??= new Regex(@"「(.)」の傍記");
        private static Regex? _PAT_CHARSIZE = null;
        public static Regex? PAT_CHARSIZE => _PAT_CHARSIZE ??= new Regex(@"(.*)段階(..)な文字");


        private static Regex? _REGEX_HIRAGANA = null;
        public static Regex REGEX_HIRAGANA => _REGEX_HIRAGANA ??= new Regex("[ぁ-んゝゞ]");
        private static Regex? _REGEX_KATAKANA = null;
        public static Regex REGEX_KATAKANA => _REGEX_KATAKANA ??= new Regex("[ァ-ンーヽヾヴ]");
        private static Regex? _REGEX_ZENKAKU = null;
        public static Regex REGEX_ZENKAKU => _REGEX_ZENKAKU ??= new Regex("[０-９Ａ-Ｚａ-ｚΑ-Ωα-ωА-Яа-я−＆’，．]");
        private static Regex? _REGEX_HANKAKU = null;
        public static Regex REGEX_HANKAKU => _REGEX_HANKAKU ??= new Regex("[A-Za-z0-9#\\-\\&'\\,]");
        private static Regex? _REGEX_KANJI = null;
        public static Regex REGEX_KANJI => _REGEX_KANJI ??= new Regex("[亜-熙々※仝〆〇ヶ]");

        public const string DYNAMIC_CONTENTS = "<div id=\"card\">\r\n<hr />\r\n<br />\r\n<a href=\"JavaScript:goLibCard();\" id=\"goAZLibCard\">●図書カード</a><script type=\"text/javascript\" src=\"../../contents.js\"></script>\r\n<script type=\"text/javascript\" src=\"../../golibcard.js\"></script>\r\n</div>";

        // KUNOJI = ["18e518f5"].pack("h*")
        // utf8 ["fecbf8fecbcb"].pack("h*")
        // DAKUTENKUNOJI = ["18e518d818f5"].pack("h*")
        // utf8 ["fecbf82e083bfecbcb"].pack("h*")

        //kurema:以下2行未実装
        //loader = Aozora2Html::YamlLoader.new(File.dirname(__FILE__))
        //ACCENT_TABLE = loader.load("../yml/accent_table.yml")

        // [class, tag]
        //kurema:以下2行未実装
        //COMMAND_TABLE = loader.load("../yml/command_table.yml")
        //JIS2UCS = loader.load("../yml/jis2ucs.yml")

        private static Encoding? _ShiftJis;
        public static Encoding ShiftJis => _ShiftJis ??= CodePagesEncodingProvider.Instance.GetEncoding("shift-jis", new EncoderReplacementFallback("〓"), new DecoderReplacementFallback("〓")) ?? throw new NullReferenceException();

        //kurema:以下はconvert_indent_type();がそのままToString();を返すので変更しないでください。
        public enum IndentTypeKey
        {
            jisage, chitsuki, midashi, jizume, yokogumi, keigakomi, caption, futoji, shatai, dai, sho,
            daisho//kurema:indent_stack用。
        }

        public static readonly System.Collections.ObjectModel.ReadOnlyDictionary<IndentTypeKey, string> INDENT_TYPE = new(new Dictionary<IndentTypeKey, string>()
        {
            {IndentTypeKey.jisage, "字下げ"},
            {IndentTypeKey.chitsuki, "地付き"},
            {IndentTypeKey.midashi, "見出し"},
            {IndentTypeKey.jizume, "字詰め"},
            {IndentTypeKey.yokogumi, "横組み"},
            {IndentTypeKey.keigakomi, "罫囲み"},
            {IndentTypeKey.caption, "キャプション"},
            {IndentTypeKey.futoji, "太字"},
            {IndentTypeKey.shatai, "斜体"},
            {IndentTypeKey.dai, "大きな文字"},
            {IndentTypeKey.sho, "小さな文字"},
        });

        public static readonly System.Collections.ObjectModel.ReadOnlyDictionary<int, string> DAKUTEN_KATAKANA_TABLE = new(new Dictionary<int, string>()
        {
            {2, "ワ゛"},
            {3, "ヰ゛"},
            {4, "ヱ゛"},
            {5, "ヲ゛"},
        });

        public enum chuuki_table_keys
        {
            chuki, kunoji, dakutenkunoji, newjis, accent,
        }

        protected Jstream stream;
        protected Helpers.IOutput @out;
        protected Helpers.TextBuffer buffer;
        protected Helpers.RubyBuffer ruby_buf;
        protected SectionKind section;//現在処理中のセクション(:head,:head_end,:chuuki,:chuuki_in,:body,:tail)
        protected Helpers.Header header;
        protected Helpers.StyleStack style_stack;//スタイルのスタック
        protected Dictionary<chuuki_table_keys, bool> chuuki_table;//最後にどの注記を出すかを保持しておく
        protected List<string> images;//使用した外字の画像保持用
        protected Stack<Helpers.IIndentStackItem> indent_stack;//基本はシンボルだが、ぶらさげのときはdivタグの文字列が入る
        protected Stack<string> tag_stack;
        protected Helpers.MidashiCounter midashi_counter;//見出しのカウンタ、見出しの種類によって増分が異なる
        protected bool terprip;//改行制御用 (terpriはLisp由来?)
        protected char? endchar = null;//解析終了文字、AccentParserやTagParserでは異なる
        protected bool noprint;//行末を読み込んだとき、何も出力しないかどうかのフラグ

        //kurema: 警告メッセージ用チャンネルを独自に追加しました。
        protected Helpers.IOutput warnChannel;

        //kurema:本来はstatic変数。しかし、parserに属した方が扱いやすいので移しました。
        public bool use_jisx0213_accent { get; set; } = false;
        public bool use_jisx0214_embed_gaiji { get; set; } = false;
        public bool use_unicode_embed_gaiji { get; set; } = false;

        protected string gaiji_dir;
        protected string[] css_files;

        public Aozora2Html(Jstream input, Helpers.IOutput output, Helpers.IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null)
        {
            stream = input;
            @out = output;
            buffer = new();
            ruby_buf = new();
            section = SectionKind.head;
            header = new(css_files);
            style_stack = new();
            chuuki_table = new();
            images = new();
            indent_stack = new();
            tag_stack = new();
            midashi_counter = new(0);
            terprip = true;
            noprint = false;//kurema:元は初期nil。falseで問題ないと思われる。
            this.warnChannel = warnChannel ?? new Helpers.OutputConsole();
            this.gaiji_dir = gaiji_dir ?? "../../../gaiji/";
            this.css_files = css_files ?? new[] { "../../aozora.css" };
        }

        public enum SectionKind
        {
            head, head_end, chuuki, chuuki_in, body, tail
        }

        public int line_number => stream.line;

        public bool block_allowed_context => style_stack.empty;

        //kurema:下を先に実装したので少し飛んでます。
        public int new_midashi_id(int size) => midashi_counter.generate_id(size);
        public int new_midashi_id(char size) => midashi_counter.generate_id(size);

        public Helpers.IBufferItem kuten2png(string substring)
        {
            var desc = PAT_KUTEN.Replace(substring, "");
            var matched = new Regex(@"([12])-(\d{1,2})-(\d{1,2})").Match("desc");
            if (matched.Success && desc != NON_0213_GAIJI && !PAT_KUTEN_DUAL.IsMatch(desc))
            {
                chuuki_table[chuuki_table_keys.newjis] = true;
                var codes = new int[] { int.Parse(matched.Groups[1].Value), int.Parse(matched.Groups[2].Value), int.Parse(matched.Groups[3].Value) };
                var folder = string.Format("{0,1}-{1:D2}", codes[0], codes[1]);//%1d-%02d
                var code = string.Format("{0,1}-{1:2d}-{2:2d}", codes[0], codes[1], codes[2]);//%1d-%02d-%02d
                return new Helpers.BufferItemTag(new Helpers.Tag.EmbedGaiji(this, folder, code, desc.Replace(IGETA_MARK.ToString(), ""), gaiji_dir));
            }
            else
            {
                return new Helpers.BufferItemString(substring);
            }
        }

        /// <summary>
        /// コマンド文字列からモードのシンボルを取り出す
        /// </summary>
        /// <param name="command"></param>
        /// <returns>[Symbol]</returns>
        public IndentTypeKey? detect_command_mode(string command)
        {
            //kurema:元は正規表現。途中でもマッチするはずなので==ではなくContains。
            if (command.Contains(INDENT_TYPE[IndentTypeKey.chitsuki] + END_MARK) || command.Contains(JISAGE_COMMAND + END_MARK))
            {
                return IndentTypeKey.chitsuki;
            }

            foreach (var keyValue in INDENT_TYPE)
            {
                if (command.Contains(keyValue.Value)) return keyValue.Key;
            }
            return null;
        }

        /// <summary>
        /// 一文字読み込む
        /// </summary>
        /// <returns></returns>
        protected char? read_char() => stream.read_char();

        //一行読み込む
        protected string? read_line() => stream.read_line();

        protected char read_accent()
        {
            throw new NotImplementedException();
            //return new Helpers.AccentParser(stream, ACCENT_END, chuuki_table, images, @out, warnChannel, gaiji_dir, css_files).process;
        }

        protected Helpers.TagParser read_to_nest(char? endchar)
        {
            throw new NotImplementedException();
            //return new Helpers.TagParser(stream, endchar, chuuki_table, images, @out, gaiji_dir: gaiji_dir).process();
        }

        protected void finalize()
        {
            throw new NotImplementedException();
            //hyoki();
            dynamic_contents();
            @out.print("</body>\r\n</html>\r\n");
        }

        protected void dynamic_contents()
        {
            @out.print(DYNAMIC_CONTENTS);
        }

        protected void close()
        {
            stream.close();
            @out.close();
        }

        /// <summary>
        /// 記法のシンボル名から文字列へ変換する
        /// シンボルが見つからなければそのまま返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string convert_indent_type(Helpers.IIndentStackItem type)
        {
            switch (type)
            {
                case Helpers.IndentStackItemString typeString:
                    return typeString.Content;
                case Helpers.IndentStackItemIndentTypeKey typeIndentTypeKey:
                    return convert_indent_type(typeIndentTypeKey.Content);
                default:
                    throw new Exception();
            }
        }

        protected string convert_indent_type(IndentTypeKey type)
        {
            return INDENT_TYPE.ContainsKey(type) ? INDENT_TYPE[type] : type.ToString();
        }

        protected string? check_close_match(IndentTypeKey type)
        {
            IndentTypeKey ind;
            if (indent_stack.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (indent_stack.Last() is Helpers.IndentStackItemString)
            {
                noprint = true;
                ind = IndentTypeKey.jisage;
            }
            else if (indent_stack.Last() is Helpers.IndentStackItemIndentTypeKey lastKey)
            {
                ind = lastKey.Content;
            }
            else
            {
                throw new Exception($"Unexpected type for {nameof(indent_stack)} item.");
            }
            if (ind == type) return null;
            else return convert_indent_type(ind);
        }

        public void implicit_close(IndentTypeKey type)
        {
            if (indent_stack.Count == 0) return;

            if (check_close_match(type) != null)
            {
                //ok, nested multiline tags, go ahead
            }
            else
            {
                //not nested, please close
                indent_stack.Pop();
                if (tag_stack.Count > 0)
                {
                    var tag = tag_stack.Pop();
                    push_chars(tag);
                }
            }
        }

        /// <summary>
        /// 本文が終わってよいかチェックし、終わっていなければ例外をあげる
        /// </summary>
        public void ensure_close()
        {
            var n = indent_stack.LastOrDefault();
            if (n is null) return;

            throw new Exceptions.TerminateInStyleException(convert_indent_type(n));
        }

        //kurema:
        //使うので先に実装。
        //C#なので元とは結構違う実装方法。何が来るかまだ調査不足だけど後で良い。
        public void push_chars(string text)
        {
            foreach (var item in text) push_char(item);
        }

        public void push_chars(Helpers.IBufferItem item)
        {
            push_chars(item.to_html());
        }

        public void push_chars(IEnumerable<Helpers.IBufferItem> bufferItems)
        {
            foreach (var item in bufferItems) push_chars(item);
        }

        public void push_char(char @char)
        {
            ruby_buf.push_char(@char, buffer);
        }

        public void explicit_close(IndentTypeKey type)
        {
            var n = check_close_match(type);
            if (n is not null) throw new Exceptions.InvalidClosingException(n);

            if (tag_stack.Count == 0) return;
            var tag = tag_stack.Pop();
            push_chars(tag);
        }

        public void parse()
        {
            throw new NotImplementedException();
        }

        public void judge_chuuki()
        {
            //注記が入るかどうかチェック
            int i = 0;
            while (true)
            {
                switch (stream.peek_char(i))
                {
                    case '-': i++; break;
                    case '\n':
                        section = i == 0 ? SectionKind.body : SectionKind.chuuki;
                        return;
                    default:
                        section = SectionKind.body;
                        @out.print("<br />\r\n");
                        return;
                }
            }
        }

        public void parse_header()
        {
            var @string = read_line();
            // refine from Tomita 09/06/14
            if (string.IsNullOrEmpty(@string))
            {
                //空行がくれば、そこでヘッダー終了とみなす
                section = SectionKind.head_end;
                @out.print(header.to_html());
            }
            else
            {
                @string = @string!.Replace(RUBY_PREFIX.ToString(), string.Empty);
                @string = PAT_RUBY.Replace(@string, string.Empty);
                header.push(@string);
            }
        }

        public void parse_chuuki()
        {
            var @string = read_line();
            if (!Regex.IsMatch(@string, @"^-+$")) return;

            switch (section)
            {
                case SectionKind.chuuki:
                    section = SectionKind.chuuki_in;
                    break;
                case SectionKind.body:
                    section = SectionKind.body;
                    break;
            }
        }

        /// <summary>
        /// 本体解析部
        /// 
        /// 1文字ずつ読み込み、dispatchして@buffer,@ruby_bufへしまう
        /// 改行コードに当たったら溜め込んだものをgeneral_outputする
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        public void parse_body()
        {
            var @char = read_char();
            bool check = true;

            switch (@char)
            {
                case ACCENT_BEGIN:
                    check = false;
                    @char = read_accent();
                    break;
                case GAIJI_MARK:
                //@char = dispatch_gaiji();
                //break;
                case COMMAND_BEGIN:
                //@char = dispatch_aozora_command();
                //break;
                case KU:
                //assign_kunoji();
                //break;
                case RUBY_BEGIN_MARK:
                //@char = apply_ruby();
                //break;
                default:
                    //kurema:TEIHON_MARK[0]は定数じゃないので普通に条件分岐で。
                    if (@char == TEIHON_MARK[0])
                    {
                        //if (buffer.Count == 0) ending_check();
                    }
                    break;
            }

            switch (@char)
            {
                case '\n':
                    //general_output();
                    break;
                case RUBY_PREFIX:
                    ruby_buf.dump_into(buffer);
                    ruby_buf.@protected = true;
                    break;
                case null:
                    //noop
                    break;
                default:
                    if (@char == endchar)
                    {
                        //suddenly finished the file
                        warnChannel.print(string.Format(Helpers.I18n.MSG["warn_unexpected_terminator"], line_number));
                        throw new Exception("terminate");//kurema:要修正。そもそも例外で大域脱出したくない。
                    }
                    if (check)
                    {
                        Helpers.Utils.illegal_char_check(@char.Value, line_number, warnChannel);
                    }
                    push_chars(escape_special_chars(@char.Value));
                    break;
            }
            throw new NotImplementedException();
        }

        //Original Aozora2Html#push_chars does not convert "'" into '&#39;'; it's old behaivor of CGI.escapeHTML().
        public string escape_special_chars(string text)
        {
            return Regex.Replace(text, @"[&"" <>]", a => escape_special_chars(a.Value[0]));
        }

        public string escape_special_chars(char @char)
        {
            switch (@char)
            {
                case '&': return "&amp;";
                case '"': return "&quot;";
                case '<': return "&lt;";
                case '>': return "&gt;";
                default: return @char.ToString();
            }
        }
    }
}
