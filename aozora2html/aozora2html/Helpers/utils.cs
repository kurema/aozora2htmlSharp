using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Aozora.Helpers;
// frozen_string_literal: true

public static class Utils
{
    // ユーティリティ関数モジュール
    //using StringRefinements

    public const string KANJI_NUMS = "一二三四五六七八九〇○";
    public const string KANJI_TEN = "十";
    public const string ZENKAKU_NUMS = "０-９";
    public const string ZENKAKU_NUMS_FULL = "０１２３４５６７８９";

    public static string CreateFontSize(int times, Aozora2Html.IndentTypeKey daisho)
    {
        string size = times switch
        {
            1 => "",
            2 => "x-",
            >= 3 => "xx-",
            _ => throw new Exceptions.InvalidFontSizeException(),
        };
        size += daisho switch
        {
            Aozora2Html.IndentTypeKey.dai => "large",
            Aozora2Html.IndentTypeKey.sho => "small",
            _ => throw new Exceptions.InvalidFontSizeException(),
        };
        return size;
    }

    //module_function :create_font_size

    public static string CreateMidashiTag(string size)
    {
        int hit = size.IndexOfAny(new[] { Aozora2Html.SIZE_SMALL, Aozora2Html.SIZE_MIDDLE, Aozora2Html.SIZE_LARGE });
        if (hit < 0) throw new Exceptions.UndefinedHeaderException();
        return size[hit] switch
        {
            Aozora2Html.SIZE_SMALL => "h5",
            Aozora2Html.SIZE_MIDDLE => "h4",
            Aozora2Html.SIZE_LARGE => "h3",
            _ => throw new Exceptions.UndefinedHeaderException(),
        };
    }

    //module_function :create_midashi_tag

    public enum MidashiType
    {
        normal, dogyo, mado,
    }

    public static string CreateMidashiClass(MidashiType type, string tag)
    {
        var normal_midashi_tag = new Dictionary<string, string>()
        {
            { "h5" , "ko-midashi" },
            { "h4" , "naka-midashi" },
            { "h3" , "o-midashi" },
        };

        var dogyo_midashi_tag = new Dictionary<string, string>()
        {
            {"h5" , "dogyo-ko-midashi"},
            {"h4" , "dogyo-naka-midashi"},
            {"h3" , "dogyo-o-midashi"},
        };

        var mado_midashi_tag = new Dictionary<string, string>()
        {
            {"h5" , "mado-ko-midashi"},
            {"h4" , "mado-naka-midashi"},
            {"h3" , "mado-o-midashi"},
        };

        return type switch
        {
            MidashiType.normal => normal_midashi_tag[tag],
            MidashiType.dogyo => dogyo_midashi_tag[tag],
            MidashiType.mado => mado_midashi_tag[tag],
            _ => throw new Exceptions.UndefinedHeaderException(),
        };
    }

    //module_function :create_midashi_class

    public static string ConvertJapaneseNumber(string command)
    {
        static string addBrancket(string original)
        {
            var sb = new System.Text.StringBuilder("[");
            sb.Append(original);
            sb.Append(']');
            return sb.ToString();
        }

        //kurema:dotnetにはPerlのtr///相当の機能がないっぽい。
        string tmp = Regex.Replace(command, addBrancket(ZENKAKU_NUMS_FULL), a => new string("0123456789"[ZENKAKU_NUMS_FULL.IndexOf(a.Value)], 1));
        tmp = Regex.Replace(tmp, addBrancket(KANJI_NUMS), a => new string("12345678900"[KANJI_NUMS.IndexOf(a.Value)], 1));
        tmp = Regex.Replace(tmp, @$"(\d){KANJI_TEN}(\d)", "$1$2");
        tmp = Regex.Replace(tmp, @$"(\d){KANJI_TEN}", "${1}0");
        tmp = Regex.Replace(tmp, @$"{KANJI_TEN}(\d)", "1$1");
        tmp = Regex.Replace(tmp, $@"{KANJI_TEN}", "10");
        return tmp;
    }
    //module_function :convert_japanese_number

    public enum IllegalCharCheckResult
    {
        jis_gaiji, chuki, onebyte, legal
    }

    public static void IllegalCharCheck(IBufferItem bufferItem, int line, IOutput output)
    {
        if (bufferItem is not BufferItemString bufferItemString) return;
        foreach (var item in bufferItemString.ToHtml()) IllegalCharCheck(item, line, output);
    }

    //kurema:
    //Rubyユーザーへの注記
    //C#で変数名の前に付ける@は無視されます。
    //予約語避けの為に使われます。
    public static IllegalCharCheckResult IllegalCharCheck(char @char, int line, IOutput output)
    {
        var result = IllegalCharCheck(@char);
        switch (result)
        {
            case IllegalCharCheckResult.jis_gaiji:
                output.PrintLine(string.Format(Resources.Resource.WarnJisGaiji, line, @char));
                break;
            case IllegalCharCheckResult.chuki:
                output.PrintLine(string.Format(Resources.Resource.WarnChuki, line, @char));
                break;
            case IllegalCharCheckResult.onebyte:
                output.PrintLine(string.Format(Resources.Resource.WarnOnebyte, line, @char));
                break;
            case IllegalCharCheckResult.legal:
            default:
                break;
        }
        return result;
    }

