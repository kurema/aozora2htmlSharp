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

        public void push(string line)
        {
            header.Add(line);
        }

        public string out_header_info(Dictionary<header_element_type_kind, string> hash, header_element_type_kind attr, string? true_name = null)
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

        public header_element_type_kind header_element_type(string text)
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
                return header_element_type_kind.original;
            }
            else if (Aozora2Html.PAT_EDITOR.IsMatch(text))
            {
                return header_element_type_kind.editor;
            }
            else if (Aozora2Html.PAT_HENYAKU.IsMatch(text))
            {
                return header_element_type_kind.henyaku;
            }
            else if (Aozora2Html.PAT_TRANSLATOR.IsMatch(text))
            {
                return header_element_type_kind.translator;
            }
            return header_element_type_kind.author;
        }

        public enum header_element_type_kind
        {
            editor, translator, henyaku, author, original, title, original_title, subtitle, original_subtitle
        }


        public header_element_type_kind process_person(string word, Dictionary<header_element_type_kind, string> header_info)
        {
            var type = header_element_type(word);
            switch (type)
            {
                case header_element_type_kind.editor:
                    header_info[header_element_type_kind.editor] = word;
                    break;
                case header_element_type_kind.translator:
                    header_info[header_element_type_kind.translator] = word;
                    break;
                case header_element_type_kind.henyaku:
                    header_info[header_element_type_kind.henyaku] = word;
                    break;
                case header_element_type_kind.author:
                default:
                    type = header_element_type_kind.author;
                    header_info[header_element_type_kind.author] = word;
                    break;
            }
            return type;
        }

        public string build_title(Dictionary<header_element_type_kind, string> header_info)
        {
            var buff = new[] {
            header_element_type_kind.author,header_element_type_kind.translator,header_element_type_kind.editor,header_element_type_kind.henyaku,
            header_element_type_kind.title,header_element_type_kind.original_title,
            header_element_type_kind.subtitle,header_element_type_kind.original_subtitle,
            }.Where(a => header_info.ContainsKey(a)).Select(a => header_info[a]);
            string buf_str = string.Join(" ", buff);
            return $"<title>{buf_str}</title>";
        }

        public Dictionary<header_element_type_kind, string> build_header_info()
        {
            //kurema: ヘッダーは行数によって順番が決まているみたい。
            var header_info = new Dictionary<header_element_type_kind, string>() { { header_element_type_kind.title, header[0] } };
            switch (header.Count)
            {
                case 2:
                    process_person(header[1], header_info);
                    break;
                case 3:
                    if (header_element_type(header[1]) == header_element_type_kind.original)
                    {
                        header_info[header_element_type_kind.original_title] = header[1];
                        process_person(header[2], header_info);
                    }
                    else if (process_person(header[2], header_info) == header_element_type_kind.author)
                    {
                        header_info[header_element_type_kind.subtitle] = header[1];
                    }
                    else
                    {
                        header_info[header_element_type_kind.author] = header[1];
                    }
                    break;
                case 4:
                    if (header_element_type(header[1]) == header_element_type_kind.original)
                    {
                        header_info[header_element_type_kind.original_title] = header[1];
                    }
                    else
                    {
                        header_info[header_element_type_kind.subtitle] = header[1];
                    }
                    if (process_person(header[3], header_info) == header_element_type_kind.author)
                    {
                        header_info[header_element_type_kind.subtitle] = header[2];
                    }
                    else
                    {
                        header_info[header_element_type_kind.author] = header[2];
                    }
                    break;
                case 5:
                    header_info[header_element_type_kind.original_title] = header[1];
                    header_info[header_element_type_kind.subtitle] = header[2];
                    header_info[header_element_type_kind.author] = header[3];
                    if (process_person(header[4], header_info) == header_element_type_kind.author)
                    {
                        throw new Exceptions.AuthorTwiceException();
                    }
                    break;
                case 6:
                    header_info[header_element_type_kind.original_title] = header[1];
                    header_info[header_element_type_kind.subtitle] = header[2];
                    header_info[header_element_type_kind.original_subtitle] = header[3];
                    header_info[header_element_type_kind.author] = header[4];
                    if (process_person(header[5], header_info) == header_element_type_kind.author)
                    {
                        throw new Exceptions.AuthorTwiceException();
                    }
                    break;
            }
            return header_info;
        }

        public string to_html()
        {
            var header_info = build_header_info();

            // <title> 行を構築
            var html_title = build_title(header_info);

            // 出力
            var out_buf = new System.Text.StringBuilder();
            out_buf.Append("<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n<!DOCTYPE html PUBLIC \"-W3CDTD XHTML 1.1EN\"\r\n    \"http:www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\r\n<html xmlns=\"http:www.w3.org/1999/xhtml\" xml:lang=\"ja\" >\r\n<head>\r\n	<meta http-equiv=\"Content-Type\" content=\"text/html;charset=Shift_JIS\" />\r\n	<meta http-equiv=\"content-style-type\" content=\"text/css\" />\r\n");
            foreach (var css in css_files)
            {
                out_buf.Append("\t<link rel=\"stylesheet\" type=\"text/css\" href=\"" + css + "\" />\r\n");
            }
            //kurema: jqueryへのリンクは青空文庫でホストする用なので修正が必要
            out_buf.Append($"\t{html_title}\r\n\t<script type=\"text/javascript\" src=\"../../jquery-1.4.2.min.js\"></script>\r\n  <link rel=\"Schema.DC\" href=\"http:purl.org/dc/elements/1.1/\" />\r\n	<meta name=\"DC.Title\" content=\"{header_info[header_element_type_kind.title]}\" />\r\n	<meta name=\"DC.Creator\" content=\"{header_info[header_element_type_kind.author]}\" />\r\n	<meta name=\"DC.Publisher\" content=\"{Aozora2Html.AOZORABUNKO}\" />\r\n</head>\r\n<body>\r\n<div class=\"metadata\">\r\n");
            out_buf.Append($"<h1 class=\"title\">{header_info[header_element_type_kind.title]}</h1>\r\n" + out_header_info(header_info, header_element_type_kind.original_title) + out_header_info(header_info, header_element_type_kind.subtitle) + out_header_info(header_info, header_element_type_kind.original_subtitle) + out_header_info(header_info, header_element_type_kind.author) + out_header_info(header_info, header_element_type_kind.editor) + out_header_info(header_info, header_element_type_kind.translator) + out_header_info(header_info, header_element_type_kind.henyaku, "editor-translator"));
            out_buf.Append("<br />\r\n<br />\r\n</div>\r\n<div id=\"contents\" style=\"display:none\"></div><div class=\"main_text\">");
            return out_buf.ToString();
        }
    }
}



