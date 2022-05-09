using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Aozora.Helpers
{

    // encoding: utf-8
    public class Header
    {
        private readonly List<string> header;
        private readonly string[] css_files;

        public Header(string[]? css_files)
        {
            header = new List<string>();
            this.css_files = css_files ?? new string[0];
        }

        public void Push(string line)
        {
            header.Add(line);
        }

        public string OutHeaderInfo(Dictionary<HeaderElementTypeKind, string> hash, HeaderElementTypeKind attr, string? true_name = null)
        {
            if (hash.ContainsKey(attr))
            {
                var found = hash[attr];
                return $"<h2 class=\"{true_name ?? attr.ToString()}\">{found}</h2>\r\n";
            }
            else
            {
                return "";
            }
        }

        public HeaderElementTypeKind Header_element_type(string text)
        {
            bool original = true;
            foreach (var ch in text)
            {
                var code = new Unpacked(ch);
                if (("00" <= code && code <= "7f") || ("8140" <= code && code <= "8258") || ("839f" <= code && code <= "8491"))
                {
                    continue;
                }
                else
                {
                    original = false;
                    break;
                }
            }
            if (original)
            {
                return HeaderElementTypeKind.original;
            }
            else if (Aozora2Html.PAT_EDITOR.IsMatch(text))
            {
                return HeaderElementTypeKind.editor;
            }
            else if (Aozora2Html.PAT_HENYAKU.IsMatch(text))
            {
                return HeaderElementTypeKind.henyaku;
            }
            else if (Aozora2Html.PAT_TRANSLATOR.IsMatch(text))
            {
                return HeaderElementTypeKind.translator;
            }
            return HeaderElementTypeKind.author;
        }

        public enum HeaderElementTypeKind
        {
            editor, translator, henyaku, author, original, title, original_title, subtitle, original_subtitle
        }


        public HeaderElementTypeKind ProcessPerson(string word, Dictionary<HeaderElementTypeKind, string> header_info)
        {
            var type = Header_element_type(word);
            switch (type)
            {
                case HeaderElementTypeKind.editor:
                    header_info[HeaderElementTypeKind.editor] = word;
                    break;
                case HeaderElementTypeKind.translator:
                    header_info[HeaderElementTypeKind.translator] = word;
                    break;
                case HeaderElementTypeKind.henyaku:
                    header_info[HeaderElementTypeKind.henyaku] = word;
                    break;
                case HeaderElementTypeKind.author:
                default:
                    type = HeaderElementTypeKind.author;
                    header_info[HeaderElementTypeKind.author] = word;
                    break;
            }
            return type;
        }

        public string BuildTitle(Dictionary<HeaderElementTypeKind, string> header_info)
        {
            var buff = new[] {
            HeaderElementTypeKind.author,HeaderElementTypeKind.translator,HeaderElementTypeKind.editor,HeaderElementTypeKind.henyaku,
            HeaderElementTypeKind.title,HeaderElementTypeKind.original_title,
            HeaderElementTypeKind.subtitle,HeaderElementTypeKind.original_subtitle,
            }.Where(a => header_info.ContainsKey(a)).Select(a => header_info[a]);
            string buf_str = string.Join(" ", buff);
            return $"<title>{buf_str}</title>";
        }

        public Dictionary<HeaderElementTypeKind, string> BuildHeaderInfo()
        {
            //kurema: ヘッダーは行数によって順番が決まているみたい。
            var header_info = new Dictionary<HeaderElementTypeKind, string>() { { HeaderElementTypeKind.title, header[0] } };
            switch (header.Count)
            {
                case 2:
                    ProcessPerson(header[1], header_info);
                    break;
                case 3:
                    if (Header_element_type(header[1]) == HeaderElementTypeKind.original)
                    {
                        header_info[HeaderElementTypeKind.original_title] = header[1];
                        ProcessPerson(header[2], header_info);
                    }
                    else if (ProcessPerson(header[2], header_info) == HeaderElementTypeKind.author)
                    {
                        header_info[HeaderElementTypeKind.subtitle] = header[1];
                    }
                    else
                    {
                        header_info[HeaderElementTypeKind.author] = header[1];
                    }
                    break;
                case 4:
                    if (Header_element_type(header[1]) == HeaderElementTypeKind.original)
                    {
                        header_info[HeaderElementTypeKind.original_title] = header[1];
                    }
                    else
                    {
                        header_info[HeaderElementTypeKind.subtitle] = header[1];
                    }
                    if (ProcessPerson(header[3], header_info) == HeaderElementTypeKind.author)
                    {
                        header_info[HeaderElementTypeKind.subtitle] = header[2];
                    }
                    else
                    {
                        header_info[HeaderElementTypeKind.author] = header[2];
                    }
                    break;
                case 5:
                    header_info[HeaderElementTypeKind.original_title] = header[1];
                    header_info[HeaderElementTypeKind.subtitle] = header[2];
                    header_info[HeaderElementTypeKind.author] = header[3];
                    if (ProcessPerson(header[4], header_info) == HeaderElementTypeKind.author)
                    {
                        throw new Exceptions.AuthorTwiceException();
                    }
                    break;
                case 6:
                    header_info[HeaderElementTypeKind.original_title] = header[1];
                    header_info[HeaderElementTypeKind.subtitle] = header[2];
                    header_info[HeaderElementTypeKind.original_subtitle] = header[3];
                    header_info[HeaderElementTypeKind.author] = header[4];
                    if (ProcessPerson(header[5], header_info) == HeaderElementTypeKind.author)
                    {
                        throw new Exceptions.AuthorTwiceException();
                    }
                    break;
            }
            return header_info;
        }

        public string ToHtml(string? jQueryPath = "../../jquery-1.4.2.min.js")
        {
            //kurema:jQueryPathをnullにした場合はscript自体出力しない。epub用。
            var header_info = BuildHeaderInfo();

            // <title> 行を構築
            var html_title = BuildTitle(header_info);

            // 出力
            var out_buf = new System.Text.StringBuilder();
            out_buf.Append("<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\"\r\n    \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" >\r\n<head>\r\n	<meta http-equiv=\"Content-Type\" content=\"text/html;charset=Shift_JIS\" />\r\n	<meta http-equiv=\"content-style-type\" content=\"text/css\" />\r\n");
            foreach (var css in css_files)
            {
                out_buf.Append($"\t<link rel=\"stylesheet\" type=\"text/css\" href=\"{ css }\" />\r\n");
            }
            out_buf.Append($"\t{html_title}\r\n");
            if (!string.IsNullOrWhiteSpace(jQueryPath)) out_buf.Append($"	<script type=\"text/javascript\" src=\"{jQueryPath}\"></script>\r\n");
            out_buf.Append($"  <link rel=\"Schema.DC\" href=\"http://purl.org/dc/elements/1.1/\" />\r\n	<meta name=\"DC.Title\" content=\"{header_info[HeaderElementTypeKind.title]}\" />\r\n	<meta name=\"DC.Creator\" content=\"{header_info[HeaderElementTypeKind.author]}\" />\r\n	<meta name=\"DC.Publisher\" content=\"{Aozora2Html.AOZORABUNKO}\" />\r\n</head>\r\n<body>\r\n<div class=\"metadata\">\r\n");
            out_buf.Append($"<h1 class=\"title\">{header_info[HeaderElementTypeKind.title]}</h1>\r\n" + OutHeaderInfo(header_info, HeaderElementTypeKind.original_title) + OutHeaderInfo(header_info, HeaderElementTypeKind.subtitle) + OutHeaderInfo(header_info, HeaderElementTypeKind.original_subtitle) + OutHeaderInfo(header_info, HeaderElementTypeKind.author) + OutHeaderInfo(header_info, HeaderElementTypeKind.editor) + OutHeaderInfo(header_info, HeaderElementTypeKind.translator) + OutHeaderInfo(header_info, HeaderElementTypeKind.henyaku, "editor-translator"));
            out_buf.Append("<br />\r\n<br />\r\n</div>\r\n<div id=\"contents\" style=\"display:none\"></div><div class=\"main_text\">");
            return out_buf.ToString();
        }
    }
}