    // 使うべきではない文字があるかチェックする
    //
    // 警告を出力するだけで結果には影響を与えない。警告する文字は以下:
    //
    // * 1バイト文字
    // * `＃`ではなく`♯`
    // * JIS(JIS X 0208)外字
    //
    // @return [void]
    //
    public static IllegalCharCheckResult IllegalCharCheck(char @char)
    {
        var code = new Unpacked(@char);
        var codeUshort = (ushort)code;

        if ((codeUshort == 0x21) ||
            (codeUshort == 0x23) ||
            ((codeUshort >= 0xa1) && (codeUshort <= 0xa5)) ||
            ((codeUshort >= 0x28) && (codeUshort <= 0x29)) ||
            (codeUshort == 0x5b) ||
            (codeUshort == 0x5d) ||
            (codeUshort == 0x3d) ||
            (codeUshort == 0x3f) ||
            (codeUshort == 0x2b) ||
            ((codeUshort >= 0x7b) && (codeUshort <= 0x7d)))
        {
            return IllegalCharCheckResult.onebyte;
        }

        if (codeUshort == 0x81f2)
        {
            return IllegalCharCheckResult.chuki;
        }

        if (((codeUshort >= 0x81ad) && (codeUshort <= 0x81b7)) ||
            ((codeUshort >= 0x81c0) && (codeUshort <= 0x81c7)) ||
            ((codeUshort >= 0x81cf) && (codeUshort <= 0x81d9)) ||
            ((codeUshort >= 0x81e9) && (codeUshort <= 0x81ef)) ||
            ((codeUshort >= 0x81f8) && (codeUshort <= 0x81fb)) ||
            ((codeUshort >= 0x8240) && (codeUshort <= 0x824e)) ||
            ((codeUshort >= 0x8259) && (codeUshort <= 0x825f)) ||
            ((codeUshort >= 0x827a) && (codeUshort <= 0x8280)) ||
            ((codeUshort >= 0x829b) && (codeUshort <= 0x829e)) ||
            ((codeUshort >= 0x82f2) && (codeUshort <= 0x82fc)) ||
            ((codeUshort >= 0x8397) && (codeUshort <= 0x839e)) ||
            ((codeUshort >= 0x83b7) && (codeUshort <= 0x83be)) ||
            ((codeUshort >= 0x83d7) && (codeUshort <= 0x83fc)) ||
            ((codeUshort >= 0x8461) && (codeUshort <= 0x846f)) ||
            ((codeUshort >= 0x8492) && (codeUshort <= 0x849e)) ||
            ((codeUshort >= 0x84bf) && (codeUshort <= 0x84fc)) ||
            ((codeUshort >= 0x8540) && (codeUshort <= 0x85fc)) ||
            ((codeUshort >= 0x8640) && (codeUshort <= 0x86fc)) ||
            ((codeUshort >= 0x8740) && (codeUshort <= 0x87fc)) ||
            ((codeUshort >= 0x8840) && (codeUshort <= 0x889e)) ||
            ((codeUshort >= 0x9873) && (codeUshort <= 0x989e)) ||
            ((codeUshort >= 0xeaa5) && (codeUshort <= 0xeafc)) ||
            ((codeUshort >= 0xeb40) && (codeUshort <= 0xebfc)) ||
            ((codeUshort >= 0xec40) && (codeUshort <= 0xecfc)) ||
            ((codeUshort >= 0xed40) && (codeUshort <= 0xedfc)) ||
            ((codeUshort >= 0xee40) && (codeUshort <= 0xeefc)) ||
            ((codeUshort >= 0xef40) && (codeUshort <= 0xeffc)))
        {
            return IllegalCharCheckResult.jis_gaiji;
        }

        return IllegalCharCheckResult.legal;
    }
    //module_function :illegal_char_check

    public static Tag.CharType GetCharType(char character)
    {
        string word = new(character, 1);
        if (Aozora2Html.REGEX_HIRAGANA.IsMatch(word)) return Tag.CharType.Hiragana;
        if (Aozora2Html.REGEX_KATAKANA.IsMatch(word)) return Tag.CharType.Katankana;
        if (Aozora2Html.REGEX_ZENKAKU.IsMatch(word)) return Tag.CharType.Zenkaku;
        if (Aozora2Html.REGEX_HANKAKU.IsMatch(word)) return Tag.CharType.Hankaku;
        if (Aozora2Html.REGEX_KANJI.IsMatch(word)) return Tag.CharType.Kanji;
        if (Regex.IsMatch(word, @"[\.;""?!\)]")) return Tag.CharType.HankakuTerminate;
        return Tag.CharType.Else;
    }
}
