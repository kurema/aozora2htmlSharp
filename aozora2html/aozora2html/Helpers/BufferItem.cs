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

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class BufferItemString : IBufferItem
    {

        public BufferItemString(string text)
        {
            Buffer = new StringBuilder(text);
        }

        public StringBuilder Buffer { get; }

        public void Append(string value)
        {
            Buffer.Append(value);
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
    }
}

