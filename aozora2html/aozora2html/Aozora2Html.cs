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

        public enum IndentTypeKey
        {
            jisage, chitsuki, midashi, jizume, yokogumi, keigakomi, caption, futoji, shatai, dai, sho
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
        protected List<string> indent_stack;//基本はシンボルだが、ぶらさげのときはdivタグの文字列が入る
        protected List<string> tag_stack;
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
            buffer = new Helpers.TextBuffer();
            ruby_buf = new Helpers.RubyBuffer();
            section = SectionKind.head;
            header = new Helpers.Header();
            style_stack = new Helpers.StyleStack();
            chuuki_table = new Dictionary<chuuki_table_keys, bool>();
            images = new List<string>();
            indent_stack = new List<string>();
            tag_stack = new List<string>();
            midashi_counter = new Helpers.MidashiCounter(0);
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
        //protected string? read_line()=>stream.re
    }
}
