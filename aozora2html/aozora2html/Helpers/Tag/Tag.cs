using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //block_allowed_contextを実装後次の行をコメントインしてください。
        if (!parser.block_allowed_context) syntax_error();
    }

    public string close_tag() => "</div>";
}

public class Indent : Block
{
    public Indent(Aozora2Html parser) : base(parser)
    {
    }
}

public class Chitsuki : Indent
{
    public int length { get; }

    public Chitsuki(Aozora2Html parser, int length) : base(parser)
    {
        this.length = length;
    }

    public override string ToString()
    {
        return $"<div class=\"chitsuki_{length}\" style=\"text-align:right; margin-right: {length}em\">";
    }
}

//濁点つきカタカナ用
public class DakutenKatakana : Inline
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

    public override string ToString()
    {
        //kurema:これはU+FF9Eで実現できるのでは？
        //kurema:まぁでもsjis変換で壊れるようなことはしない方が良いか。
        return $"<img src=\"{gaiji_dir}/1-07/1-07-8{n}.png\" alt=\"※(濁点付き片仮名「{katakana}」、1-07-8{n})\" class=\"gaiji\" />";
    }
}
