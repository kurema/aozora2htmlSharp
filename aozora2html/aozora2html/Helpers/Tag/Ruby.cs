using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers.Tag;

// complex ruby markup
// if css3 is major supported, please fix ruby position with property "ruby-position"
// see also: http://www.w3.org/TR/2001/WD-css3-ruby-20010216/

/// <summary>
/// ルビ用
/// 
/// 現状、under_rubyは無視しているのに注意
/// </summary>
public class Ruby : ReferenceMentioned, IHtmlProvider
{
    public string RubyUpper { get; set; }
    public string RubyUnder { get; set; }

    public Ruby(object? @string, string ruby, string under_ruby = "") : base(@string)
    {
        this.RubyUpper = ruby ?? throw new ArgumentNullException(nameof(ruby));
        this.RubyUnder = under_ruby ?? throw new ArgumentNullException(nameof(under_ruby));
    }

    public string ToHtml() => $"<ruby><rb>{TargetHtml}</rb><rp>{Aozora2Html.PAREN_BEGIN_MARK}</rp><rt>{RubyUpper}</rt><rp>{Aozora2Html.PAREN_END_MARK}</rp></ruby>";

    public static bool IncludeRuby(System.Collections.IEnumerable array)
    {
        static bool CaseReferenceMentioned(ReferenceMentioned referenceMentioned)
        {
            //kurema:ちょっと微妙だけど関数内関数化した。
            if (referenceMentioned.Target is System.Collections.IEnumerable eltArray) return IncludeRuby(eltArray);
            else return referenceMentioned.Target is Ruby;
        }

        if (array is null) return false;
        //kurema:
        //これはforeach相当で合ってる？
        //array.index do |elt|
        foreach (var elt in array)
        {
            switch (elt)
            {
                case BufferItemTag itemTag when itemTag.Content is Ruby:
                case Ruby: return true;
                case BufferItemTag itemTag when itemTag.Content is ReferenceMentioned eltRM1:
                    return CaseReferenceMentioned(eltRM1);
                case ReferenceMentioned eltRM2:
                    //kurema:原文通りだけど、falseの場合継続とかじゃなくて即returnで良いの？
                    return CaseReferenceMentioned(eltRM2);
            }
        }
        return false;
    }

    //rubyタグの再割り当て
    public static Ruby RearrangeRuby(System.Collections.IEnumerable targets, string upper_ruby, string under_ruby)
    {
        if (!IncludeRuby(targets))
        {
            return new Ruby(targets, upper_ruby, under_ruby);
        }

        var new_targets = new List<object?>();
        bool new_upper_is_array = string.IsNullOrEmpty(upper_ruby);
        var new_upper = new_upper_is_array ? new StringBuilder() : new StringBuilder(upper_ruby);
        bool new_under_is_array = string.IsNullOrEmpty(under_ruby);
        var new_under = new_under_is_array ? new StringBuilder() : new StringBuilder(under_ruby);

        if (new_upper.Length >= 1 && new_under.Length >= 1)
        {
            throw new Exceptions.DontAllowTripleRubyException();
        }

        void caseRuby(Ruby ruby, StringBuilder new_under, StringBuilder new_upper)
        {
            if (ruby.Target is System.Collections.IEnumerable) throw new Exceptions.DontUseDoubleRubyException();
            if (string.IsNullOrEmpty(ruby.RubyUpper))
            {
                if (!new_under_is_array) throw new Exceptions.DontUseDoubleRubyException();
                new_under.Append(ruby.RubyUnder);
            }
            else
            {
                if (!new_upper_is_array) throw new Exceptions.DontUseDoubleRubyException();
                new_upper.Append(ruby.RubyUpper);
            }
        }

        void caseMentioned(ReferenceMentioned mentioned, StringBuilder new_under, StringBuilder new_upper, List<object?> new_targets, object? x)
        {
            if (mentioned.Target is System.Collections.IEnumerable targetArray)
            {
                // recursive
                var ruby2 = RearrangeRuby(targetArray, "", "");
                var target2 = ruby2.Target;
                var upper_ruby2 = ruby2.RubyUpper;
                var under_ruby2 = ruby2.RubyUnder;
                // rotation!!
                if (ruby2.Target is not System.Collections.IEnumerable targetArray2) throw new Exception("Unexpected code path.");
                foreach (var y in targetArray2)
                {
                    ReferenceMentioned tmp = (ReferenceMentioned)mentioned.Clone();
                    tmp.Target = y;
                    new_targets.Add(tmp);
                }
                if (new_under_is_array) new_under.Append(under_ruby2);
                else if (under_ruby2.Length > 0) throw new Exceptions.DontUseDoubleRubyException();
                if (new_upper_is_array) new_upper.Append(upper_ruby2);
                else if (upper_ruby2.Length > 0) throw new Exceptions.DontUseDoubleRubyException();
            }
            else
            {
                new_targets.Add(x);
                //kurema:
                //空文字列を追加する意味はよく分からない。
                //https://github.com/aozorahack/aozora2html/blob/159698801a60e8d57c9b8e3818cd11d793caff52/lib/aozora2html/tag/ruby.rb#L85-L89
                if (new_under_is_array) new_under.Append("");
                if (new_upper_is_array) new_upper.Append("");
            }
        }

        foreach (var x in targets)
        {
            switch (x)
            {
                case BufferItemTag tag when tag.Content is Ruby ruby:
                    caseRuby(ruby, new_under, new_upper);
                    break;
                case Ruby ruby:
                    caseRuby(ruby, new_under, new_upper);
                    break;
                case ReferenceMentioned mentioned:
                    caseMentioned(mentioned, new_under, new_upper, new_targets, x);
                    break;
                default:
                    new_targets.Add(x);
                    if (new_under_is_array) new_under.Append("");
                    if (new_upper_is_array) new_upper.Append("");
                    break;
            }
        }
        return new Ruby(new_targets.ToArray(), new_upper.ToString(), new_under.ToString());
    }

