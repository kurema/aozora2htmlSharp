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
    public class Aozora2Html : Helpers.INewMidashiIdProvider
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
        public const string JIAGE_COMMAND = "字上げ";//kurema:JISAGEとJIAGEは字面が似ているので注意！
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
        public static Regex PAT_ORIKAESHI_JISAGE => _PAT_ORIKAESHI_JISAGE ??= new Regex(@"折り返して(\d*)字下げ", RegexOptions.Compiled);
        private static Regex? _PAT_ORIKAESHI_JISAGE2 = null;
        public static Regex PAT_ORIKAESHI_JISAGE2 => _PAT_ORIKAESHI_JISAGE2 ??= new Regex(@"(\d*)字下げ、折り返して(\d*)字下げ", RegexOptions.Compiled);
        private static Regex? _PAT_JI_LEN = null;
        public static Regex PAT_JI_LEN => _PAT_JI_LEN ??= new Regex(@"([0-9]+)字", RegexOptions.Compiled);
        private static Regex? _PAT_INLINE_RUBY = null;
        public static Regex PAT_INLINE_RUBY => _PAT_INLINE_RUBY ??= new Regex(@"「(.*)」の注記付き", RegexOptions.Compiled);
        private static Regex? _PAT_IMAGE = null;
        public static Regex PAT_IMAGE => _PAT_IMAGE ??= new Regex(@"(.*)（(fig.+\.png)(、横([0-9]+)×縦([0-9]+))*）入る", RegexOptions.Compiled);
        private static Regex? _PAT_FRONTREF = null;
        public static Regex PAT_FRONTREF => _PAT_FRONTREF ??= new Regex(@"「([^「」]*(?:「.+」)*[^「」]*)」[にはの](「.+」の)*(.+)", RegexOptions.Compiled);
        //private static Regex? _PAT_FRONTREF2 = null;
        //public static Regex PAT_FRONTREF2 => _PAT_FRONTREF2 ??= new Regex(@"「([^「」]*(?:「.+」)*[^「」]*」?)」[にはの](「.+」の)*(.+)", RegexOptions.Compiled);
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
        public static Regex REGEX_HANKAKU => _REGEX_HANKAKU ??= new Regex(@"[A-Za-z0-9#\-\&'\,]", RegexOptions.Compiled);
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

        public enum ChuukiTableKeys
        {
            chuki, kunoji, dakutenkunoji, newjis, accent,
        }

        protected Jstream stream;
        protected IOutput @out;
        protected TextBuffer buffer;
        protected RubyBuffer ruby_buf;
        public SectionKind Section { get; set; }//現在処理中のセクション(:head,:head_end,:chuuki,:chuuki_in,:body,:tail)
        protected Header header;
        protected StyleStack style_stack;//スタイルのスタック
        protected Dictionary<ChuukiTableKeys, bool> chuuki_table;//最後にどの注記を出すかを保持しておく
        protected List<(string, List<string>)> images;//使用した外字の画像保持用
        protected Stack<IIndentStackItem> indent_stack;//基本はシンボルだが、ぶらさげのときはdivタグの文字列が入る
        protected Stack<string> tag_stack;
        protected MidashiCounter midashi_counter;//見出しのカウンタ、見出しの種類によって増分が異なる
        protected bool terprip;//改行制御用 (terpriはLisp由来?)
        protected char? endchar = null;//解析終了文字、AccentParserやTagParserでは異なる
        protected bool noprint;//行末を読み込んだとき、何も出力しないかどうかのフラグ

#pragma warning disable IDE1006 // 命名スタイル
        //kurema:jQueryは小文字始まりでしょ。
        public string? jQueryPath { get; set; } = "../../jquery-1.4.2.min.js";
