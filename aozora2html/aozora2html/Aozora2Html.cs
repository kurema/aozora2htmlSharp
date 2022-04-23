using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Aozora.Helpers;

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

        //kurema:.NET 7以降では[RegexGenerator]が使えるようになります。ただし移行するとUWPとかから使えなくなります。困る。
        private static Regex? _PAT_EDITOR = null;
        public static Regex PAT_EDITOR => _PAT_EDITOR ??= new Regex(@"(校訂|編|編集)$", RegexOptions.Compiled);
        private static Regex? _PAT_HENYAKU = null;
        public static Regex PAT_HENYAKU => _PAT_HENYAKU ??= new Regex(@"編訳$", RegexOptions.Compiled);
        private static Regex? _PAT_TRANSLATOR = null;
        public static Regex PAT_TRANSLATOR => _PAT_TRANSLATOR ??= new Regex(@"訳$", RegexOptions.Compiled);
        public const char RUBY_PREFIX = '｜';
        private static Regex? _PAT_RUBY = null;
        public static Regex PAT_RUBY => _PAT_RUBY ??= new Regex(@"《.*?》", RegexOptions.Compiled);
        private static Regex? _PAT_DIRECTION = null;
        public static Regex PAT_DIRECTION => _PAT_DIRECTION ??= new Regex(@"(右|左|上|下)に(.*)", RegexOptions.Compiled);
        private static Regex? _PAT_REF = null;
        public static Regex PAT_REF => _PAT_REF ??= new Regex(@"^「.+」", RegexOptions.Compiled);
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
        public static Regex PAT_REST_NOTES => _PAT_REST_NOTES ??= new Regex(@"(左|下)に「(.*)」の(ルビ|注記|傍記)", RegexOptions.Compiled);
        private static Regex? _PAT_KUTEN = null;
        public static Regex PAT_KUTEN => _PAT_KUTEN ??= new Regex(@"「※」[は|の]", RegexOptions.Compiled);
        private static Regex? _PAT_KUTEN_DUAL = null;
        public static Regex PAT_KUTEN_DUAL => _PAT_KUTEN_DUAL ??= new Regex(@"※.*※", RegexOptions.Compiled);
        private static Regex? _PAT_GAIJI = null;
        public static Regex PAT_GAIJI => _PAT_GAIJI ??= new Regex(@"(?:＃)(.*)(?:、)(.*)", RegexOptions.Compiled);
        private static Regex? _PAT_KAERITEN = null;
        public static Regex PAT_KAERITEN => _PAT_KAERITEN ??= new Regex(@"^([一二三四五六七八九十レ上中下甲乙丙丁天地人]+)$", RegexOptions.Compiled);
        private static Regex? _PAT_OKURIGANA = null;
        public static Regex PAT_OKURIGANA => _PAT_OKURIGANA ??= new Regex(@"^（(.+)）$", RegexOptions.Compiled);
        private static Regex? _PAT_REMOVE_OKURIGANA = null;
        public static Regex PAT_REMOVE_OKURIGANA => _PAT_REMOVE_OKURIGANA ??= new Regex(@"[（）]", RegexOptions.Compiled);
        private static Regex? _PAT_CHITSUKI = null;
        public static Regex PAT_CHITSUKI => _PAT_CHITSUKI ??= new Regex(@"(地付き|字上げ)(終わり)*$", RegexOptions.Compiled);
        private static Regex? _PAT_ORIKAESHI_JISAGE = null;
        public static Regex PAT_ORIKAESHI_JISAGE => _PAT_ORIKAESHI_JISAGE ??= new Regex(@"折り返して(\\d*)字下げ", RegexOptions.Compiled);
        private static Regex? _PAT_ORIKAESHI_JISAGE2 = null;
        public static Regex PAT_ORIKAESHI_JISAGE2 => _PAT_ORIKAESHI_JISAGE2 ??= new Regex(@"(\\d*)字下げ、折り返して(\\d*)字下げ", RegexOptions.Compiled);
        private static Regex? _PAT_JI_LEN = null;
        public static Regex PAT_JI_LEN => _PAT_JI_LEN ??= new Regex(@"([0-9]+)字", RegexOptions.Compiled);
        private static Regex? _PAT_INLINE_RUBY = null;
        public static Regex PAT_INLINE_RUBY => _PAT_INLINE_RUBY ??= new Regex(@"「(.*)」の注記付き", RegexOptions.Compiled);
        private static Regex? _PAT_IMAGE = null;
        public static Regex PAT_IMAGE => _PAT_IMAGE ??= new Regex(@"(.*)（(fig.+\\.png)(、横([0-9]+)×縦([0-9]+))*）入る", RegexOptions.Compiled);
        private static Regex? _PAT_FRONTREF = null;
        public static Regex PAT_FRONTREF => _PAT_FRONTREF ??= new Regex(@"「([^「」]*(?:「.+」)*[^「」]*)」[にはの](「.+」の)*(.+)", RegexOptions.Compiled);
        private static Regex? _PAT_RUBY_DIR = null;
        public static Regex PAT_RUBY_DIR => _PAT_RUBY_DIR ??= new Regex(@"(左|下)に「([^」]*)」の(ルビ|注記)", RegexOptions.Compiled);
        private static Regex? _PAT_CHUUKI = null;
        public static Regex PAT_CHUUKI => _PAT_CHUUKI ??= new Regex(@"「(.+?)」の注記", RegexOptions.Compiled);
        private static Regex? _PAT_BOUKI = null;
        public static Regex PAT_BOUKI => _PAT_BOUKI ??= new Regex(@"「(.)」の傍記", RegexOptions.Compiled);
        private static Regex? _PAT_CHARSIZE = null;
        public static Regex PAT_CHARSIZE => _PAT_CHARSIZE ??= new Regex(@"(.*)段階(..)な文字", RegexOptions.Compiled);


        private static Regex? _REGEX_HIRAGANA = null;
        public static Regex REGEX_HIRAGANA => _REGEX_HIRAGANA ??= new Regex("[ぁ-んゝゞ]", RegexOptions.Compiled);
        private static Regex? _REGEX_KATAKANA = null;
        public static Regex REGEX_KATAKANA => _REGEX_KATAKANA ??= new Regex("[ァ-ンーヽヾヴ]", RegexOptions.Compiled);
        private static Regex? _REGEX_ZENKAKU = null;
        public static Regex REGEX_ZENKAKU => _REGEX_ZENKAKU ??= new Regex("[０-９Ａ-Ｚａ-ｚΑ-Ωα-ωА-Яа-я−＆’，．]", RegexOptions.Compiled);
        private static Regex? _REGEX_HANKAKU = null;
        public static Regex REGEX_HANKAKU => _REGEX_HANKAKU ??= new Regex("[A-Za-z0-9#\\-\\&'\\,]", RegexOptions.Compiled);
        private static Regex? _REGEX_KANJI = null;

        //kurema:https://dobon.net/vb/dotnet/string/ishiragana.html
        //public static Regex REGEX_KANJI => _REGEX_KANJI ??= new Regex("[亜-熙々※仝〆〇ヶ]");//kurema:これは流石にUnicodeでは怪しい。
        public static Regex REGEX_KANJI => _REGEX_KANJI ??= new Regex(
            @"[々※仝〆〇ヶ\p{IsCJKUnifiedIdeographs}\p{IsCJKCompatibilityIdeographs}\p{IsCJKUnifiedIdeographsExtensionA}]|" +
            @"[\uD840-\uD869][\uDC00-\uDFFF]|\uD869[\uDC00-\uDEDF]", RegexOptions.Compiled);//kurema:JIS文字だけならここまでは必要ない。


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
        protected IOutput @out;
        protected TextBuffer buffer;
        protected RubyBuffer ruby_buf;
        protected SectionKind section;//現在処理中のセクション(:head,:head_end,:chuuki,:chuuki_in,:body,:tail)
        protected Header header;
        protected StyleStack style_stack;//スタイルのスタック
        protected Dictionary<chuuki_table_keys, bool> chuuki_table;//最後にどの注記を出すかを保持しておく
        protected List<string> images;//使用した外字の画像保持用
        protected Stack<IIndentStackItem> indent_stack;//基本はシンボルだが、ぶらさげのときはdivタグの文字列が入る
        protected Stack<string> tag_stack;
        protected MidashiCounter midashi_counter;//見出しのカウンタ、見出しの種類によって増分が異なる
        protected bool terprip;//改行制御用 (terpriはLisp由来?)
        protected char? endchar = null;//解析終了文字、AccentParserやTagParserでは異なる
        protected bool noprint;//行末を読み込んだとき、何も出力しないかどうかのフラグ

        //kurema: 警告メッセージ用チャンネルを独自に追加しました。
        protected IOutput warnChannel;

        //kurema:本来はstatic変数。しかし、parserに属した方が扱いやすいので移しました。
        public bool use_jisx0213_accent { get; set; } = false;
        public bool use_jisx0214_embed_gaiji { get; set; } = false;
        public bool use_unicode_embed_gaiji { get; set; } = false;

        protected string gaiji_dir;
        protected string[] css_files;

        public Aozora2Html(Jstream input, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null)
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
            this.warnChannel = warnChannel ?? new OutputConsole();
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
        public int new_midashi_id(string size) => midashi_counter.generate_id(size);

        public IBufferItem kuten2png(string substring)
        {
            var desc = PAT_KUTEN.Replace(substring, "");
            var matched = new Regex(@"([12])-(\d{1,2})-(\d{1,2})").Match("desc");
            if (matched.Success && desc != NON_0213_GAIJI && !PAT_KUTEN_DUAL.IsMatch(desc))
            {
                chuuki_table[chuuki_table_keys.newjis] = true;
                var codes = new int[] { int.Parse(matched.Groups[1].Value), int.Parse(matched.Groups[2].Value), int.Parse(matched.Groups[3].Value) };
                var folder = string.Format("{0,1}-{1:D2}", codes[0], codes[1]);//%1d-%02d
                var code = string.Format("{0,1}-{1:2d}-{2:2d}", codes[0], codes[1], codes[2]);//%1d-%02d-%02d
                return new BufferItemTag(new Helpers.Tag.EmbedGaiji(this, folder, code, desc.Replace(new string(IGETA_MARK, 1), ""), gaiji_dir));
            }
            else
            {
                return new BufferItemString(substring);
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

        protected (string, string) read_to_nest(char? endchar)
        {
            throw new NotImplementedException();
            //return new Helpers.TagParser(stream, endchar, chuuki_table, images, @out, gaiji_dir: gaiji_dir).process();
        }

        protected void finalize()
        {
            hyoki();
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
        protected string convert_indent_type(IIndentStackItem type) => type switch
        {
            IndentStackItemString typeString => typeString.Content,
            IndentStackItemIndentTypeKey typeIndentTypeKey => convert_indent_type(typeIndentTypeKey.Content),
            _ => throw new Exception(),
        };

        protected string convert_indent_type(IndentTypeKey type) => INDENT_TYPE.ContainsKey(type) ? INDENT_TYPE[type] : type.ToString();

        protected string? check_close_match(IndentTypeKey type)
        {
            IndentTypeKey ind;
            if (indent_stack.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (indent_stack.Last() is IndentStackItemString)
            {
                noprint = true;
                ind = IndentTypeKey.jisage;
            }
            else if (indent_stack.Last() is IndentStackItemIndentTypeKey lastKey)
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

        public bool implicit_close(IndentTypeKey type)
        {
            //kurema:apply_burasage()を見ると返り値bool扱いが良いっぽいけど、かなり妙。
            if (indent_stack.Count == 0) return false;

            if (check_close_match(type) != null)
            {
                //ok, nested multiline tags, go ahead
                return false;
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
                return true;
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

        public void explicit_close(IndentTypeKey type)
        {
            var n = check_close_match(type);
            if (n is not null) throw new Exceptions.InvalidClosingException(n);

            if (tag_stack.Count == 0) return;
            var tag = tag_stack.Pop();
            push_chars(tag);
        }

        /// <summary>
        /// main loop
        /// </summary>
        /// <exception cref="Exceptions.EncountUndefinedConditionException"></exception>
        public virtual void parse()
        {
            while (true)
            {
                switch (section)
                {
                    case SectionKind.head:
                        parse_header();
                        break;
                    case SectionKind.head_end:
                        judge_chuuki();
                        break;
                    case SectionKind.chuuki:
                    case SectionKind.chuuki_in:
                        parse_chuuki();
                        break;
                    case SectionKind.body:
                        parse_body();
                        break;
                    case SectionKind.tail:
                        parse_tail();
                        break;
                    default:
                        throw new Exceptions.EncountUndefinedConditionException();
                }
            }
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
                @string = @string!.Replace(new string(RUBY_PREFIX, 1), string.Empty);
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
        /// <exception cref="Exception"></exception>
        public void parse_body()
        {
            object? @char = read_char();
            bool check = true;

            switch (@char)
            {
                case ACCENT_BEGIN:
                    check = false;
                    @char = read_accent();
                    break;
                case GAIJI_MARK:
                    @char = dispatch_gaiji();
                    break;
                case COMMAND_BEGIN:
                    @char = dispatch_aozora_command();
                    break;
                case KU:
                    assign_kunoji();
                    break;
                case RUBY_BEGIN_MARK:
                    @char = apply_ruby();
                    break;
                default:
                    //kurema:TEIHON_MARK[0]は定数じゃないので普通に条件分岐で。
                    if ((char?)@char == TEIHON_MARK[0])
                    {
                        if (buffer.Count == 0) ending_check();
                    }
                    break;
            }

            if (@char is IBufferItem bufferItem) @char = bufferItem.to_html();
            if (@char is string s)
            {
                if (s.Length == 0) @char = null;
                if (s.Length == 1) @char = s[0];
            }

            switch (@char)
            {
                case '\n':
                    general_output();
                    break;
                case RUBY_PREFIX:
                    ruby_buf.dump_into(buffer);
                    ruby_buf.@protected = true;
                    break;
                case null:
                    //noop
                    break;
                default:
                    if ((@char as char?) == endchar)
                    {
                        //suddenly finished the file
                        warnChannel.print(string.Format(I18n.MSG["warn_unexpected_terminator"], line_number));
                        throw new Exceptions.TerminateException();//kurema:例外で大域脱出したくない…。
                    }

                    if (@char is char charChar)
                    {
                        if (check) Utils.illegal_char_check(charChar, line_number, warnChannel);
                        push_chars(escape_special_chars(new string(charChar, 1)));
                        break;
                    }
                    if (@char is string charString)
                    {
                        if (check) foreach (var charItem in charString) Utils.illegal_char_check(charItem, line_number, warnChannel);
                        push_chars(escape_special_chars(charString));
                    }
                    break;
            }
        }

        /// <summary>
        /// 本文が終了したかどうかチェックする
        /// </summary>
        public void ending_check()
        {
            //`底本：`でフッタ(:tail)に遷移
            if (stream.peek_char(0) != TEIHON_MARK[1] || stream.peek_char(1) != TEIHON_MARK[2]) return;

            section = SectionKind.tail;
            ensure_close();
            @out.print("</div>\r\n<div class=\"bibliographical_information\">\r\n<hr />\r\n<br />\r\n");
        }

        //kurema:
        //C#なので元とは結構違う実装方法。何が来るかまだ調査不足だけど後で良い。
        public void push_chars(string text)
        {
            foreach (var item in text) push_char(item);
        }

        public void push_chars(IBufferItem item)
        {
            push_chars(item.to_html());
        }

        public void push_chars(IEnumerable<IBufferItem> bufferItems)
        {
            foreach (var item in bufferItems) push_chars(item);
        }

        public void push_char(char @char)
        {
            ruby_buf.push_char(@char, buffer);
        }

        /// <summary>
        /// 読み込んだ行の出力を行う
        /// 
        /// parserが改行文字を読み込んだら呼ばれる。
        /// 最終的に@ruby_bufと@bufferは初期化する
        /// </summary>
        /// <exception cref="Exceptions.DontCrlfInStyleException"></exception>
        //@return [void]
        public void general_output()
        {
            if (style_stack.last() is not null)
            {
                throw new Exceptions.DontCrlfInStyleException(style_stack.last_command() ?? "");
            }

            //bufferにインデントタグだけがあったら改行しない！
            if (noprint)
            {
                noprint = false;
                return;
            }
            ruby_buf.dump_into(buffer);
            var buf = buffer;
            buffer = new TextBuffer();
            var tail = new List<string>();

            var indent_type = buf.blank_type();
            var terpripLocal = buf.terpri() && terprip;
            terprip = true;

            if (indent_stack.LastOrDefault() is not null and IndentStackItemString lastString && indent_type == TextBuffer.blank_type_result.@false)//kurema:indentの場合は含まない？
            {
                @out.print(lastString.Content);
            }

            foreach (var s in buf)
            {
                if (s is BufferItemTag stag)
                {
                    if (stag.tag is Helpers.Tag.IOnelineIndent stagOI)
                    {
                        tail.Insert(0, stagOI.close_tag());
                    }
                    else if (stag.tag is Helpers.Tag.UnEmbedGaiji stagUEG && !stagUEG.escaped)
                    {
                        //消してあった※を復活させて
                        @out.print(new string(GAIJI_MARK, 1));
                    }
                }
                @out.print(s.to_html());
            }

            //最後はCRLFを出力する
            if (indent_stack.LastOrDefault() is BufferItemString)
            {
                //ぶら下げindent
                //tail always active

                //kurema:
                //元は
                //@out.print tail.map(&:to_s).join
                //to_s必要？
                @out.print(string.Join("", tail?.ToArray() ?? new string[0]));
                if (indent_type == TextBuffer.blank_type_result.inline) @out.print("\r\n");
                else if (indent_type == TextBuffer.blank_type_result.@true && terpripLocal) @out.print("<br />\r\n");
                else @out.print("</div>\r\n");
            }
            else if (tail.Count == 0 && terpripLocal)
            {
                @out.print("<br />\r\n");
            }
            else
            {
                @out.print(string.Join("", tail?.ToArray() ?? new string[0]));
                @out.print("\r\n");
            }
        }

        /// <summary>
        /// 前方参照の発見 Ruby,style重ねがけ等々のため、要素の配列で返す
        /// 
        /// 前方参照は`○○［＃「○○」に傍点］`、`吹喋［＃「喋」に「ママ」の注記］`といった表記
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        //@return [TextBuffer|false]
        //kurema:元々はfalseを返していた場面でnullを返しています。タプルにするのも変なので。
        public TextBuffer? search_front_reference(string @string)
        {
            if (string.IsNullOrEmpty(@string)) return null;
            IList<IBufferItem> searching_buf = ruby_buf.present ? ruby_buf : buffer;
            var last_string = searching_buf.LastOrDefault();
            switch (last_string)
            {
                case BufferItemString last_string_string:
                    if (last_string_string.Length == 0)
                    {
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        return search_front_reference(@string);
                    }
                    else if (last_string_string.to_html().EndsWith(@string))
                    //else if (new Regex($"{Regex.Escape(@string)}$").IsMatch(last_string_string.to_html()))
                    //kurema:
                    //元はコレだけどあってる？
                    //elsif last_string.match?(Regexp.new("#{Regexp.quote(string)}$"))
                    {
                        //完全一致
                        //start = match.begin(0)
                        //tail = match.end(0)
                        //last_string[start,tail-start] = ""
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        searching_buf.Add(new BufferItemString(new Regex($"{Regex.Escape(@string)}$").Replace(last_string_string.to_html(), "")));
                        return new TextBuffer(@string);
                    }
                    else if (@string.EndsWith(last_string_string.to_html()))
                    {
                        //部分一致
                        //kurema:元は再帰対策でlast_stringと同じものをtmpに置いてたっぽい。
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        var found = search_front_reference(new Regex($"{Regex.Escape(last_string_string.to_html())}$").Replace(@string, ""));//kurema:不安
                        if (found != null)
                        {
                            found.Add(last_string);
                            return found;
                        }
                        else
                        {
                            searching_buf.Add(last_string);
                            return null;
                        }
                    }
                    break;
                case BufferItemTag last_string_tag when last_string_tag.tag is Helpers.Tag.ReferenceMentioned referenceMentioned:
                    var inner = referenceMentioned.target_string;
                    if (inner == @string)
                    {
                        //完全一致
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        return new TextBuffer(new[] { last_string_tag });
                    }
                    else if (@string.EndsWith(inner))
                    {
                        //部分一致
                        var found = search_front_reference(new Regex($"{Regex.Escape(inner)}$").Replace(@string, ""));
                        if (found != null)
                        {
                            found.Add(last_string);
                            return found;
                        }
                        else
                        {
                            searching_buf.Add(last_string);
                            return null;
                        }

                    }
                    break;
                default: return null;
            }
            return null;
        }

        /// <summary>
        /// 発見した前方参照を元に戻す
        /// 
        /// @ruby_bufがあれば@ruby_bufに、なければ@bufferにpushする
        /// バッファの最後と各要素が文字列ならconcatし、どちらが文字列でなければ（concatできないので）pushする
        /// </summary>
        /// <param name="reference"></param>
        //@return [void]
        public void recovery_front_reference(TextBuffer reference)
        {
            foreach (var elt in reference)
            {
                //if @ruby_buf.protected
                if (ruby_buf.present)
                {
                    ruby_buf.Add(elt);
                }
                else if (buffer.LastOrDefault() is BufferItemString buffer_last_string)
                {
                    if (elt is BufferItemString elt_string)
                    {
                        buffer_last_string.Append(elt_string.to_html());
                    }
                    else
                    {
                        buffer.Add(elt);
                    }
                }
                else //rubocop:disable Lint/DuplicateBranch
                {
                    ruby_buf.Add(elt);
                }
            }
        }

        public Helpers.Tag.UnEmbedGaiji? escape_gaiji(string command)
        {
            var match = PAT_GAIJI.Match(command);
            if (!match.Success || match.Groups.Count < 3) return null;
            var kanji = match.Groups[1].Value;
            var line = match.Groups[2].Value;

            var tmp = images.FirstOrDefault(a => a.StartsWith(kanji));
            var index = images.IndexOf(tmp);
            if (tmp is not null)
            {
                images[index] += line;
            }
            else
            {
                images.Add(kanji + line);
            }
            return new Helpers.Tag.UnEmbedGaiji(command);
        }

        public IBufferItem dispatch_gaiji()
        {
            //「※」の次が「［」でなければ外字ではない
            if (stream.peek_char(0) != COMMAND_BEGIN) return new BufferItemString(new string( GAIJI_MARK,1));

            //「［」を読み捨てる
            read_char();
            //embed?
            var (command, _) = read_to_nest(COMMAND_END);
            var try_emb = kuten2png(command);
            if (try_emb is BufferItemString try_emb_string && try_emb_string.to_html() == command) return new BufferItemString(try_emb_string.to_html());

            var matched = Regex.Match(command, @"U\+([0-9A-F]{4,5})");
            if (matched.Success && use_unicode_embed_gaiji)
            {
                var unicode_num = matched.Groups[1].Value;
                return new BufferItemTag(new Helpers.Tag.EmbedGaiji(this, null, null, command, gaiji_dir, unicode_num));
            }
            else
            {
                //Unemb
                return new BufferItemTag(escape_gaiji(command) ?? throw new ArgumentNullException());
            }
        }

        /// <summary>
        /// 注記記法の場合分け
        /// </summary>
        public IBufferItem? dispatch_aozora_command()
        {
            //「［」の次が「＃」でなければ注記ではない
            if (stream.peek_char(0) != IGETA_MARK)
            {
                return new BufferItemString(new string(COMMAND_BEGIN, 1));
            }
            //「＃」を読み捨てる
            read_char();
            var (command, raw) = read_to_nest(COMMAND_END);
            //適用順序はこれで大丈夫か？　誤爆怖いよ誤爆

            IBufferItem? GetReturnValue(Helpers.Tag.Tag? tag)
            {
                return tag is null ? null : new BufferItemTag(tag);
            }

            if (command.Contains(ORIKAESHI_COMMAND))
            {
                apply_burasage(command);
                return null;
            }
            else if (command.StartsWith(OPEN_MARK))
            {
                return GetReturnValue(exec_block_start_command(command));
            }
            else if (command.StartsWith(CLOSE_MARK))
            {
                return GetReturnValue(exec_block_end_command(command));
            }
            else if (command.Contains(WARICHU_COMMAND))
            {
                apply_warichu(command);
                return null;
            }
            else if (command.Contains(JISAGE_COMMAND))
            {
                return GetReturnValue(apply_jisage(command));
            }
            else if (Regex.IsMatch(command, @"fig(\d)+_(\d)+\.png"))
            {
                return GetReturnValue(exec_img_command(command, raw));
            }//avoid to try complex ruby -- escape to notes
            else if (PAT_REST_NOTES.IsMatch(command))
            {
                return GetReturnValue(apply_rest_notes(command));
            }
            else if (command.EndsWith(END_MARK))
            {
                exec_inline_end_command(command);
                return null;
            }
            else if (Regex.IsMatch(command, @"1-7-8[2345]"))//kurema:正規表現を二度実行するのは微妙。
            {
                return GetReturnValue(apply_dakuten_katakana(command));
            }
            else if (PAT_KAERITEN.IsMatch(command))
            {
                return new BufferItemTag(new Helpers.Tag.Kaeriten(command));
            }
            else if (PAT_OKURIGANA.IsMatch(command))
            {
                return new BufferItemTag(new Helpers.Tag.Okurigana(PAT_OKURIGANA.Replace(command, "")));
            }
            else if (PAT_CHITSUKI.IsMatch(command))
            {
                return GetReturnValue(apply_chitsuki(command));
            }
            else if (exec_inline_start_command(command))
            {
                return null;
            }
            else//rubocop:disable Lint/DuplicateBranch
            {
                return GetReturnValue(apply_rest_notes(command));
            }
        }

        public void apply_burasage(string command)
        {
            if (implicit_close(IndentTypeKey.jisage))//kurema:implicit_close()は別にbool返してないっぽいけど…
            {
                terprip = false;
                general_output();
            }
            noprint = true; //always no print
            command = Utils.convert_japanese_number(command);
            string? tag;
            if (command.Contains(TENTSUKI_COMMAND))
            {
                var matched = PAT_ORIKAESHI_JISAGE.Match(command);
                if (!matched.Success || matched.Groups.Count < 2) throw new Exception();
                var width = matched.Groups[1].Value;
                tag = $"<div class=\"burasage\" style=\"margin-left: {width}em; text-indent: -{width}em;\">";
            }
            else
            {
                var matched = PAT_ORIKAESHI_JISAGE2.Match(command);
                if (!matched.Success || matched.Groups.Count < 3) throw new Exception();
                var (left, indent) = (matched.Groups[1].Value, matched.Groups[2].Value);
                var left2 = int.Parse(left) - int.Parse(indent);
                tag = $"<div class=\"burasage\" style=\"margin-left: {indent}em; text-indent: {left2}em;\">";
            }
            indent_stack.Push(new IndentStackItemString(tag));
            tag_stack.Push(string.Empty); //dummy
        }

        public int? jisage_width(string command)
        {
            var matched = new Regex(@$"(\d*)(?:{JISAGE_COMMAND})").Match(Utils.convert_japanese_number(command));
            if (!matched.Success || matched.Groups.Count < 1) return null;
            if (int.TryParse(matched.Groups[1].Value, out int num))
            {
                return num;
            }
            return null;
        }

        public Helpers.Tag.MultilineJisage? apply_jisage(string command)
        {
            if (command.Contains(MADE_MARK) | command.Contains(END_MARK))
            {
                //字下げ終わり
                explicit_close(IndentTypeKey.jisage);
                indent_stack.Pop();
                return null;
            }
            else if (command.Contains(ONELINE_COMMAND))
            {
                //1行だけ
                buffer.Insert(0, new BufferItemTag(new Helpers.Tag.OnelineJisage(this, jisage_width(command) ?? 0)));
                return null;
            }
            else if ((buffer.Count == 0) && (stream.peek_char(0) == '\n'))
            {
                //commandのみ
                terprip = false;
                implicit_close(IndentTypeKey.jisage);
                //adhook hack
                noprint = false;
                indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.jisage));
                return new Helpers.Tag.MultilineJisage(this, jisage_width(command) ?? 0);
            }
            else //rubocop:disable Lint/DuplicateBranch
            {
                buffer.Insert(0, new BufferItemTag(new Helpers.Tag.OnelineJisage(this, jisage_width(command) ?? 0)));
                return null;
            }
        }

        public void apply_warichu(string command)
        {
            if (command.Contains(END_MARK))
            {
                if (stream.peek_char(0) != PAREN_END_MARK)
                {
                    push_char(PAREN_END_MARK);
                }
                push_chars("</span>");
            }
            else
            {
                var check = ruby_buf.LastOrDefault();

                //NOTE: Do not remove duplicates!
                //rubocop:disable Style/IdenticalConditionalBranches
                if (check is BufferItemString itemString && itemString.to_html().EndsWith(new String(PAREN_BEGIN_MARK, 1)))
                {
                    push_chars("<span class=\"warichu\">");
                }
                else
                {
                    push_chars("<span class=\"warichu\">");
                    push_char(PAREN_BEGIN_MARK);
                }
                //rubocop:enable Style/IdenticalConditionalBranches
            }
        }

        public int chitsuki_length(string command)
        {
            command = Utils.convert_japanese_number(command);
            var matched = PAT_JI_LEN.Match(command);
            if (matched.Success && int.TryParse(matched.Groups[1].Value, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        public Helpers.Tag.Chitsuki? apply_chitsuki(string @string, bool multiline = false)
        {
            if (@string.Contains(CLOSE_MARK + INDENT_TYPE[IndentTypeKey.chitsuki] + END_MARK) ||
                @string.Contains(CLOSE_MARK + JISAGE_COMMAND + END_MARK))
            {
                explicit_close(IndentTypeKey.chitsuki);
                indent_stack.Pop();
                return null;
            }
            else
            {
                var len = chitsuki_length(@string);
                if (multiline)
                {
                    //複数行指定
                    implicit_close(IndentTypeKey.chitsuki);
                    indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.chitsuki));
                    return new Helpers.Tag.MultilineChitsuki(this, len);
                }
                else
                {
                    //1行のみ
                    return new Helpers.Tag.OnelineChitsuki(this, len);
                }
            }
        }

        public Helpers.Tag.MultilineMidashi apply_midashi(string command)
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.midashi));
            Utils.MidashiType midashi_type;
            if (command.Contains(DOGYO_MARK))
            {
                midashi_type = Utils.MidashiType.dogyo;
            }
            else if (command.Contains(MADO_MARK))
            {
                midashi_type = Utils.MidashiType.mado;
            }
            else
            {
                midashi_type = Utils.MidashiType.normal;
                terprip = false;
            }
            return new Helpers.Tag.MultilineMidashi(this, command, midashi_type);
        }

        public Helpers.Tag.MultilineYokogumi apply_yokogumi()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.yokogumi));
            return new Helpers.Tag.MultilineYokogumi(this);
        }

        public Helpers.Tag.Keigakomi apply_keigakomi()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.keigakomi));
            return new Helpers.Tag.Keigakomi(this);
        }

        public Helpers.Tag.MultilineCaption apply_caption()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.caption));
            return new Helpers.Tag.MultilineCaption(this);
        }

        public Helpers.Tag.Jizume? apply_jizume(string command)
        {
            var matched = Regex.Match(Utils.convert_japanese_number(command), @$"(\d*)(?:{INDENT_TYPE[IndentTypeKey.jizume]})");
            if (!matched.Success || matched.Groups.Count < 2 || !int.TryParse(matched.Groups[1].Value, out int w)) return null;
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.jizume));
            return new Helpers.Tag.Jizume(this, w);
        }

        public void push_block_tag(Helpers.Tag.Block? tag, StringBuilder closing)
        {
            if (tag is null) throw new Exception($"{nameof(tag)} is empty");
            push_chars(new BufferItemTag(tag));
            closing.Append(tag.close_tag());
        }

        public IndentTypeKey detect_style_size(string style)
        {
            if (style.Contains("小"))
            {
                return IndentTypeKey.sho;
            }
            else
            {
                return IndentTypeKey.dai;
            }
        }

        public bool exec_inline_start_command(string command)
        {
            switch (command)
            {
                case CHUUKI_COMMAND:
                    style_stack.push(command, "</ruby>");
                    push_chars("<ruby><rb>");
                    return true;
                case TCY_COMMAND:
                    style_stack.push(command, "</span>");
                    push_chars("<span dir=\"ltr\">");
                    return true;
                case KEIGAKOMI_COMMAND:
                    style_stack.push(command, "</span>");
                    push_chars("<span class=\"keigakomi\">");
                    return true;
                case YOKOGUMI_COMMAND:
                    style_stack.push(command, "</span>");
                    push_chars("<span class=\"yokogumi\">");
                    return true;
                case CAPTION_COMMAND:
                    style_stack.push(command, "</span>");
                    push_chars("<span class=\"caption\">");
                    return true;
                case WARIGAKI_COMMAND:
                    style_stack.push(command, "</span>");
                    push_chars("<span class=\"warigaki\">");
                    return true;
                case OMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h3>");
                    terprip = false;
                    push_chars($"<h3 class=\"o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(100)}\">");
                    return true;
                case NAKAMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h4>");
                    terprip = false;
                    push_chars($"<h4 class=\"naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(10)}\">");
                    return true;
                case KOMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h5>");
                    terprip = false;
                    push_chars($"<h5 class=\"ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(1)}\">");
                    return true;
                case DOGYO_OMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h3>");
                    terprip = false;
                    push_chars($"<h3 class=\"dogyo-o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(100)}\">");
                    return true;
                case DOGYO_NAKAMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h4>");
                    terprip = false;
                    push_chars($"<h4 class=\"dogyo-naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(10)}\">");
                    return true;
                case DOGYO_KOMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h5>");
                    terprip = false;
                    push_chars($"<h5 class=\"dogyo-ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi#{@midashi_counter.generate_id(1)}\">");
                    return true;
                case MADO_OMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h3>");
                    terprip = false;
                    push_chars($"<h3 class=\"mado-o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(100)}\">");
                    return true;
                case MADO_NAKAMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h4>");
                    terprip = false;
                    push_chars($"<h4 class=\"mado-naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(10)}\">");
                    return true;
                case MADO_KOMIDASHI_COMMAND:
                    style_stack.push(command, "</a></h5>");
                    terprip = false;
                    push_chars($"<h5 class=\"mado-ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.generate_id(1)}\">");
                    return true;
                default:
                    var matchedCharSize = PAT_CHARSIZE.Match(command);
                    if (matchedCharSize.Success)
                    {
                        style_stack.push(command, "</span>");
                        var nest = matchedCharSize.Groups[1].Value;
                        var style = matchedCharSize.Groups[2].Value;
                        var times = int.Parse(Utils.convert_japanese_number(nest));
                        var daisho = detect_style_size(style);
                        var html_class = daisho.ToString() + times.ToString();
                        var size = Utils.create_font_size(times, daisho);
                        push_chars($"<span class=\"{html_class}\" style=\"font-size: {size};\">");
                        return true;
                    }
                    else
                    {
                        //Decoration
                        var key = command;
                        var matchedDirection = PAT_DIRECTION.Match(command);
                        var filter = (string x) => x;
                        if (matchedDirection.Success)
                        {
                            var dir = matchedDirection.Groups[1].Value;
                            var com = matchedDirection.Groups[2].Value;
                            //renew command
                            key = com;
                            if (command.Contains(TEN_MARK))
                            {
                                if (dir == new string(LEFT_MARK, 1) || dir == new string(UNDER_MARK, 1))
                                {
                                    filter = x => $"{x}_after";
                                }
                            }
                            else if (command.Contains(SEN_MARK))
                            {
                                if (dir == new string(LEFT_MARK, 1) || dir == new string(OVER_MARK, 1))
                                {
                                    filter = x => x.Replace("under", "over");
                                }
                            }
                        }

                        var found = YamlValues.CommandTable(key);
                        //found = [class, tag]
                        if (found.Item1 is not null)
                        {
                            style_stack.push(command, $"</{found.tag}>");
                            push_chars($"<{found.tag} class=\"{filter.Invoke(found.@class)}\">");
                            return true;
                        }
                        else
                        {
#if DEBUG
                            warnChannel.print(string.Format(I18n.MSG["warn_undefined_command"], line_number, key));
#endif
                            return false;
                        }
                    }
            }
        }

        public void exec_inline_end_command(string command)
        {
            var encount = command.Replace(END_MARK, "");
            if (encount == MAIN_MARK)
            {
                //force to finish main_text
                section = SectionKind.tail;
                ensure_close();
                noprint = true;
                @out.print("</div>\r\n<div class=\"after_text\">\r\n<hr />\r\n");
            }
            else if (encount.Contains(CHUUKI_COMMAND) && style_stack.last_command() == CHUUKI_COMMAND)
            {
                //special inline ruby
                style_stack.pop();
                var matched = PAT_INLINE_RUBY.Match(encount);
                if (!matched.Success) throw new Exception("Regex failed.");
                var ruby = matched.Groups[1].Value;
                push_chars($"</rb><rp>（</rp><rt>{ruby}</rt><rp>）</rp></ruby>");

            }
            else if (style_stack.last_command()?.Contains(encount) == true)
            {
                push_chars(style_stack.pop().closingTag);
            }
            else
            {
                throw new Exceptions.InvalidNestingException(encount, style_stack.last_command() ?? "");
            }
        }

        public Helpers.Tag.EditorNote? exec_block_start_command(string command)
        {
            var original_command = command;//kurema:C#では普通string自体を書き換えません。つまりdup相当は不要。
            command = new Regex($"^{OPEN_MARK}").Replace(command, "");
            var match_buf = new StringBuilder();

            void push_item(string command, StringBuilder match_buf, bool pop, IndentTypeKey key, Func<string, Helpers.Tag.Block?> func, Action? action = null)//kurema:これは関数内関数です。
            {
                if (command.Contains(INDENT_TYPE[key]))
                {
                    if (pop && match_buf.Length != 0) indent_stack.Pop();
                    push_block_tag(func(command), match_buf);
                    action?.Invoke();
                }
            }

            if (command.Contains(INDENT_TYPE[IndentTypeKey.jisage]))
            {
                push_block_tag(apply_jisage(command), match_buf);
            }
            else if (new Regex($"({INDENT_TYPE[IndentTypeKey.chitsuki]}|{JIAGE_COMMAND})$").IsMatch(command))
            {
                push_block_tag(apply_chitsuki(command, multiline: true) ?? throw new Exception(), match_buf);
            }

            //kurema:繰り返しが多いので関数化しました。
            push_item(command, match_buf, false, IndentTypeKey.midashi, a => apply_midashi(a));
            push_item(command, match_buf, true, IndentTypeKey.jizume, a => apply_jizume(a));
            push_item(command, match_buf, true, IndentTypeKey.yokogumi, a => apply_yokogumi());
            push_item(command, match_buf, true, IndentTypeKey.keigakomi, a => apply_keigakomi());
            push_item(command, match_buf, true, IndentTypeKey.caption, a => apply_caption());
            push_item(command, match_buf, true, IndentTypeKey.futoji, a => new Helpers.Tag.MultilineStyle(this, "futoji"),
                () => { indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.futoji)); });
            push_item(command, match_buf, true, IndentTypeKey.shatai, a => new Helpers.Tag.MultilineStyle(this, "shatai"),
                () => { indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.shatai)); });

            {//kurema:空ブロックは微妙な変数名をスコープ内だけにするのが目的です。
                var matched = PAT_CHARSIZE.Match(command);
                if (matched.Success)
                {
                    var nest = matched.Groups[1].Value;
                    var style = matched.Groups[2].Value;
                    if (match_buf.Length > 0) indent_stack.Pop();
                    var daisho = detect_style_size(style);
                    push_block_tag(new Helpers.Tag.FontSize(this, int.Parse(Utils.convert_japanese_number(nest)), daisho), match_buf);
                    indent_stack.Push(new IndentStackItemIndentTypeKey(daisho));
                }
            }

            if (match_buf.Length == 0)
            {
                return apply_rest_notes(original_command);
            }
            else
            {
                tag_stack.Push(match_buf.ToString());
                return null;
            }
        }

        public Helpers.Tag.EditorNote? exec_block_end_command(string command)
        {
            var original_command = command;//kurema:C#では普通string自体を書き換えません。
            command = new Regex($@"^{CLOSE_MARK}").Replace(command, "");
            IIndentStackItem? matched = null;
            var mode = detect_command_mode(command);

            if (mode != null)
            {
                explicit_close(mode.Value);
                matched = indent_stack.Pop();
            }

            if (matched != null)
            {
                if (matched is not IndentStackItemString) terprip = false;
                return null;
            }
            else
            {
                return apply_rest_notes(original_command);
            }
        }

        public Helpers.Tag.Tag exec_img_command(string command, string raw)
        {
            var matched = PAT_IMAGE.Match(raw);
            if (matched.Success)
            {
                var alt = matched.Groups[1].Value;
                var src = matched.Groups[2].Value;
                var width = matched.Groups[4].Value;
                var height = matched.Groups[5].Value;

                string css_class = alt.Contains(PHOTO_COMMAND) ? "photo" : "illustration";
                return new Helpers.Tag.Img(src, css_class, alt, width, height);
            }
            else
            {
                return apply_rest_notes(command);
            }
        }

        public IBufferItem exec_frontref_command(string command)
        {
            var matched = PAT_FRONTREF.Match(command);
            if (!matched.Success) throw new Exception();
            string reference = matched.Groups[1].Value;
            string spec1 = matched.Groups[2].Value;
            string spec2 = matched.Groups[3].Value;
            //var spec = !string.IsNullOrEmpty(spec1) ? spec1 + spec2 : spec2;
            var spec = spec1 + spec2;
            if (!string.IsNullOrEmpty(reference))
            {
                var found = search_front_reference(reference);
                if (found is not null)
                {
                    var tmp = exec_style(found, spec);
                    if (tmp is not null) return tmp;

                    recovery_front_reference(found);
                }
            }
            //comment out?
            return new BufferItemTag(apply_rest_notes(command));
        }

        /// <summary>
        /// 傍記を並べる用
        /// </summary>
        /// <param name="bouki"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public string multiply(string bouki, int times)
        {
            //kurema:もっとスマートに書けると思う。
            if (times == 0) return "";
            var sep = "&nbsp;";
            var result = new StringBuilder(bouki);
            for (int i = 1; i < times; i++)
            {
                result.Append(sep);
                result.Append(bouki);
            }
            return result.ToString();
        }

        /// <summary>
        /// rubyタグの再生成(本体はrearrange_ruby)
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="upper_ruby"></param>
        /// <param name="under_ruby"></param>
        /// <returns></returns>
        //complex ruby wrap up utilities -- don't erase! we will use soon ...
        public Helpers.Tag.Ruby rearrange_ruby_tag(System.Collections.IEnumerable targets, string upper_ruby, string under_ruby)
        {
            return Helpers.Tag.Ruby.rearrange_ruby(targets, upper_ruby, under_ruby);
        }

        public IBufferItem? exec_style(TextBuffer targets, string command)
        {
            var try_kuten = kuten2png(command);
            if (try_kuten.to_html() != command) { return try_kuten; }
            else if (command.Contains(TCY_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.Dir(targets));
            }
            else if (command.Contains(YOKOGUMI_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.InlineYokogumi(targets));
            }
            else if (command.Contains(KEIGAKOMI_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.InlineKeigakomi(targets));
            }
            else if (command.Contains(CAPTION_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.InlineCaption(targets));
            }
            else if (command.Contains(KAERITEN_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.Kaeriten(targets.to_html()));//kurema:to_html()して大丈夫？
            }
            else if (command.Contains(KUNTEN_OKURIGANA_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.Okurigana(targets.to_html()));
            }
            else if (command.Contains(MIDASHI_COMMAND))
            {
                var midashi_type = Utils.MidashiType.normal;
                if (command.Contains(DOGYO_MARK))
                {
                    midashi_type = Utils.MidashiType.dogyo;
                }
                else if (command.Contains(MADO_MARK))
                {
                    midashi_type = Utils.MidashiType.mado;
                }
                else
                {
                    terprip = false;
                }
                return new BufferItemTag(new Helpers.Tag.Midashi(this, targets, command, midashi_type));
            }
            {
                var match = PAT_CHARSIZE.Match(command);
                if (match.Success)
                {
                    var nest = match.Groups[1].Value;
                    var style = match.Groups[2].Value;
                    return new BufferItemTag(new Helpers.Tag.InlineFontSize(targets, int.Parse(Utils.convert_japanese_number(nest)), detect_style_size(style)));
                }
            }
            {
                var match = PAT_RUBY_DIR.Match(command);
                if (match.Success)
                {
                    var under = match.Groups[2].Value;
                    if (targets.Count == 1 && targets[0] is BufferItemTag targetFirstTag && targetFirstTag.tag is Helpers.Tag.Ruby tag)
                    {
                        if (!string.IsNullOrEmpty(tag.under_ruby)) throw new Exceptions.DontAllowTripleRubyException();

                        tag.under_ruby = under;
                        return new BufferItemTag(tag);
                    }
                    else
                    {
                        return new BufferItemTag(rearrange_ruby_tag(targets, "", under));
                    }
                }
            }
            {
                var match = PAT_CHUUKI.Match(command);
                if (match.Success)
                {
                    return new BufferItemTag(rearrange_ruby_tag(targets, match.Groups[1].Value, ""));
                }
            }
            {
                var match = PAT_BOUKI.Match(command);
                if (match.Success)
                {
                    return new BufferItemTag(rearrange_ruby_tag(targets, multiply(match.Groups[1].Value, targets.to_html().Length), ""));
                }
            }
            {//kurema:else相当
                //direction fix!
                var filter = (string x) => x;

                var match = PAT_DIRECTION.Match(command);
                if (match.Success)
                {
                    var dir = match.Groups[1].Value;
                    var com = match.Groups[2].Value;
                    //renew command
                    command = com;
                    if (command.Contains(TEN_MARK))
                    {
                        if (dir == new string(LEFT_MARK, 1) || dir == new string(UNDER_MARK, 1))
                        {
                            filter = x => $"{x}_after";
                        }
                    }
                    else if (command.Contains(SEN_MARK))
                    {
                        if (dir == new string(LEFT_MARK, 1) || dir == new string(OVER_MARK, 1))
                        {
                            filter = x => x.Replace("under", "over");
                        }
                    }
                }

                var found = YamlValues.CommandTable(command);
                //found = [class, tag]
                if (found.@class is not null && found.tag is not null)
                {
                    return new BufferItemTag(new Helpers.Tag.Decorate(targets, filter.Invoke(found.@class), found.tag));
                }
                else
                {
                    return null;
                }
            }
        }

        public Helpers.Tag.Tag? apply_dakuten_katakana(string command)
        {
            var match = Regex.Match(command, "1-7-8([2345])");
            if (!match.Success) return null;
            var n = int.Parse(match.Groups[1].Value);
            var frontref = DAKUTEN_KATAKANA_TABLE[n];
            var found = search_front_reference(frontref);
            if (found is not null)
            {
                return new Helpers.Tag.DakutenKatakana(n, String.Join("", found), gaiji_dir);
            }
            else
            {
                return apply_rest_notes(command);
            }
        }

        /// <summary>
        /// くの字点の処理
        /// 
        /// くの字点は現状そのまま出力するのでフッタの「表記について」で出力するかどうかのフラグ処理だけ行う
        /// </summary>
        public void assign_kunoji()
        {
            var second = stream.peek_char(0);
            switch (second)
            {
                case NOJI: chuuki_table[chuuki_table_keys.kunoji] = true; break;
                case DAKUTEN:
                    if (stream.peek_char(1) == NOJI)
                    {
                        chuuki_table[chuuki_table_keys.dakutenkunoji] = true;
                    }
                    break;
            }
        }

        public Helpers.Tag.EditorNote apply_rest_notes(string command)
        {
            chuuki_table[chuuki_table_keys.chuki] = true;
            return new Helpers.Tag.EditorNote(command);
        }

        //｜が来たときは文字種を無視してruby_bufを守らなきゃいけない
        public string? apply_ruby()
        {
            ruby_buf.@protected = false;
            var (ruby, _) = read_to_nest(RUBY_END_MARK);
            if (ruby.Length == 0)
            {
                return new string(RUBY_BEGIN_MARK, RUBY_END_MARK);
            }

            buffer.AddRange(ruby_buf.create_ruby(ruby));
            return null;
        }

        /// <summary>
        /// parse_bodyのフッタ版
        /// </summary>
        public void parse_tail()
        {
            //kurema:色々怪しい
            var @char = read_char();
            bool check = true;
            string? other = null;
            switch (@char)
            {
                case ACCENT_BEGIN:
                    check = false;
                    @char = read_accent();
                    break;
                case GAIJI_MARK:
                    other = dispatch_gaiji()?.to_html();
                    break;
                case COMMAND_BEGIN:
                    other = dispatch_aozora_command()?.to_html();
                    break;
                case KU:
                    assign_kunoji();
                    break;
                case RUBY_BEGIN_MARK:
                    other = new BufferItemString(apply_ruby() ?? "")?.to_html();
                    break;
                default:
                    if (@char == endchar)
                    {
                        throw new Exceptions.TerminateException();
                    }
                    break;
            }

            if (other is not null)
            {
                @char = null;
                if (other.Length == 0)
                {
                    @char = null;
                    other = null;
                }
                else if (other.Length == 1)
                {
                    @char = other[0];
                    other = null;
                }
            }

            switch (@char)
            {
                case '\n':
                    tail_output();
                    break;
                case RUBY_PREFIX:
                    ruby_buf.dump_into(buffer);
                    ruby_buf.@protected = true;
                    break;
                case null:
                    //noop
                    break;
                default:
                    if (other is null)
                    {
                        if (check) Utils.illegal_char_check(@char ?? ' ', line_number, warnChannel);
                        push_chars(escape_special_chars(@char ?? ' '));
                    }
                    else
                    {
                        var html = other;
                        if (check) foreach (var item in html) Utils.illegal_char_check(item, line_number, warnChannel);
                        push_chars(escape_special_chars(html));
                    }
                    break;
            }
        }

        /// <summary>
        /// general_outputのフッタ版
        /// </summary>
        public void tail_output()
        {
            ruby_buf.dump_into(buffer);
            var @string = string.Join(string.Empty, buffer);
            buffer = new TextBuffer();
            @string = @string.Replace("info@aozora.gr.jp", "<a href=\"mailto: info@aozora.gr.jp\">info@aozora.gr.jp</a>");
            {
                var aozora = "青空文庫（http://www.aozora.gr.jp/）";
                //kurema:
                //rubyの$&は最期にマッチした正規表現
                //https://docs.ruby-lang.org/ja/latest/method/Kernel/v/=26.html
                @string = @string.Replace(aozora, $"<a href=\"http://www.aozora.gr.jp/\">{aozora}</a>");
            }
            if (Regex.IsMatch(@string, @"(<br />$|</p>$|</h\d>$|<div.*>$|</div>$|^<[^>]*>$)"))
            {
                @out.print(@string);
                @out.print("\r\n");
            }
            else
            {
                @out.print(@string);
                @out.print("<br />\r\n");
            }
        }

        /// <summary>
        /// `●表記について`で使用した注記等を出力する
        /// </summary>
        public void hyoki()
        {
            //<br /> times fix
            @out.print("<br />\r\n</div>\r\n<div class=\"notation_notes\">\r\n<hr />\r\n<br />\r\n●表記について<br />\r\n<ul>\r\n");
            @out.print("\t<li>このファイルは W3C 勧告 XHTML1.1 にそった形式で作成されています。</li>\r\n");

            void printCondition(chuuki_table_keys key, string text)
            {
                if (chuuki_table[key]) @out.print(text);
            }

            printCondition(chuuki_table_keys.chuki, "\t<li>［＃…］は、入力者による注を表す記号です。</li>\r\n");
            if (chuuki_table[chuuki_table_keys.kunoji])
            {
                if (chuuki_table[chuuki_table_keys.dakutenkunoji])
                {
                    @out.print($"\t<li>「くの字点」は「{KU + NOJI}」で、「濁点付きくの字点」は「{KU + DAKUTEN + NOJI}」で表しました。</li>\r\n");
                }
                else
                {
                    @out.print($"\t<li>「くの字点」は「{KU + NOJI}」で表しました。</li>\r\n");
                }
            }
            else if (chuuki_table[chuuki_table_keys.dakutenkunoji])
            {
                @out.print($"\t<li>「濁点付きくの字点」は「{KU + DAKUTEN + NOJI}」で表しました。</li>\r\n");
            }
            if (!use_jisx0214_embed_gaiji) printCondition(chuuki_table_keys.newjis, "\t<li>「くの字点」をのぞくJIS X 0213にある文字は、画像化して埋め込みました。</li>\r\n");
            if (!use_unicode_embed_gaiji) printCondition(chuuki_table_keys.accent, "\t<li>アクセント符号付きラテン文字は、画像化して埋め込みました。</li>\r\n");
            if (images.Count > 0)
            {
                @out.print("\t<li>この作品には、JIS X 0213にない、以下の文字が用いられています。（数字は、底本中の出現「ページ-行」数。）これらの文字は本文内では「※［＃…］」の形で示しました。</li>\r\n</ul>\r\n<br />\r\n\t\t<table class=\"gaiji_list\">\r\n");
                foreach (var cell in images)
                {
                    var k = cell;
                    var vs = string.Join("、", k.ToCharArray().Select(a => new string(a, 1)));
                    @out.print($@"			<tr>
				<td>
				{k}
				</td>
				<td>&nbsp;&nbsp;</td>
				<td>
{vs}				</td>
				<!--
				<td>
				　　<img src=""../../../gaiji/others/xxxx.png"" alt=""#{k}"" width=32 height=32 />
				</td>
				-->
			</tr>
");
                }
                @out.print("\t\t</table>\r\n");
            }
            else
            {
                @out.print("</ul>\r\n");
            }
            @out.print("</div>\r\n");

        }

        //Original Aozora2Html#push_chars does not convert "'" into '&#39;'; it's old behaivor of CGI.escapeHTML().
        public string escape_special_chars(string text)
        {
            return Regex.Replace(text, @"[&"" <>]", a => escape_special_chars(a.Value[0]));
        }

        public string escape_special_chars(char @char) => @char switch
        {
            '&' => "&amp;",
            '"' => "&quot;",
            '<' => "&lt;",
            '>' => "&gt;",
            _ => new string(@char, 1),
        };
    }
}
