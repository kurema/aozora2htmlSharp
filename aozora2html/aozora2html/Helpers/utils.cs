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

    public static string create_font_size(int times, Aozora2Html.IndentTypeKey daisho)
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

    public static string create_midashi_tag(string size)
    {
        if (size.Contains(new string(Aozora2Html.SIZE_SMALL, 1))) return "h5";
        else if (size.Contains(new string(Aozora2Html.SIZE_MIDDLE, 1))) return "h4";
        else if (size.Contains(new string(Aozora2Html.SIZE_LARGE, 1))) return "h3";
        else throw new Exceptions.UndefinedHeaderException();
    }

    //module_function :create_midashi_tag


    public enum MidashiType
    {
        normal, dogyo, mado,
    }

    public static string create_midashi_class(MidashiType type, string tag)
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

    public static string convert_japanese_number(string command)
    {
        static string addBrancket(string original)
        {
            var sb = new System.Text.StringBuilder("[");
            sb.Append(original);
            sb.Append("]");
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

    public enum illegal_char_check_result
    {
        jis_gaiji, chuki, onebyte, legal
    }

    public static void illegal_char_check(IBufferItem bufferItem, int line, IOutput output)
    {
        if (bufferItem is not BufferItemString bufferItemString) return;
        foreach (var item in bufferItemString.to_html()) illegal_char_check(item, line, output);
    }

    //kurema:
    //Rubyユーザーへの注記
    //C#で変数名の前に付ける@は無視されます。
    //予約語避けの為に使われます。
    public static illegal_char_check_result illegal_char_check(char @char, int line, IOutput output)
    {
        var result = illegal_char_check(@char);
        switch (result)
        {
            case illegal_char_check_result.jis_gaiji:
                output.println(string.Format(I18n.MSG["warn_jis_gaiji"], line, @char));
                break;
            case illegal_char_check_result.chuki:
                output.println(string.Format(I18n.MSG["warn_chuki"], line, @char));
                break;
            case illegal_char_check_result.onebyte:
                output.println(string.Format(I18n.MSG["warn_onebyte"], line, @char));
                break;
            case illegal_char_check_result.legal:
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
    public static illegal_char_check_result illegal_char_check(char @char)
    {
        var code = new Unpacked(@char);

        if ((code == "21") ||
            (code == "23") ||
            ((code >= "a1") && (code <= "a5")) ||
            ((code >= "28") && (code <= "29")) ||
            (code == "5b") ||
            (code == "5d") ||
            (code == "3d") ||
            (code == "3f") ||
            (code == "2b") ||
            ((code >= "7b") && (code <= "7d")))
        {
            return illegal_char_check_result.onebyte;
        }

        if (code == "81f2")
        {
            return illegal_char_check_result.chuki;
        }

        if (((code >= "81ad") && (code <= "81b7")) ||
            ((code >= "81c0") && (code <= "81c7")) ||
            ((code >= "81cf") && (code <= "81d9")) ||
            ((code >= "81e9") && (code <= "81ef")) ||
            ((code >= "81f8") && (code <= "81fb")) ||
            ((code >= "8240") && (code <= "824e")) ||
            ((code >= "8259") && (code <= "825f")) ||
            ((code >= "827a") && (code <= "8280")) ||
            ((code >= "829b") && (code <= "829e")) ||
            ((code >= "82f2") && (code <= "82fc")) ||
            ((code >= "8397") && (code <= "839e")) ||
            ((code >= "83b7") && (code <= "83be")) ||
            ((code >= "83d7") && (code <= "83fc")) ||
            ((code >= "8461") && (code <= "846f")) ||
            ((code >= "8492") && (code <= "849e")) ||
            ((code >= "84bf") && (code <= "84fc")) ||
            ((code >= "8540") && (code <= "85fc")) ||
            ((code >= "8640") && (code <= "86fc")) ||
            ((code >= "8740") && (code <= "87fc")) ||
            ((code >= "8840") && (code <= "889e")) ||
            ((code >= "9873") && (code <= "989e")) ||
            ((code >= "eaa5") && (code <= "eafc")) ||
            ((code >= "eb40") && (code <= "ebfc")) ||
            ((code >= "ec40") && (code <= "ecfc")) ||
            ((code >= "ed40") && (code <= "edfc")) ||
            ((code >= "ee40") && (code <= "eefc")) ||
            ((code >= "ef40") && (code <= "effc")))
        {
            return illegal_char_check_result.jis_gaiji;
        }

        return illegal_char_check_result.legal;
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
