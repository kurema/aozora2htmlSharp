using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public interface IBufferItem : Tag.IHtmlProvider
    {
    }

    public class BufferItemTag : IBufferItem
    {
        public Tag.Tag Content { get; }

        public BufferItemTag(Tag.Tag tag)
        {
            this.Content = tag;
        }

        public string ToHtml()
        {
            return Tag.Tag.GetHtml(Content);
        }
    }

    public class BufferItemString : IBufferItem
    {

        public BufferItemString(string text)
        {
            Buffer = new TextFragmentStringBuilder(new StringBuilder(text));
        }

        public BufferItemString(ReadOnlyMemory<char> text)
        {
            var sb = new StringBuilder();
            sb.Append(text);
            Buffer = new TextFragmentStringBuilder(sb);
        }

        public BufferItemString(ReadOnlySpan<char> text)
        {
            var sb = new StringBuilder();
#if NET7_0_OR_GREATER
            sb.Append(text);
#else
            sb.Append(text.ToString());
#endif
            Buffer = new TextFragmentStringBuilder(sb);
        }

        public TextFragmentStringBuilder Buffer { get; }

        public void Append(string value)
        {
            Buffer.Append(value);
        }

        public void Append(ReadOnlyMemory<char> value)
        {
            Buffer.Append(value);
        }

        public void Append(ReadOnlySpan<char> value)
        {
            Buffer.Append(value);
        }

        public void Append(ITextFragment fragment)
        {
            Buffer.Append(fragment);
        }

        public int Length => Buffer.Length;

        public string ToHtml()
        {
            return ToString();
        }

        public override string ToString()
        {
            return Buffer.ToString();
        }

        public ReadOnlyMemory<char> AsMemory()
        {
            return Buffer.AsMemory();
        }
    }
}

