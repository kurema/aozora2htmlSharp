using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Aozora.Helpers;

//kurema:
//TextFragmentはJstreamStringで使われます。
//JstreamStringはパフォーマンス上のメリットが特にないのでお勧めしません。
//コンソールオプションからも削除しました。

public interface ITextFragment
{
    char? Char { get; }
    bool IsEmpty { get; }
    ReadOnlyMemory<char> AsMemory();
    bool TryGetAppended(ITextFragment? other, out ITextFragment? result);
    int Length { get; }
}

public class TextFragmentMemory : ITextFragment
{
    static TextFragmentMemory? _Empty;
    public static TextFragmentMemory Empty => _Empty ??= new TextFragmentMemory();

    public static TextFragmentMemory FromFirstChar(ReadOnlyMemory<char> @base) => new TextFragmentMemory(@base, 0, 1);
    public static TextFragmentMemory FromFirstChar(string text) => new TextFragmentMemory(text.AsMemory(), 0, 1);

    public TextFragmentMemory()
    {
        Base = ReadOnlyMemory<char>.Empty;
        Position = 0;
        Length = 0;
    }

    public TextFragmentMemory(ReadOnlyMemory<char> @base, int position = 0) : this(@base, position, @base.Length - position)
    {
    }

    public TextFragmentMemory(ReadOnlyMemory<char> @base, int position, int length)
    {
        Base = @base;
        Position = position;
        if (Length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        Length = length;
    }

    public ReadOnlyMemory<char> AsMemory() => Base.Slice(Position, Length);

    public ReadOnlyMemory<char> Base { get; }

    public int Position { get; }
    public int Length { get; }

    public bool IsEmpty => Length == 0;

    public char? Char => Length == 1 && Position < Base.Length ? Base.Span[Position] : null;

    public bool TryGetAppended(ITextFragment? other, out ITextFragment? result)
    {
        if (other is null || other.IsEmpty)
        {
            result = this;
            return true;
        }

        if (this.IsEmpty)
        {
            result = other;
            return true;
        }
        if (other is TextFragmentMemory otherString)
        {
            if (!Base.Equals(otherString.Base))
            {
                result = null;
                return false;
            }
            if (Position + Length == otherString.Position)
            {
                result = new TextFragmentMemory(Base, Position, Length + other.Length);
                return true;
            }
        }
        {
            result = null;
            return false;
        }
    }

    public override string ToString()
    {
        return AsMemory().ToString();
    }
}

public class TextFragmentChar : ITextFragment
{
    public TextFragmentChar(char @char)
    {
        Char = @char;
    }

    public char? Char { get; }

    public bool IsEmpty => false;

    public ReadOnlyMemory<char> AsMemory() => new string(Char!.Value, 1).AsMemory();

    public int Length => 1;

    public bool TryGetAppended(ITextFragment? other, out ITextFragment? result)
    {
        if (other is null || other.IsEmpty)
        {
            result = this;
            return true;
        }
        result = null;
        return false;
    }

    public override string ToString()
    {
        return Char?.ToString() ?? string.Empty;
    }
}

public class TextFragmentStringBuilder : ITextFragment
{
    public TextFragmentStringBuilder() : this(new StringBuilder())
    {
    }

    public TextFragmentStringBuilder(StringBuilder content, ITextFragment? buffer = null)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Buffer = buffer ?? TextFragmentMemory.Empty;
    }

    StringBuilder Content { get; }
    ITextFragment Buffer;


    public char? Char
    {
        get
        {
            Flush();
            return Length == 1 ? Content.ToString()[0] : null;
        }
    }

    public bool IsEmpty => Length == 0;

    public ReadOnlyMemory<char> AsMemory() => Content.Length == 0 ? Buffer.AsMemory() : this.ToString().AsMemory();

    public override string ToString()
    {
        Flush();
        return Content.ToString();
    }

    public int Length => Content.Length + Buffer.Length;

    public void Flush()
    {
        Content.Append(Buffer.AsMemory());
        Buffer = TextFragmentMemory.Empty;
    }

    /// <summary>
    /// これは基本的に失敗します。Append()を使ってください。
    /// </summary>
    /// <param name="other"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryGetAppended(ITextFragment? other, out ITextFragment? result)
    {
        if (other is null || other.IsEmpty)
        {
            result = this;
            return true;
        }
        result = null;
        return false;
    }

    public void Append(ITextFragment? other)
    {
        if (other is null || other.IsEmpty)
        {
            return;
        }
        if (Buffer.TryGetAppended(other, out var text))
        {
            Buffer = text ?? TextFragmentMemory.Empty;
        }
        else
        {
            Flush();
            Buffer = other;
        }
        return;
    }

    public void Append(string other)
    {
        Flush();
        Content.Append(other);
    }

    public void Append(ReadOnlySpan<char> other)
    {
        Flush();
#if NET7_0_OR_GREATER
        Content.Append(other);
#else
        Content.Append(other.ToString());
#endif
    }

    public void Append(ReadOnlyMemory<char> other)
    {
        Flush();
        Content.Append(other);
    }

}
