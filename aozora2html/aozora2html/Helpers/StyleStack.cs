using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Aozora.Helpers
{

    public record Style
    {
        public Style(string command, string closingTag)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.ClosingTag = closingTag ?? throw new ArgumentNullException(nameof(closingTag));
        }

        public string Command { get; set; }

        public string ClosingTag { get; set; }
    }

    public class StyleStack
    {
        public StyleStack()
        {
            stack = new List<Style>();
        }

        public void Push(Style elem)
        {
            stack.Add(elem);
        }

        public void Push(string command, string closingTag)
        {
            stack.Add(new Style(command, closingTag));
        }

        public bool IsEmpty => stack.Count == 0;

        public Style Pop()
        {
            var result = stack.Last();
            stack.Remove(result);
            return result;
        }

        public Style? Last()
        {
            return stack.LastOrDefault();
        }

        public string? LastCommand()
        {
            return stack.LastOrDefault()?.Command;
        }

        private readonly List<Style> stack;
    }
}

