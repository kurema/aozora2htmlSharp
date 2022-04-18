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
        public Tag.Tag tag { get; }

        public BufferItemTag(Tag.Tag tag)
        {
            this.tag = tag;
        }

        public string to_html()
        {
            return Tag.Tag.GetHtml(tag);
        }
    }

    public class BufferItemString : IBufferItem
    {

        public BufferItemString(string text)
        {
            buffer = new StringBuilder(text);
        }

        public StringBuilder buffer { get; }

        public void Append(string value)
        {
            buffer.Append(value);
        }

        public int Length => buffer.Length;

        public string to_html()
        {
            return ToString();
        }

        public override string ToString()
        {
            return buffer.ToString();
        }
    }
}

