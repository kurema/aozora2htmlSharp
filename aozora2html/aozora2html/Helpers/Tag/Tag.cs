using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aozora.Helpers.Tag;

public enum CharType
{
    Else, Katankana,
}

public interface IClosable
{
    public string close_tag();
}

public interface ICharTypeProvider
{
    public CharType char_type { get; }
}

public interface IHtmlProvider
{
    public string to_html();

}

/*
require_relative 'tag/accent'
require_relative 'tag/gaiji'
require_relative 'tag/embed_gaiji'
require_relative 'tag/un_embed_gaiji'
require_relative 'tag/editor_note'
require_relative 'tag/oneline_indent'
require_relative 'tag/multiline'
require_relative 'tag/multiline_style'
require_relative 'tag/font_size'
require_relative 'tag/jizume'
require_relative 'tag/keigakomi'
require_relative 'tag/multiline_yokogumi'
require_relative 'tag/multiline_caption'
require_relative 'tag/multiline_midashi'
require_relative 'tag/jisage'
require_relative 'tag/oneline_jisage'
require_relative 'tag/multiline_jisage'
require_relative 'tag/oneline_chitsuki'
require_relative 'tag/multiline_chitsuki'
require_relative 'tag/midashi'
require_relative 'tag/ruby'
require_relative 'tag/kunten'
require_relative 'tag/kaeriten'
require_relative 'tag/okurigana'
require_relative 'tag/inline_keigakomi'
require_relative 'tag/inline_yokogumi'
require_relative 'tag/inline_caption'
require_relative 'tag/inline_font_size'
require_relative 'tag/decorate'
require_relative 'tag/dir'
require_relative 'tag/img'
 */

public class Tag : ICharTypeProvider
{
    public string inspect() { return ToString() ?? ""; }

    public virtual CharType char_type => CharType.Else;

    public void syntax_error()
    {
        throw new Exceptions.TagSyntaxException();
    }
}

public class Inline : Tag { }

public class Block : Tag, IClosable
{
    public Block(Aozora2Html parser) : base()
    {
        if (!parser.block_allowed_context) syntax_error();
    }

    public virtual string close_tag() => "</div>";
}

public class Indent : Block
{
    public Indent(Aozora2Html parser) : base(parser)
    {
    }
}

public class Chitsuki : Indent, IHtmlProvider
{
    public int length { get; }

    public Chitsuki(Aozora2Html parser, int length) : base(parser)
    {
        this.length = length;
    }

    public string to_html()
    {
        return $"<div class=\"chitsuki_{length}\" style=\"text-align:right; margin-right: {length}em\">";
    }
}

//濁点つきカタカナ用
public class DakutenKatakana : Inline, IHtmlProvider
{
    public DakutenKatakana(int num, string katakana, string gaiji_dir)
    {
        this.n = num;
        this.katakana = katakana ?? throw new ArgumentNullException(nameof(katakana));
        this.gaiji_dir = gaiji_dir ?? throw new ArgumentNullException(nameof(gaiji_dir));
    }

    public int n { get; }
    public string katakana { get; }
    public string gaiji_dir { get; }

    public override CharType char_type => CharType.Katankana;

    //kurema:これは後で実装。
    //using StringRefinements

    public string to_html()
    {
        //kurema:これはU+FF9Eで実現できるのでは？
        //kurema:まぁでもsjis変換で壊れるようなことはしない方が良いか。
        return $"<img src=\"{gaiji_dir}/1-07/1-07-8{n}.png\" alt=\"※(濁点付き片仮名「{katakana}」、1-07-8{n})\" class=\"gaiji\" />";
    }
}
