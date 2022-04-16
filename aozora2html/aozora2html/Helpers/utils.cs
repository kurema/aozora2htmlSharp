using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Aozora.Helpers;
// frozen_string_literal: true

public class Utils
{
    // ユーティリティ関数モジュール
    //using StringRefinements

    public const string KANJI_NUMS = "一二三四五六七八九〇";
    public const string KANJI_TEN = "十";
    public const string ZENKAKU_NUMS = "０-９";
    public const string ZENKAKU_NUMS_FULL = "０１２３４５６７８９";

    public enum Daisho
    {
        dai, sho
    }

    public static string create_font_size(dynamic times, Daisho daisho)
    {
        string size = times switch
        {
            1 => "",
            2 => "x-",
            >= 3 => "xx-",
            _ => throw new Exceptions.InvalidFontSizeException(),
        };
        switch (daisho)
        {
            case Daisho.dai:
                size += "large";
                break;
            case Daisho.sho:
                size += "small";
                break;
            default:
                throw new Exceptions.InvalidFontSizeException();
        }
        return size;
    }

    //module_function :create_font_size

    public static string create_midashi_tag(char size)
    {
        return size switch
        {
            Aozora2Html.SIZE_SMALL => "h5",
            Aozora2Html.SIZE_MIDDLE => "h4",
            Aozora2Html.SIZE_LARGE => "h3",
            _ => throw new Exceptions.UndefinedHeaderException()
        };
    }

    //module_function :create_midashi_tag


    public enum MidashiType
    {
        normal, dogyo, mado,
    }

    public string create_midashi_class(MidashiType type, string tag)
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

    public string convert_japanese_number(string command)
    {
        //kurema:dotnetにはPerlのtr///相当の機能がないっぽい。
        string tmp = Regex.Replace(command, ZENKAKU_NUMS_FULL, a => "0123456789"[ZENKAKU_NUMS_FULL.IndexOf(a.Value)].ToString());
        tmp = Regex.Replace(tmp, KANJI_NUMS, a => "1234567890"[KANJI_NUMS.IndexOf(a.Value)].ToString());
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

    public illegal_char_check_result illegal_char_check(char @char, int line, IOutput output)
    {
        var result = illegal_char_check(@char);
        switch (result)
        {
            case illegal_char_check_result.jis_gaiji:
                output.print(string.Format(I18n.MSG["warn_jis_gaiji"], line, @char));
                break;
            case illegal_char_check_result.chuki:
                output.print(string.Format(I18n.MSG["warn_chuki"], line, @char));
                break;
            case illegal_char_check_result.onebyte:
                output.print(string.Format(I18n.MSG["warn_onebyte"], line, @char));
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
    public illegal_char_check_result illegal_char_check(char @char)
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
}
