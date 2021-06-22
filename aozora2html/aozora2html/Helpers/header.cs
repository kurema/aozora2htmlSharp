//using System;
//using System.Text.RegularExpressions;
//using System.Collections.Generic;

//namespace Aozora.Helpers
//{

//    // encoding: utf-8
//    public class Header
//    {
//        private List<string> header;

//        public Header()
//        {
//            header = new List<string>();
//        }


//        public void push(string line)
//        {
//            header.Add(line);
//        }


//        public string out_header_info(Dictionary<string,string> hash, string attr, string? true_name = null)
//        {
//            if (hash.ContainsKey(attr))
//            {
//                var found = hash[attr];
//                return $"<h2 class=\"{true_name ?? attr}\">{found}</h2>\r\n";
//            }
//            else
//            {
//                return "";
//            }
//        }


//        public dynamic header_element_type(string text)
//        {
//            bool original = true;
//            foreach (var ch in text)
//            {
//                dynamic code = ch.unpack("H*")[0];
//                if (("00" <= code && code <= "7f") || ("8140" <= code && code <= "8258") || ("839f" <= code && code <= "8491"))
//                    {
//                    continue;
//                }
//                    else
//                {
//                    original = false;
//                    break;
//                }
//            }
//            if (original)
//            {
//                //:original
//            }
//            else if (Aozora2Html.PAT_EDITOR.IsMatch(text))
//            {
//                //:editor elsif string.match(PAT_HENYAKU)
//                //:henyaku
//            }
//            else if (Aozora2Html.PAT_TRANSLATOR.IsMatch(text))
//            {
//                //:translator end
//            }
//        }


//            public dynamic process_person(dynamic string, dynamic header_info)
//            {
//                dynamic type = header_element_type(string);
//                switch (type)
//                {
//                    case : editor header_info[:editor] = string:
//                        break;
//                        case :translator header_info[:translator] = string:
//                        break;
//                        case ":henyaku":
//                            //header_info[:henyaku] = string
//                        break;
//            default:
//                            dynamic type = :author header_info[:author] = string;
//            break;
//        }
//        //type
//    }


//    public dynamic build_title(dynamic header_info)
//    {
//        dynamic buf = [:author, :translator, :editor, :henyaku,;
//        //:title, :original_title,
//        //:subtitle, :original_subtitle].map{|item| header_info[item]}.compact
//        dynamic buf_str = buf.join(" ");
//        //"<title>#{buf_str}</title>"
//    }


//    public dynamic build_header_info()
//    {
//        dynamic header_info = {:title => @header[0]};
//                    switch (@header.length)
//                    {
//                        case 2:
//                            //process_person(@header[1], header_info)
//                        break;
//                        case 3:
//                            if (header_element_type(@header[1]) == :original)
//                            {
//                                //header_info[:original_title] = @header[1]
//                                //process_person(@header[2], header_info)
//                            }
//                            else if (process_person(@header[2], header_info) == :author header_info[:subtitle] = @header[1])
//                            {
//}
//                            else
//{
//    //header_info[:author] = @header[1]
//}
//break;
//                        case 4:
//                            if (header_element_type(@header[1]) == :original)
//                            {
//    //header_info[:original_title] = @header[1]
//}
//                            else
//{
//    //header_info[:subtitle] = @header[1]
//}
//if (process_person(@header[3], header_info) == :author header_info[:subtitle] = @header[2])
//                            {
//}
//                            else
//{
//    //header_info[:author] = @header[2]
//}
//break;
//                        case 5:
//                            //header_info[:original_title] = @header[1]
//                            //header_info[:subtitle] = @header[2]
//                            //header_info[:author] = @header[3]
//                            if (process_person(@header[4], header_info) == :author raise Aozora2Html.Error, "parser encounted author twice")
//                            {
//}
//break;
//                        case 6:
//                            //header_info[:original_title] = @header[1]
//                            //header_info[:subtitle] = @header[2]
//                            //header_info[:original_subtitle] = @header[3]
//                            //header_info[:author] = @header[4]
//                            if (process_person(@header[5], header_info) == :author raise Aozora2Html.Error, "parser encounted author twice")
//                            {
//}
//break;
//                    }
//                    //header_info
//                }
                
                
//                public dynamic to_html()
//{
//    dynamic header_info = build_header_info();

//    // <title> 行を構築
//    dynamic html_title = build_title(header_info);

//    // 出力
//    List<string> out_buf = new List<string>();
//    //out_buf.push("<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n<!DOCTYPE html PUBLIC \"-W3CDTD XHTML 1.1EN\"\r\n    \"http:www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\r\n<html xmlns=\"http:www.w3.org/1999/xhtml\" xml:lang=\"ja\" >\r\n<head>\r\n	<meta http-equiv=\"Content-Type\" content=\"text/html;charset=Shift_JIS\" />\r\n	<meta http-equiv=\"content-style-type\" content=\"text/css\" />\r\n")
//    foreach (var css in css_files)
//    {
//        //out_buf.push("\t<link rel=\"stylesheet\" type=\"text/css\" href=\"" + css + "\" />\r\n")
//    }
//    //out_buf.push("\t#{html_title}\r\n	<script type=\"text/javascript\" src=\"../../jquery-1.4.2.min.js\"></script>\r\n  <link rel=\"Schema.DC\" href=\"http:purl.org/dc/elements/1.1/\" />\r\n	<meta name=\"DC.Title\" content=\"#{header_info[:title]}\" />\r\n	<meta name=\"DC.Creator\" content=\"#{header_info[:author]}\" />\r\n	<meta name=\"DC.Publisher\" content=\"#{AOZORABUNKO}\" />\r\n</head>\r\n<body>\r\n<div class=\"metadata\">\r\n")
//    //out_buf.push("<h1 class=\"title\">#{header_info[:title]}</h1>\r\n" + out_header_info(header_info, :original_title) + out_header_info(header_info, :subtitle) + out_header_info(header_info, :original_subtitle) + out_header_info(header_info, :author) + out_header_info(header_info, :editor) + out_header_info(header_info, :translator) + out_header_info(header_info, :henyaku, "editor-translator"))
//    //out_buf.push("<br />\r\n<br />\r\n</div>\r\n<div id=\"contents\" style=\"display:none\"></div><div class=\"main_text\">")
//    //out_buf.join("")
//}
                
//            }
//        }



