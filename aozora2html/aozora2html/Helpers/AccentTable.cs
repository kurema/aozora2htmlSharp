using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static (string? code, string? name, int depth) AccentTable(char? x, char? y, char? z)
    {
        switch (x)
        {
            case '!':
                switch (y)
                {
                    case '@': return ("1-09/1-09-03", "逆感嘆符", 2);
                }
                break;
            case '?':
                switch (y)
                {
                    case '@': return ("1-09/1-09-22", "逆疑問符", 2);
                }
                break;
            case 'A':
                switch (y)
                {
                    case '`': return ("1-09/1-09-23", "グレーブアクセント付きA", 2);
                    case '\'': return ("1-09/1-09-24", "アキュートアクセント付きA", 2);
                    case '^': return ("1-09/1-09-25", "サーカムフレックスアクセント付き", 2);
                    case '~': return ("1-09/1-09-26", "チルド付きA", 2);
                    case ':': return ("1-09/1-09-27", "ダイエレシス付きA", 2);
                    case '&': return ("1-09/1-09-28", "上リング付きA", 2);
                    case '_': return ("1-09/1-09-85", "マクロン付きA", 2);
                    case 'E':
                        switch (z)
                        {
                            case '&': return ("1-09/1-09-29", "リガチャAE", 3);
                        }
                        break;
                }
                break;
            case 'C':
                switch (y)
                {
                    case ',': return ("1-09/1-09-30", "セディラ付きC", 2);
                }
                break;
            case 'E':
                switch (y)
                {
                    case '`': return ("1-09/1-09-31", "グレーブアクセント付きE", 2);
                    case '\'': return ("1-09/1-09-32", "アキュートアクセント付きE", 2);
                    case '^': return ("1-09/1-09-33", "サーカムフレックスアクセント付きE", 2);
                    case ':': return ("1-09/1-09-34", "ダイエレシス付きE", 2);
                    case '_': return ("1-09/1-09-88", "マクロン付きE", 2);
                }
                break;
            case 'I':
                switch (y)
                {
                    case '`': return ("1-09/1-09-35", "グレーブアクセント付きI", 2);
                    case '\'': return ("1-09/1-09-36", "アキュートアクセント付きI", 2);
                    case '^': return ("1-09/1-09-37", "サーカムフレックスアクセント付きI", 2);
                    case ':': return ("1-09/1-09-38", "ダイエレシス付きI", 2);
                    case '_': return ("1-09/1-09-86", "マクロン付きI", 2);
                }
                break;
            case 'N':
                switch (y)
                {
                    case '~': return ("1-09/1-09-40", "チルド付きN", 2);
                }
                break;
            case 'O':
                switch (y)
                {
                    case '`': return ("1-09/1-09-41", "グレーブアクセント付きO", 2);
                    case '\'': return ("1-09/1-09-42", "アキュートアクセント付きO", 2);
                    case '^': return ("1-09/1-09-43", "サーカムフレックスアクセント付きO", 2);
                    case '~': return ("1-09/1-09-44", "チルド付きO", 2);
                    case ':': return ("1-09/1-09-45", "ダイエレシス付きO", 2);
                    case '/': return ("1-09/1-09-46", "ストローク付きO", 2);
                    case '_': return ("1-09/1-09-89", "マクロン付きO", 2);
                    case 'E':
                        switch (z)
                        {
                            case '&': return ("1-11/1-11-11", "リガチャOE大文字", 3);
                        }
                        break;
                }
                break;
            case 'U':
                switch (y)
                {
                    case '`': return ("1-09/1-09-47", "グレーブアクセント付きU", 2);
                    case '\'': return ("1-09/1-09-48", "アキュートアクセント付きU", 2);
                    case '^': return ("1-09/1-09-49", "サーカムフレックスアクセント付きU", 2);
                    case ':': return ("1-09/1-09-50", "ダイエレシス付きU", 2);
                    case '_': return ("1-09/1-09-87", "マクロン付きU", 2);
                }
                break;
            case 'Y':
                switch (y)
                {
                    case '\'': return ("1-09/1-09-51", "アキュートアクセント付きY", 2);
                }
                break;
            case 's':
                switch (y)
                {
                    case '&': return ("1-09/1-09-53", "ドイツ語エスツェット", 2);
                }
                break;
            case 'a':
                switch (y)
                {
                    case '`': return ("1-09/1-09-54", "グレーブアクセント付きA小文字", 2);
                    case '\'': return ("1-09/1-09-55", "アキュートアクセント付きA小文字", 2);
                    case '^': return ("1-09/1-09-56", "サーカムフレックスアクセント付きA小文字", 2);
                    case '~': return ("1-09/1-09-57", "チルド付きA小文字", 2);
                    case ':': return ("1-09/1-09-58", "ダイエレシス付きA小文字", 2);
                    case '&': return ("1-09/1-09-59", "上リング付きA小文字", 2);
                    case '_': return ("1-09/1-09-90", "マクロン付きA小文字", 2);
                    case 'e':
                        switch (z)
                        {
                            case '&': return ("1-09/1-09-60", "リガチャAE小文字", 3);
                        }
                        break;
                }
                break;
            case 'c':
                switch (y)
                {
                    case ',': return ("1-09/1-09-61", "セディラ付きC小文字", 2);
                }
                break;
            case 'e':
                switch (y)
                {
                    case '`': return ("1-09/1-09-62", "グレーブアクセント付きE小文字", 2);
                    case '\'': return ("1-09/1-09-63", "アキュートアクセント付きE小文字", 2);
                    case '^': return ("1-09/1-09-64", "サーカムフレックスアクセント付きE小文字", 2);
                    case ':': return ("1-09/1-09-65", "ダイエレシス付きE小文字", 2);
                    case '_': return ("1-09/1-09-93", "マクロン付きE小文字", 2);
                }
                break;
            case 'i':
                switch (y)
                {
                    case '`': return ("1-09/1-09-66", "グレーブアクセント付きI小文字", 2);
                    case '\'': return ("1-09/1-09-67", "アキュートアクセント付きI小文字", 2);
                    case '^': return ("1-09/1-09-68", "サーカムフレックスアクセント付きI小文字", 2);
                    case ':': return ("1-09/1-09-69", "ダイエレシス付きI小文字", 2);
                    case '_': return ("1-09/1-09-91", "マクロン付きI小文字", 2);
                }
                break;
            case 'n':
                switch (y)
                {
                    case '~': return ("1-09/1-09-71", "チルド付きN小文字", 2);
                }
                break;
            case 'o':
                switch (y)
                {
                    case '`': return ("1-09/1-09-72", "グレーブアクセント付きO小文字", 2);
                    case '\'': return ("1-09/1-09-73", "アキュートアクセント付きO小文字", 2);
                    case '^': return ("1-09/1-09-74", "サーカムフレックスアクセント付きO小文字", 2);
                    case '~': return ("1-09/1-09-75", "チルド付きO小文字", 2);
                    case ':': return ("1-09/1-09-76", "ダイエレシス付きO小文字", 2);
                    case '_': return ("1-09/1-09-94", "マクロン付きO小文字", 2);
                    case '/': return ("1-09/1-09-77", "ストローク付きO小文字", 2);
                    case 'e':
                        switch (z)
                        {
                            case '&': return ("1-11/1-11-10", "リガチャOE小文字", 3);
                        }
                        break;
                }
                break;
            case 'u':
                switch (y)
                {
                    case '`': return ("1-09/1-09-78", "グレーブアクセント付きU小文字", 2);
                    case '\'': return ("1-09/1-09-79", "アキュートアクセント付きU小文字", 2);
                    case '^': return ("1-09/1-09-80", "サーカムフレックスアクセント付きU小文字", 2);
                    case '_': return ("1-09/1-09-92", "マクロン付きU小文字", 2);
                    case ':': return ("1-09/1-09-81", "ダイエレシス付きU小文字", 2);
                }
                break;
            case 'y':
                switch (y)
                {
                    case '\'': return ("1-09/1-09-82", "アキュートアクセント付きY小文字", 2);
                    case ':': return ("1-09/1-09-84", "ダイエレシス付きY小文字", 2);
                }
                break;
        }
        return (null, null, 0);
    }
}
