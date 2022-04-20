using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers
{
    public interface IIndentStackItem
    {
    }

    public class IndentStackItemString : IIndentStackItem
    {
        public IndentStackItemString(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string Content { get; }
    }

    public class IndentStackItemIndentTypeKey : IIndentStackItem
    {
        public IndentStackItemIndentTypeKey(Aozora2Html.IndentTypeKey content)
        {
            Content = content;
        }

        public Aozora2Html.IndentTypeKey Content{get;}
    }
}