#pragma warning restore IDE1006 // 命名スタイル

        //kurema: 警告メッセージ用チャンネルを独自に追加しました。
        protected IOutput warnChannel;

        //kurema:
        //本来はstatic変数。しかし、parserに属した方が扱いやすいので移しました。
        //具体的にはマルチスレッドで同時に実行する時とかにstaticだと問題があります。
        public bool UseJisx0213Accent { get; set; } = false;
        public bool UseJisx0214EmbedGaiji { get; set; } = false;
        public bool UseUnicodeEmbedGaiji { get; set; } = false;

        protected string gaiji_dir;
        protected string[] css_files;

        public Aozora2Html(Jstream input, IOutput output, IOutput? warnChannel = null, string? gaiji_dir = null, string[]? css_files = null)
        {
            stream = input;
            @out = output;
            buffer = new();
            ruby_buf = new();
            Section = SectionKind.head;
            style_stack = new();
            chuuki_table = new();
            images = new();
            indent_stack = new();
            tag_stack = new();
            midashi_counter = new(0);
            terprip = true;
            noprint = false;//kurema:元は初期nil。falseで問題ないと思われる。
            this.warnChannel = warnChannel ?? new OutputConsole();
            {
                this.gaiji_dir = gaiji_dir ?? "../../../gaiji/";
                if (System.IO.Path.DirectorySeparatorChar != '/') this.gaiji_dir = this.gaiji_dir.Replace(System.IO.Path.DirectorySeparatorChar, '/');
                if (System.IO.Path.AltDirectorySeparatorChar != '/') this.gaiji_dir = this.gaiji_dir.Replace(System.IO.Path.AltDirectorySeparatorChar, '/');
                if (!this.gaiji_dir.EndsWith("/")) this.gaiji_dir += "/";
            }
            this.css_files = css_files ?? new[] { "../../aozora.css" };
            header = new(this.css_files);
        }

        public enum SectionKind
        {
            head, head_end, chuuki, chuuki_in, body, tail
        }

        public int LineNumber => stream.Line;

        public bool BlockAllowedContext => style_stack.IsEmpty;

        /// <summary>
        /// parseする
        /// 
        /// 終了時（終端まで来た場合）にはthrow :terminateで脱出する
        /// </summary>
        public void Process()
        {
            try
            {
                Parse();
            }
            catch (Exceptions.AozoraException e)
            {
                //kurema:原文と異なりexitしません。
                warnChannel.PrintLine(e.GetMessageAozora(LineNumber));
                return;
            }
            catch (Exceptions.TerminateException)
            {
            }
            finally
            {
                TailOutput();
                FinalizeAozora();
                Close();
            }
        }

        public int GenerateNewMidashiId(int size) => midashi_counter.GenerateId(size);
        public int GenerateNewMidashiId(string size) => midashi_counter.GenerateId(size);

        public IBufferItem KutenToPng(string substring)
        {
            var desc = PAT_KUTEN.Replace(substring, "");
            var matched = new Regex(@"([12])-(\d{1,2})-(\d{1,2})").Match(desc);
            if (matched.Success && desc != NON_0213_GAIJI && !PAT_KUTEN_DUAL.IsMatch(desc))
            {
                chuuki_table[ChuukiTableKeys.newjis] = true;
                var codes = new int[] { int.Parse(matched.Groups[1].Value), int.Parse(matched.Groups[2].Value), int.Parse(matched.Groups[3].Value) };
                var folder = string.Format("{0,1}-{1:D2}", codes[0], codes[1]);//%1d-%02d
                var code = string.Format("{0,1}-{1:D2}-{2:D2}", codes[0], codes[1], codes[2]);//%1d-%02d-%02d
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
        public static IndentTypeKey? DetectCommandMode(string command)
        {
            //kurema:元は正規表現。途中でもマッチするはずなので==ではなくContains。
            if (command.Contains(INDENT_TYPE[IndentTypeKey.chitsuki] + END_MARK) || command.Contains(JIAGE_COMMAND + END_MARK))
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
        public virtual char? ReadChar() => stream.ReadChar();

        //一行読み込む
        public string? ReadLine() => stream.ReadLine();

        protected TextBuffer ReadAccent()
        {
            return new AccentParser(stream, ACCENT_END, chuuki_table, images, @out, warnChannel, gaiji_dir, css_files)
            {
                UseJisx0213Accent = this.UseJisx0213Accent,
                UseJisx0214EmbedGaiji = this.UseJisx0214EmbedGaiji,
                UseUnicodeEmbedGaiji = this.UseUnicodeEmbedGaiji
            }.ProcessAccent();
        }

        protected virtual (string, string) ReadToNest(char? endchar)
        {
            return new TagParser(stream, endchar, chuuki_table, images, @out, warnChannel: warnChannel, gaiji_dir: gaiji_dir)
            {
                UseJisx0213Accent = this.UseJisx0213Accent,
                UseJisx0214EmbedGaiji = this.UseJisx0214EmbedGaiji,
                UseUnicodeEmbedGaiji = this.UseUnicodeEmbedGaiji
            }.ProcessTag();
        }

        protected void FinalizeAozora()
        {
            PrintHyoki();
            DynamicContents();
            @out.Print("</body>\r\n</html>\r\n");
        }

        protected void DynamicContents()
        {
            @out.Print(DYNAMIC_CONTENTS);
        }

        protected void Close()
        {
            stream.Close();
            @out.Close();
            warnChannel.Close();
        }

        /// <summary>
        /// 記法のシンボル名から文字列へ変換する
        /// シンボルが見つからなければそのまま返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string ConvertIndentType(IIndentStackItem type) => type switch
        {
            IndentStackItemString typeString => typeString.Content,
            IndentStackItemIndentTypeKey typeIndentTypeKey => ConvertIndentType(typeIndentTypeKey.Content),
            _ => throw new Exception(),
        };

        protected string ConvertIndentType(IndentTypeKey type) => INDENT_TYPE.ContainsKey(type) ? INDENT_TYPE[type] : type.ToString();

        protected string? CheckCloseMatch(IndentTypeKey type)
        {
            IndentTypeKey? ind;
            if (indent_stack.FirstOrDefault() is IndentStackItemString)
            {
                noprint = true;
                ind = IndentTypeKey.jisage;
            }
            else if (indent_stack.FirstOrDefault() is IndentStackItemIndentTypeKey lastKey)
            {
                ind = lastKey.Content;
            }
            else
            {
                ind = null;
            }
            if (ind == type) return null;
            else return ConvertIndentType(type);
        }

        public bool ImplicitClose(IndentTypeKey type)
        {
            //kurema:apply_burasage()を見ると返り値bool扱いが良いっぽいけど、かなり妙。
            if (indent_stack.Count == 0) return false;

            if (CheckCloseMatch(type) != null)
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
                    PushChars(tag);
                }
                return true;
            }
        }

        /// <summary>
        /// 本文が終わってよいかチェックし、終わっていなければ例外をあげる
        /// </summary>
        public void EnsureClose()
        {
            var n = indent_stack.FirstOrDefault();
            if (n is null) return;

            throw new Exceptions.TerminateInStyleException(ConvertIndentType(n));
        }

        public void ExplicitClose(IndentTypeKey type)
        {
            var n = CheckCloseMatch(type);
            if (n is not null) throw new Exceptions.InvalidClosingException(n);

            if (tag_stack.Count == 0) return;
            var tag = tag_stack.Pop();
            PushChars(tag);
        }

        /// <summary>
        /// main loop
        /// </summary>
        /// <exception cref="Exceptions.EncountUndefinedConditionException"></exception>
        public virtual void Parse()
        {
            while (true)
            {
                switch (Section)
                {
                    case SectionKind.head:
                        ParseHeader();
                        break;
                    case SectionKind.head_end:
                        JudgeChuuki();
                        break;
                    case SectionKind.chuuki:
                    case SectionKind.chuuki_in:
                        ParseChuuki();
                        break;
                    case SectionKind.body:
                        ParseBody();
                        break;
                    case SectionKind.tail:
                        ParseTail();
                        break;
                    default:
                        throw new Exceptions.EncountUndefinedConditionException();
                }
            }
        }

        public void JudgeChuuki()
        {
            //注記が入るかどうかチェック
            int i = 0;
            while (true)
            {
                switch (stream.PeekChar(i))
                {
                    case '-': i++; break;
                    case '\n':
                        Section = i == 0 ? SectionKind.body : SectionKind.chuuki;
                        return;
                    default:
                        Section = SectionKind.body;
                        @out.Print("<br />\r\n");
                        return;
                }
            }
        }

        public void ParseHeader()
        {
            var @string = ReadLine();
            // refine from Tomita 09/06/14
            if (string.IsNullOrEmpty(@string))
            {
                //空行がくれば、そこでヘッダー終了とみなす
                Section = SectionKind.head_end;
                @out.Print(header.ToHtml(jQueryPath));
            }
            else
            {
                @string = @string!.Replace(new string(RUBY_PREFIX, 1), string.Empty);
                @string = PAT_RUBY.Replace(@string, string.Empty);
                header.Push(@string);
            }
        }

        public void ParseChuuki()
        {
            var @string = ReadLine();
            if (@string is null) return;
            if (!Regex.IsMatch(@string, @"^-+$")) return;

            switch (Section)
            {
                case SectionKind.chuuki:
                    Section = SectionKind.chuuki_in;
                    break;
                case SectionKind.chuuki_in:
                    Section = SectionKind.body;
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
        public void ParseBody()
        {
            object? @char = ReadChar();
            @char ??= ":eof";//kurema:C#版では:eofをnullで代用してるが、下では普通に@charがnullになりえるので適当な":eof"で仮置き。
            bool check = true;

            switch (@char)
            {
                case ACCENT_BEGIN:
                    check = false;
                    @char = ReadAccent();
                    break;
                case GAIJI_MARK:
                    @char = DispatchGaiji();
                    break;
                case COMMAND_BEGIN:
                    @char = DispatchAozoraCommand();
                    break;
                case KU:
                    AssignKunoji();
                    break;
                case RUBY_BEGIN_MARK:
                    @char = ApplyRuby();
                    break;
                default:
                    //kurema:TEIHON_MARK[0]は定数じゃないので普通に条件分岐で。
                    if ((char?)@char == TEIHON_MARK[0])
                    {
                        if (buffer.Count == 0) EndingCheck();
                    }
                    break;
            }

            if (@char is BufferItemString bufferItemString) @char = bufferItemString.ToHtml();
            if (@char is string s)
            {
                if (s.Length == 0) @char = null;
                if (s.Length == 1) @char = s[0];
            }

            switch (@char)
            {
                case '\n':
                    GeneralOutput();
                    break;
                case RUBY_PREFIX:
                    ruby_buf.DumpInto(buffer);
                    ruby_buf.IsProtected = true;
                    break;
                case null:
                    //noop
                    break;
                default:
                    if ((@char is ":eof" && endchar is null) || (@char is char c && c == endchar) || (@char is string s2 && s2.Length is 1 && s2[0] == endchar)
                        || (@char is BufferItemString bufferS && bufferS.Length == 1 && bufferS.ToHtml()[0] == endchar))
                    {
                        //suddenly finished the file
                        warnChannel.PrintLine(string.Format(I18n.Messages["warn_unexpected_terminator"], LineNumber));
                        throw new Exceptions.TerminateException();//kurema:例外で大域脱出したくない…。
                    }

                    if (@char is char charChar)
                    {
                        if (check) Utils.IllegalCharCheck(charChar, LineNumber, warnChannel);
                        PushChars(EscapeSpecialChars(new string(charChar, 1)));
                        break;
                    }
                    if (@char is string charString)
                    {
                        if (check) foreach (var charItem in charString) Utils.IllegalCharCheck(charItem, LineNumber, warnChannel);
                        PushChars(EscapeSpecialChars(charString));
                    }
                    else if (@char is IBufferItem bufferItem)
                    {
                        if (check) Utils.IllegalCharCheck(bufferItem, LineNumber, warnChannel);
                        PushChars(bufferItem);
                    }
                    else if (@char is TextBuffer textBuffer)
                    {
                        foreach (var item in textBuffer)
                        {
                            if (check) Utils.IllegalCharCheck(item, LineNumber, warnChannel);
                            PushChars(item);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 本文が終了したかどうかチェックする
        /// </summary>
        public void EndingCheck()
        {
            //`底本：`でフッタ(:tail)に遷移
            if (stream.PeekChar(0) != TEIHON_MARK[1] || stream.PeekChar(1) != TEIHON_MARK[2]) return;

            Section = SectionKind.tail;
            EnsureClose();
            @out.Print("</div>\r\n<div class=\"bibliographical_information\">\r\n<hr />\r\n<br />\r\n");
        }

        //kurema:
        //C#なので元とは結構違う実装方法。何が来るかまだ調査不足だけど後で良い。
        public void PushChars(string text)
        {
            foreach (var item in text) PushChar(item);
        }

        public void PushChars(IBufferItem item)
        {
            if (item is BufferItemString) PushChars(item.ToHtml());
            else if (item is BufferItemTag itemTag) PushChar(itemTag.Content);
        }

        public void PushChars(IEnumerable<IBufferItem> bufferItems)
        {
            foreach (var item in bufferItems) PushChars(item);
        }

        public void PushChar(char @char)
        {
            ruby_buf.PushChar(@char, buffer);
        }

        public void PushChar(Helpers.Tag.Tag tag)
        {
            ruby_buf.PushChar(tag, buffer);
        }

        /// <summary>
        /// 読み込んだ行の出力を行う
        /// 
        /// parserが改行文字を読み込んだら呼ばれる。
        /// 最終的に@ruby_bufと@bufferは初期化する
        /// </summary>
        /// <exception cref="Exceptions.DontCrlfInStyleException"></exception>
        //@return [void]
        public void GeneralOutput()
        {
            if (style_stack.Last() is not null)
            {
                throw new Exceptions.DontCrlfInStyleException(style_stack.LastCommand() ?? "");
            }

            //bufferにインデントタグだけがあったら改行しない！
            if (noprint)
            {
                noprint = false;
                return;
            }
            ruby_buf.DumpInto(buffer);
            var buf = buffer;
            buffer = new TextBuffer();
            var tail = new List<string>();

            var indent_type = buf.BlankType();
            var terpripLocal = buf.Terpri() && terprip;
            terprip = true;

            if (indent_stack.FirstOrDefault() is not null and IndentStackItemString lastString && indent_type == TextBuffer.BlankTypeResult.@false)//kurema:indentの場合は含まない？
            {
                @out.Print(lastString.Content);
            }

            foreach (var s in buf)
            {
                if (s is BufferItemTag stag)
                {
                    if (stag.Content is Helpers.Tag.IOnelineIndent stagOI)
                    {
                        tail.Insert(0, stagOI.CloseTag());
                    }
                    else if (stag.Content is Helpers.Tag.UnEmbedGaiji stagUEG && !stagUEG.Escaped)
                    {
                        //消してあった※を復活させて
                        @out.Print(new string(GAIJI_MARK, 1));
                    }
                }
                @out.Print(s.ToHtml());
            }

            //最後はCRLFを出力する
            if (indent_stack.FirstOrDefault() is IndentStackItemString)
            {
                //ぶら下げindent
                //tail always active

                //kurema:
                //元は
                //@out.print tail.map(&:to_s).join
                //to_s必要？
                @out.Print(string.Join("", tail?.ToArray() ?? new string[0]));
                if (indent_type == TextBuffer.BlankTypeResult.inline) @out.Print("\r\n");
                else if (indent_type == TextBuffer.BlankTypeResult.@true && terpripLocal) @out.Print("<br />\r\n");
                else @out.Print("</div>\r\n");
            }
            else if (tail.Count == 0 && terpripLocal)
            {
                @out.Print("<br />\r\n");
            }
            else
            {
                @out.Print(string.Join("", tail?.ToArray() ?? new string[0]));
                @out.Print("\r\n");
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
        public TextBuffer? SearchFrontReference(string @string)
        {
            if (string.IsNullOrEmpty(@string)) return null;
            IList<IBufferItem> searching_buf = ruby_buf.IsPresent ? ruby_buf : buffer;
            var last_string = searching_buf.LastOrDefault();
            switch (last_string)
            {
                case BufferItemString last_string_string:
                    if (last_string_string.Length == 0)
                    {
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        return SearchFrontReference(@string);
                    }
                    else if (last_string_string.ToHtml().EndsWith(@string))
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
                        searching_buf.Add(new BufferItemString(new Regex($"{Regex.Escape(@string)}$").Replace(last_string_string.ToHtml(), "")));
                        return new TextBuffer(@string);
                    }
                    else if (@string.EndsWith(last_string_string.ToHtml()))
                    {
                        //部分一致
                        //kurema:元は再帰対策でlast_stringと同じものをtmpに置いてたっぽい。
                        var tmp = searching_buf[searching_buf.Count - 1];
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        var found = SearchFrontReference(new Regex($"{Regex.Escape(last_string_string.ToHtml())}$").Replace(@string, ""));//kurema:不安
                        if (found != null)
                        {
                            found.Add(tmp);
                            return found;
                        }
                        else
                        {
                            searching_buf.Add(tmp);
                            return null;
                        }
                    }
                    break;
                case BufferItemTag last_string_tag when last_string_tag.Content is Helpers.Tag.ReferenceMentioned referenceMentioned:
                    var inner = referenceMentioned.TargetHtml;
                    if (inner == @string)
                    {
                        //完全一致
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        return new TextBuffer(new[] { last_string_tag });
                    }
                    else if (@string.EndsWith(inner))
                    {
                        //部分一致
                        var tmp = searching_buf[searching_buf.Count - 1];
                        searching_buf.RemoveAt(searching_buf.Count - 1);
                        var found = SearchFrontReference(new Regex($"{Regex.Escape(inner)}$").Replace(@string, ""));
                        if (found != null)
                        {
                            found.Add(tmp);
                            return found;
                        }
                        else
                        {
                            searching_buf.Add(tmp);
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
        public void RecoveryFrontReference(TextBuffer reference)
        {
            foreach (var elt in reference)
            {
                //if @ruby_buf.protected
                if (ruby_buf.IsPresent)
                {
                    ruby_buf.Add(elt);
                }
                else if (buffer.LastOrDefault() is BufferItemString buffer_last_string)
                {
                    if (elt is BufferItemString elt_string)
                    {
                        buffer_last_string.Append(elt_string.ToHtml());
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

        public Helpers.Tag.UnEmbedGaiji? EscapeGaiji(string command)
        {
            var match = PAT_GAIJI.Match(command);
            if (!match.Success || match.Groups.Count < 3) return null;
            var kanji = match.Groups[1].Value;
            var line = match.Groups[2].Value;

            var tmp = images.FirstOrDefault(a => a.Item1 == kanji);
            var index = images.IndexOf(tmp);
            if (tmp.Item1 == kanji)
            {
                images[index].Item2.Add(line);
            }
            else
            {
                images.Add((kanji, new List<string>() { line }));
            }
            return new Helpers.Tag.UnEmbedGaiji(command);
        }

        public IBufferItem DispatchGaiji()
        {
            //「※」の次が「［」でなければ外字ではない
            if (stream.PeekChar(0) != COMMAND_BEGIN) return new BufferItemString(new string(GAIJI_MARK, 1));

            //「［」を読み捨てる
            ReadChar();
            //embed?
            var (command, _) = ReadToNest(COMMAND_END);
            var try_emb = KutenToPng(command);
            if (try_emb is not BufferItemString try_emb_string || try_emb_string.ToHtml() != command) return try_emb;

            var matched = Regex.Match(command, @"U\+([0-9A-F]{4,5})");
            if (matched.Success && UseUnicodeEmbedGaiji)
            {
                var unicode_num = matched.Groups[1].Value;
                return new BufferItemTag(new Helpers.Tag.EmbedGaiji(this, null, null, command, gaiji_dir, unicode_num));
            }
            else
            {
                //Unemb
                return new BufferItemTag(EscapeGaiji(command) ?? throw new ArgumentNullException());
            }
        }

        /// <summary>
        /// 注記記法の場合分け
        /// </summary>
        public IBufferItem? DispatchAozoraCommand()
        {
            //「［」の次が「＃」でなければ注記ではない
            if (stream.PeekChar(0) != IGETA_MARK)
            {
                return new BufferItemString(new string(COMMAND_BEGIN, 1));
            }
            //「＃」を読み捨てる
            ReadChar();
            var (command, raw) = ReadToNest(COMMAND_END);
            //適用順序はこれで大丈夫か？　誤爆怖いよ誤爆

            static IBufferItem? GetReturnValue(Helpers.Tag.Tag? tag)
            {
                return tag is null ? null : new BufferItemTag(tag);
            }

            if (command.Contains(ORIKAESHI_COMMAND))
            {
                ApplyBurasage(command);
                return null;
            }
            else if (command.StartsWith(OPEN_MARK))
            {
                return GetReturnValue(ExecBlockStartCommand(command));
            }
            else if (command.StartsWith(CLOSE_MARK))
            {
                return GetReturnValue(ExecBlockEndCommand(command));
            }
            else if (command.Contains(WARICHU_COMMAND))
            {
                ApplyWarichu(command);
                return null;
            }
            else if (command.Contains(JISAGE_COMMAND))
            {
                return GetReturnValue(ApplyJisage(command));
            }
            else if (Regex.IsMatch(command, @"fig(\d)+_(\d)+\.png", RegexOptions.Compiled))
            {
                return GetReturnValue(ExecImgCommand(command, raw));
            }//avoid to try complex ruby -- escape to notes
            else if (PAT_REST_NOTES.IsMatch(command))
            {
                return GetReturnValue(ApplyRestNotes(command));
            }
            else if (command.EndsWith(END_MARK))
            {
                ExecInlineEndCommand(command);
                return null;
            }
            else if (PAT_REF.IsMatch(command))
            {
                return ExecFrontrefCommand(command);
            }
            else if (Regex.IsMatch(command, @"1-7-8[2345]"))//kurema:正規表現を二度実行するのは微妙。
            {
                return GetReturnValue(ApplyDakutenKatakana(command));
            }
            else if (PAT_KAERITEN.IsMatch(command))
            {
                return new BufferItemTag(new Helpers.Tag.Kaeriten(command));
            }
            else if (PAT_OKURIGANA.IsMatch(command))
            {
                return new BufferItemTag(new Helpers.Tag.Okurigana(PAT_REMOVE_OKURIGANA.Replace(command, "")));
            }
            else if (PAT_CHITSUKI.IsMatch(command))
            {
                return GetReturnValue(ApplyChitsuki(command));
            }
            else if (ExecInlineStartCommand(command))
            {
                return null;
            }
            else//rubocop:disable Lint/DuplicateBranch
            {
                return GetReturnValue(ApplyRestNotes(command));
            }
        }

        public void ApplyBurasage(string command)
        {
            if (ImplicitClose(IndentTypeKey.jisage))//kurema:implicit_close()は別にbool返してないっぽいけど…
            {
                terprip = false;
                GeneralOutput();
            }
            noprint = true; //always no print
            command = Utils.ConvertJapaneseNumber(command);
            string? tag;
            if (command.Contains(TENTSUKI_COMMAND))
            {
                var matched = PAT_ORIKAESHI_JISAGE.Match(command);
                string width;
                if (!matched.Success || matched.Groups.Count < 2) width = "0";
                else width = matched.Groups[1].Value;
                tag = $"<div class=\"burasage\" style=\"margin-left: {width}em; text-indent: -{width}em;\">";
            }
            else
            {
                var matched = PAT_ORIKAESHI_JISAGE2.Match(command);
                var (left, indent) = (!matched.Success || matched.Groups.Count < 3) ? ("0", "0") : (matched.Groups[1].Value, matched.Groups[2].Value);
                var left2 = int.Parse(left) - int.Parse(indent);
                tag = $"<div class=\"burasage\" style=\"margin-left: {indent}em; text-indent: {left2}em;\">";
            }
            indent_stack.Push(new IndentStackItemString(tag));
            tag_stack.Push(string.Empty); //dummy
        }

        public int? GetJisageWidth(string command)
        {
            var matched = new Regex(@$"(\d*)(?:{JISAGE_COMMAND})").Match(Utils.ConvertJapaneseNumber(command));
            if (!matched.Success || matched.Groups.Count < 1) return null;
            if (int.TryParse(matched.Groups[1].Value, out int num))
            {
                return num;
            }
            return null;
        }

        public Helpers.Tag.MultilineJisage? ApplyJisage(string command)
        {
            if (command.Contains(MADE_MARK) | command.Contains(END_MARK))
            {
                //字下げ終わり
                ExplicitClose(IndentTypeKey.jisage);
                indent_stack.Pop();
                return null;
            }
            else if (command.Contains(ONELINE_COMMAND))
            {
                //1行だけ
                buffer.Insert(0, new BufferItemTag(new Helpers.Tag.OnelineJisage(this, GetJisageWidth(command) ?? 0)));
                return null;
            }
            else if ((buffer.Count == 0) && (stream.PeekChar(0) == '\n'))
            {
                //commandのみ
                terprip = false;
                ImplicitClose(IndentTypeKey.jisage);
                //adhook hack
                noprint = false;
                indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.jisage));
                return new Helpers.Tag.MultilineJisage(this, GetJisageWidth(command) ?? 0);
            }
            else //rubocop:disable Lint/DuplicateBranch
            {
                buffer.Insert(0, new BufferItemTag(new Helpers.Tag.OnelineJisage(this, GetJisageWidth(command) ?? 0)));
                return null;
            }
        }

        public void ApplyWarichu(string command)
        {
            if (command.Contains(END_MARK))
            {
                if (stream.PeekChar(0) != PAREN_END_MARK)
                {
                    PushChar(PAREN_END_MARK);
                }
                PushChars("</span>");
            }
            else
            {
                var check = ruby_buf.LastOrDefault();

                //NOTE: Do not remove duplicates!
                //rubocop:disable Style/IdenticalConditionalBranches
                if (check is BufferItemString itemString && itemString.ToHtml().EndsWith(new String(PAREN_BEGIN_MARK, 1)))
                {
                    PushChars("<span class=\"warichu\">");
                }
                else
                {
                    PushChars("<span class=\"warichu\">");
                    PushChar(PAREN_BEGIN_MARK);
                }
                //rubocop:enable Style/IdenticalConditionalBranches
            }
        }

        public int ChitsukiLength(string command)
        {
            command = Utils.ConvertJapaneseNumber(command);
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

        public Helpers.Tag.Chitsuki? ApplyChitsuki(string @string, bool multiline = false)
        {
            if (@string.Contains(CLOSE_MARK + INDENT_TYPE[IndentTypeKey.chitsuki] + END_MARK) ||
                @string.Contains(CLOSE_MARK + JIAGE_COMMAND + END_MARK))
            {
                ExplicitClose(IndentTypeKey.chitsuki);
                indent_stack.Pop();
                return null;
            }
            else
            {
                var len = ChitsukiLength(@string);
                if (multiline)
                {
                    //複数行指定
                    ImplicitClose(IndentTypeKey.chitsuki);
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

        public Helpers.Tag.MultilineMidashi ApplyMidashi(string command)
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

        public Helpers.Tag.MultilineYokogumi ApplyYokogumi()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.yokogumi));
            return new Helpers.Tag.MultilineYokogumi(this);
        }

        public Helpers.Tag.Keigakomi ApplyKeigakomi()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.keigakomi));
            return new Helpers.Tag.Keigakomi(this);
        }

        public Helpers.Tag.MultilineCaption ApplyCaption()
        {
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.caption));
            return new Helpers.Tag.MultilineCaption(this);
        }

        public Helpers.Tag.Jizume? ApplyJizume(string command)
        {
            var matched = Regex.Match(Utils.ConvertJapaneseNumber(command), @$"(\d*)(?:{INDENT_TYPE[IndentTypeKey.jizume]})");
            if (!matched.Success || matched.Groups.Count < 2 || !int.TryParse(matched.Groups[1].Value, out int w)) return null;
            indent_stack.Push(new IndentStackItemIndentTypeKey(IndentTypeKey.jizume));
            return new Helpers.Tag.Jizume(this, w);
        }

        public void PushBlockTag(Helpers.Tag.Block? tag, StringBuilder closing)
        {
            if (tag is null) throw new Exception($"{nameof(tag)} is empty");
            PushChars(new BufferItemTag(tag));
            closing.Append(tag.CloseTag());
        }

        public IndentTypeKey DetectStyleSize(string style)
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

        public bool ExecInlineStartCommand(string command)
        {
            switch (command)
            {
                case CHUUKI_COMMAND:
                    style_stack.Push(command, "</ruby>");
                    PushChars("<ruby><rb>");
                    return true;
                case TCY_COMMAND:
                    style_stack.Push(command, "</span>");
                    PushChars("<span dir=\"ltr\">");
                    return true;
                case KEIGAKOMI_COMMAND:
                    style_stack.Push(command, "</span>");
                    PushChars("<span class=\"keigakomi\">");
                    return true;
                case YOKOGUMI_COMMAND:
                    style_stack.Push(command, "</span>");
                    PushChars("<span class=\"yokogumi\">");
                    return true;
                case CAPTION_COMMAND:
                    style_stack.Push(command, "</span>");
                    PushChars("<span class=\"caption\">");
                    return true;
                case WARIGAKI_COMMAND:
                    style_stack.Push(command, "</span>");
                    PushChars("<span class=\"warigaki\">");
                    return true;
                case OMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h3>");
                    terprip = false;
                    PushChars($"<h3 class=\"o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(100)}\">");
                    return true;
                case NAKAMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h4>");
                    terprip = false;
                    PushChars($"<h4 class=\"naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(10)}\">");
                    return true;
                case KOMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h5>");
                    terprip = false;
                    PushChars($"<h5 class=\"ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(1)}\">");
                    return true;
                case DOGYO_OMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h3>");
                    PushChars($"<h3 class=\"dogyo-o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(100)}\">");
                    return true;
                case DOGYO_NAKAMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h4>");
                    PushChars($"<h4 class=\"dogyo-naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(10)}\">");
                    return true;
                case DOGYO_KOMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h5>");
                    PushChars($"<h5 class=\"dogyo-ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi{@midashi_counter.GenerateId(1)}\">");
                    return true;
                case MADO_OMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h3>");
                    PushChars($"<h3 class=\"mado-o-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(100)}\">");
                    return true;
                case MADO_NAKAMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h4>");
                    PushChars($"<h4 class=\"mado-naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(10)}\">");
                    return true;
                case MADO_KOMIDASHI_COMMAND:
                    style_stack.Push(command, "</a></h5>");
                    PushChars($"<h5 class=\"mado-ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi{midashi_counter.GenerateId(1)}\">");
                    return true;
                default:
                    var matchedCharSize = PAT_CHARSIZE.Match(command);
                    if (matchedCharSize.Success)
                    {
                        style_stack.Push(command, "</span>");
                        var nest = matchedCharSize.Groups[1].Value;
                        var style = matchedCharSize.Groups[2].Value;
                        var times = int.Parse(Utils.ConvertJapaneseNumber(nest));
                        var daisho = DetectStyleSize(style);
                        var html_class = daisho.ToString() + times.ToString();
                        var size = Utils.CreateFontSize(times, daisho);
                        PushChars($"<span class=\"{html_class}\" style=\"font-size: {size};\">");
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

                        var (@class, tag) = YamlValues.CommandTable(key);
                        //found = [class, tag]
                        if (@class is not null)
                        {
                            style_stack.Push(command, $"</{tag}>");
                            PushChars($"<{tag} class=\"{filter.Invoke(@class)}\">");
                            return true;
                        }
                        else
                        {
#if DEBUG
                            warnChannel.PrintLine(string.Format(I18n.Messages["warn_undefined_command"], LineNumber, key));
#endif
                            return false;
                        }
                    }
            }
        }

        public void ExecInlineEndCommand(string command)
        {
            var encount = command.Replace(END_MARK, "");
            if (encount == MAIN_MARK)
            {
                //force to finish main_text
                Section = SectionKind.tail;
                EnsureClose();
                noprint = true;
                @out.Print("</div>\r\n<div class=\"after_text\">\r\n<hr />\r\n");
            }
            else if (encount.Contains(CHUUKI_COMMAND) && style_stack.LastCommand() == CHUUKI_COMMAND)
            {
                //special inline ruby
                style_stack.Pop();
                var matched = PAT_INLINE_RUBY.Match(encount);
                //if (!matched.Success) throw new Exception("Regex failed.");
                var ruby = (!matched.Success) ? "" : matched.Groups[1].Value;
                PushChars($"</rb><rp>（</rp><rt>{ruby}</rt><rp>）</rp></ruby>");

            }
            else if (style_stack.LastCommand()?.Contains(encount) == true)
            {
                PushChars(style_stack.Pop().ClosingTag);
            }
            else
            {
                throw new Exceptions.InvalidNestingException(encount, style_stack.LastCommand() ?? "");
            }
        }

        public Helpers.Tag.EditorNote? ExecBlockStartCommand(string command)
        {
            var original_command = command;//kurema:C#では普通string自体を書き換えません。つまりdup相当は不要。
            command = new Regex($"^{OPEN_MARK}").Replace(command, "");
            var match_buf = new StringBuilder();

            void push_item(string command, StringBuilder match_buf, bool pop, IndentTypeKey key, Func<string, Helpers.Tag.Block?> func, Action? action = null)//kurema:これは関数内関数です。
            {
                if (command.Contains(INDENT_TYPE[key]))
                {
                    if (pop && match_buf.Length != 0) indent_stack.Pop();
                    PushBlockTag(func(command), match_buf);
                    action?.Invoke();
                }
            }

            if (command.Contains(INDENT_TYPE[IndentTypeKey.jisage]))
            {
                PushBlockTag(ApplyJisage(command), match_buf);
            }
            else if (new Regex($"({INDENT_TYPE[IndentTypeKey.chitsuki]}|{JIAGE_COMMAND})$").IsMatch(command))
            {
                PushBlockTag(ApplyChitsuki(command, multiline: true) ?? throw new Exception(), match_buf);
            }

            //kurema:繰り返しが多いので関数化しました。
            push_item(command, match_buf, false, IndentTypeKey.midashi, a => ApplyMidashi(a));
            push_item(command, match_buf, true, IndentTypeKey.jizume, a => ApplyJizume(a));
            push_item(command, match_buf, true, IndentTypeKey.yokogumi, a => ApplyYokogumi());
            push_item(command, match_buf, true, IndentTypeKey.keigakomi, a => ApplyKeigakomi());
            push_item(command, match_buf, true, IndentTypeKey.caption, a => ApplyCaption());
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
                    var daisho = DetectStyleSize(style);
                    PushBlockTag(new Helpers.Tag.FontSize(this, int.Parse(Utils.ConvertJapaneseNumber(nest)), daisho), match_buf);
                    indent_stack.Push(new IndentStackItemIndentTypeKey(daisho));
                }
            }

            if (match_buf.Length == 0)
            {
                return ApplyRestNotes(original_command);
            }
            else
            {
                tag_stack.Push(match_buf.ToString());
                return null;
            }
        }

        public Helpers.Tag.EditorNote? ExecBlockEndCommand(string command)
        {
            var original_command = command;//kurema:C#では普通string自体を書き換えません。
            command = new Regex($@"^{CLOSE_MARK}").Replace(command, "");
            IIndentStackItem? matched = null;
            var mode = DetectCommandMode(command);

            if (mode != null)
            {
                ExplicitClose(mode.Value);
                matched = indent_stack.Count == 0 ? null : indent_stack.Pop();
            }

            if (matched != null)
            {
                if (matched is not IndentStackItemString) terprip = false;
                return null;
            }
            else
            {
                return ApplyRestNotes(original_command);
            }
        }

        public Helpers.Tag.Tag ExecImgCommand(string command, string raw)
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
                return ApplyRestNotes(command);
            }
        }

        public IBufferItem ExecFrontrefCommand(string command)
        {
            var matched = PAT_FRONTREF.Match(command);
            if (!matched.Success)
            {
                return new BufferItemTag(ApplyRestNotes(command));
                //kurema:
                //「吾輩は猫である」中にある「［＃「なります」」は底本では「なります。」］」対策。
                //だったのだが、apply_rest_notes(command)で問題ないのでコメントアウト。
                //原文もそういう意図だとも読める。

                //matched = PAT_FRONTREF2.Match(command);
                //if (!matched.Success) throw new Exception();
            }
            string reference = matched.Groups[1].Value;
            string spec1 = matched.Groups[2].Value;
            string spec2 = matched.Groups[3].Value;
            //var spec = !string.IsNullOrEmpty(spec1) ? spec1 + spec2 : spec2;
            var spec = spec1 + spec2;
            if (!string.IsNullOrEmpty(reference))
            {
                var found = SearchFrontReference(reference);
                if (found is not null)
                {
                    var tmp = ExecStyle(found, spec);
                    if (tmp is not null) return tmp;

                    RecoveryFrontReference(found);
                }
            }
            //comment out?
            return new BufferItemTag(ApplyRestNotes(command));
        }

        /// <summary>
        /// 傍記を並べる用
        /// </summary>
        /// <param name="bouki"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string GetMultipliedText(string bouki, int times)
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
        public Helpers.Tag.Ruby RearrangeRubyTag(System.Collections.IEnumerable targets, string upper_ruby, string under_ruby)
        {
            return Helpers.Tag.Ruby.RearrangeRuby(targets, upper_ruby, under_ruby);
        }

        public IBufferItem? ExecStyle(TextBuffer targets, string command)
        {
            var try_kuten = KutenToPng(command);
            if (try_kuten.ToHtml() != command) { return try_kuten; }
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
                return new BufferItemTag(new Helpers.Tag.Kaeriten(targets.ToHtml()));//kurema:to_html()して大丈夫？
            }
            else if (command.Contains(KUNTEN_OKURIGANA_COMMAND))
            {
                return new BufferItemTag(new Helpers.Tag.Okurigana(targets.ToHtml()));
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
                    return new BufferItemTag(new Helpers.Tag.InlineFontSize(targets, int.Parse(Utils.ConvertJapaneseNumber(nest)), DetectStyleSize(style)));
                }
            }
            {
                var match = PAT_RUBY_DIR.Match(command);
                if (match.Success)
                {
                    var under = match.Groups[2].Value;
                    if (targets.Count == 1 && targets[0] is BufferItemTag targetFirstTag && targetFirstTag.Content is Helpers.Tag.Ruby tag)
                    {
                        if (!string.IsNullOrEmpty(tag.RubyUnder)) throw new Exceptions.DontAllowTripleRubyException();

                        tag.RubyUnder = under;
                        return new BufferItemTag(tag);
                    }
                    else
                    {
                        return new BufferItemTag(RearrangeRubyTag(targets, "", under));
                    }
                }
            }
            {
                var match = PAT_CHUUKI.Match(command);
                if (match.Success)
                {
                    return new BufferItemTag(RearrangeRubyTag(targets, match.Groups[1].Value, ""));
                }
            }
            {
                var match = PAT_BOUKI.Match(command);
                if (match.Success)
                {
                    return new BufferItemTag(RearrangeRubyTag(targets, GetMultipliedText(match.Groups[1].Value, targets.ToHtml().Length), ""));
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

                var (@class, tag) = YamlValues.CommandTable(command);
                //found = [class, tag]
                if (@class is not null && tag is not null)
                {
                    return new BufferItemTag(new Helpers.Tag.Decorate(targets, filter.Invoke(@class), tag));
                }
                else
                {
                    return null;
                }
            }
        }

        public Helpers.Tag.Tag? ApplyDakutenKatakana(string command)
        {
            var match = Regex.Match(command, "1-7-8([2345])");
            if (!match.Success) return null;
            var n = int.Parse(match.Groups[1].Value);
            var frontref = DAKUTEN_KATAKANA_TABLE[n];
            var found = SearchFrontReference(frontref);
            if (found is not null)
            {
                return new Helpers.Tag.DakutenKatakana(n, String.Join("", found), gaiji_dir);
            }
            else
            {
                return ApplyRestNotes(command);
            }
        }

        /// <summary>
        /// くの字点の処理
        /// 
        /// くの字点は現状そのまま出力するのでフッタの「表記について」で出力するかどうかのフラグ処理だけ行う
        /// </summary>
        public void AssignKunoji()
        {
            var second = stream.PeekChar(0);
            switch (second)
            {
                case NOJI: chuuki_table[ChuukiTableKeys.kunoji] = true; break;
                case DAKUTEN:
                    if (stream.PeekChar(1) == NOJI)
                    {
                        chuuki_table[ChuukiTableKeys.dakutenkunoji] = true;
                    }
                    break;
            }
        }

        public Helpers.Tag.EditorNote ApplyRestNotes(string command)
        {
            chuuki_table[ChuukiTableKeys.chuki] = true;
            return new Helpers.Tag.EditorNote(command);
        }

        //｜が来たときは文字種を無視してruby_bufを守らなきゃいけない
        public string? ApplyRuby()
        {
            ruby_buf.IsProtected = false;
            var (ruby, _) = ReadToNest(RUBY_END_MARK);
            if (ruby.Length == 0)
            {
                return new string(RUBY_BEGIN_MARK, RUBY_END_MARK);
            }

            buffer.AddRange(ruby_buf.CreateRuby(ruby));
            return null;
        }

        /// <summary>
        /// parse_bodyのフッタ版
        /// </summary>
        public void ParseTail()
        {
            //kurema:色々怪しい
            var @char = ReadChar();
            bool check = true;
            bool escape = true;//kurema:read_accent()は文字列を返すので強引にエスケープしない指示をする。
            IBufferItem[] otherBuffer = new IBufferItem[0];
            switch (@char)
            {
                case ACCENT_BEGIN:
                    check = false;
                    @char = null;
                    escape = false;
                    otherBuffer = ReadAccent().ToArray();
                    break;
                case GAIJI_MARK:
                    @char = null;
                    otherBuffer = new[] { DispatchGaiji() };
                    break;
                case COMMAND_BEGIN:
                    {
                        @char = null;
                        var result = DispatchAozoraCommand();
                        if (result is not null) otherBuffer = new[] { result };
                    }
                    break;
                case KU:
                    AssignKunoji();
                    break;
                case RUBY_BEGIN_MARK:
                    @char = null;
                    otherBuffer = new[] { new BufferItemString(ApplyRuby() ?? "") };
                    break;
                default:
                    if (@char == endchar)
                    {
                        throw new Exceptions.TerminateException();
                    }
                    break;
            }

            if (otherBuffer.Length == 1 && otherBuffer[0] is BufferItemString otherString)
            {
                if (otherString.Length == 0)
                {
                    @char = null;
                    otherBuffer = new IBufferItem[0];
                }
                else if (otherString.Length == 1)
                {
                    @char = otherString.ToHtml()[0];
                    otherBuffer = new IBufferItem[0];
                }
            }

            switch (@char)
            {
                case '\n':
                    TailOutput();
                    break;
                case RUBY_PREFIX:
                    ruby_buf.DumpInto(buffer);
                    ruby_buf.IsProtected = true;
                    break;
                case null when otherBuffer.Length == 0:
                    //noop
                    break;
                default:
                    if (otherBuffer.Length == 0)
                    {
                        if (check) Utils.IllegalCharCheck(@char ?? ' ', LineNumber, warnChannel);
                        if (escape) PushChars(EscapeSpecialChars(@char ?? ' ')); else PushChars(new string(@char ?? ' ', 1));
                    }
                    else
                    {
                        foreach (var itemBuffer in otherBuffer)
                        {
                            if (check) Utils.IllegalCharCheck(itemBuffer, LineNumber, warnChannel);
                            if (escape) PushChars(EscapeSpecialChars(itemBuffer)); else PushChars(itemBuffer);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// general_outputのフッタ版
        /// </summary>
        public void TailOutput()
        {
            ruby_buf.DumpInto(buffer);
            var @string = string.Join(string.Empty, buffer.Select(a => a.ToHtml()));
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
                @out.Print(@string);
                @out.Print("\r\n");
            }
            else
            {
                @out.Print(@string);
                @out.Print("<br />\r\n");
            }
        }

        /// <summary>
        /// `●表記について`で使用した注記等を出力する
        /// </summary>
        public void PrintHyoki()
        {
            //<br /> times fix
            @out.Print("<br />\r\n</div>\r\n<div class=\"notation_notes\">\r\n<hr />\r\n<br />\r\n●表記について<br />\r\n<ul>\r\n");
            @out.Print("\t<li>このファイルは W3C 勧告 XHTML1.1 にそった形式で作成されています。</li>\r\n");

            void printCondition(ChuukiTableKeys key, string text)
            {
                if (checkChuukiTable(key)) @out.Print(text);
            }

            bool checkChuukiTable(ChuukiTableKeys key)
            {
                return chuuki_table.ContainsKey(key) && chuuki_table[key];
            }

            printCondition(ChuukiTableKeys.chuki, "\t<li>［＃…］は、入力者による注を表す記号です。</li>\r\n");
            if (checkChuukiTable(ChuukiTableKeys.kunoji))
            {
                if (checkChuukiTable(ChuukiTableKeys.dakutenkunoji))
                {
                    @out.Print($"\t<li>「くの字点」は「{KU + NOJI}」で、「濁点付きくの字点」は「{KU + DAKUTEN + NOJI}」で表しました。</li>\r\n");
                }
                else
                {
                    @out.Print($"\t<li>「くの字点」は「{KU + NOJI}」で表しました。</li>\r\n");
                }
            }
            else if (checkChuukiTable(ChuukiTableKeys.dakutenkunoji))
            {
                @out.Print($"\t<li>「濁点付きくの字点」は「{KU + DAKUTEN + NOJI}」で表しました。</li>\r\n");
            }
            if (!UseJisx0214EmbedGaiji) printCondition(ChuukiTableKeys.newjis, "\t<li>「くの字点」をのぞくJIS X 0213にある文字は、画像化して埋め込みました。</li>\r\n");
            if (!UseUnicodeEmbedGaiji) printCondition(ChuukiTableKeys.accent, "\t<li>アクセント符号付きラテン文字は、画像化して埋め込みました。</li>\r\n");
            if (images.Count > 0)
            {
                @out.Print("\t<li>この作品には、JIS X 0213にない、以下の文字が用いられています。（数字は、底本中の出現「ページ-行」数。）これらの文字は本文内では「※［＃…］」の形で示しました。</li>\r\n</ul>\r\n<br />\r\n\t\t<table class=\"gaiji_list\">\r\n");
                foreach (var cell in images)
                {
                    var k = cell.Item1;
                    var vs = string.Join("、", cell.Item2);
                    //kurema:ヒアドキュメント(逐語的文字列)に変換する場合は、Visual Studioを使用して下の行でクイック操作(Ctrl+.)を押してください。
                    @out.Print($"\t\t\t<tr>\r\n\t\t\t\t<td>\r\n\t\t\t\t{k}\r\n\t\t\t\t</td>\r\n\t\t\t\t<td>&nbsp;&nbsp;</td>\r\n\t\t\t\t<td>\r\n{vs}\t\t\t\t</td>\r\n\t\t\t\t<!--\r\n\t\t\t\t<td>\r\n\t\t\t\t　　<img src=\"../../../gaiji/others/xxxx.png\" alt=\"{k}\" width=32 height=32 />\r\n\t\t\t\t</td>\r\n\t\t\t\t-->\r\n\t\t\t</tr>\r\n");
                }
                @out.Print("\t\t</table>\r\n");
            }
            else
            {
                @out.Print("</ul>\r\n");
            }
            @out.Print("</div>\r\n");

        }

        public static IBufferItem EscapeSpecialChars(IBufferItem text)
        {
            if (text is BufferItemString @string) return new BufferItemString(EscapeSpecialChars(@string.ToHtml()));
            else return text;
        }


        //Original Aozora2Html#push_chars does not convert "'" into '&#39;'; it's old behaivor of CGI.escapeHTML().
        public static string EscapeSpecialChars(string text)
        {
            return Regex.Replace(text, @"[&"" <>]", a => EscapeSpecialChars(a.Value[0]));
        }

        public static string EscapeSpecialChars(char @char) => @char switch
        {
            '&' => "&amp;",
            '"' => "&quot;",
            '<' => "&lt;",
            '>' => "&gt;",
            _ => new string(@char, 1),
        };
    }
}