    public override object Clone()
    {
        return new Ruby(Target, RubyUpper, RubyUnder);
    }

    /*
# ----------------------------------------------------
#
# def gen_rt(string)
#if string == ''
#     '<rt class="dummy_ruby"></rt>'
#else
#     "<rt class=\"real_ruby\">#{string}</rt>"
# end
# end
#
#
# complex ruby is waiting for IE support and CSS3 candidate
# def to_s
# ans = "<ruby class=\"complex_ruby\"><rbc>" # indicator of new version of aozora ruby
#if @ruby.is_a?(Array) and @ruby.length > 0
#       # cell is used
#       @rbspan = @ruby.length
# end
#if @under_ruby.is_a?(Array) and @under_ruby.length > 0
#       # cell is used, but two way cell is not supported
#if @rbspan
# raise Aozora2Html::Error, I18n.t(:unsupported_ruby)
#else
#         @rbspan = @under_ruby.length
# end
# end
#
#     # target
#if @rbspan
#       @target.each{|x|
# ans.concat("<rb>#{x.to_s}</rb>")
#       }
#else
# ans.concat("<rb>#{@target.to_s}</rb>")
# end
#
# ans.concat("</rbc><rtc>")
#
#     # upper ruby
#if @ruby.is_a?(Array)
#       @ruby.each{|x|
# ans.concat(gen_rt(x))
#       }
# elsif @rbspan
#if @ruby != ""
# ans.concat("<rt class=\"real_ruby\" rbspan=\"#{@rbspan}\">#{@ruby}</rt>")
#else
# ans.concat("<rt class=\"dummy_ruby\" rbspan=\"#{@rbspan}\"></rt>")
# end
#else
# ans.concat(gen_rt(@ruby))
# end
#
# ans.concat("</rtc>")
#
#     # under_ruby (if exists)
#if @under_ruby.length > 0
# ans.concat("<rtc>")
#if @under_ruby.is_a?(Array)
#         @under_ruby.each{|x|
# ans.concat(gen_rt(x))
#         }
# elsif @rbspan
# ans.concat("<rt class=\"real_ruby\" rbspan=\"#{@rbspan}\">#{@under_ruby}</rt>")
#else
# ans.concat(gen_rt(@under_ruby))
# end
# ans.concat("</rtc>")
# end
#
#     # finalize
# ans.concat("</ruby>")
#
# ans
# end
     */
}