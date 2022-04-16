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
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.closingTag = closingTag ?? throw new ArgumentNullException(nameof(closingTag));
        }

        public string command { get; set; }

        public string closingTag { get; set; }
    }

    public class StyleStack
    {
        public StyleStack()
        {
            stack = new List<Style>();
        }

        public void push(Style elem)
        {
            stack.Add(elem);
        }

        public void push(string command, string closingTag)
        {
            stack.Add(new Style(command, closingTag));
        }

        public bool empty => stack.Count == 0;

        public Style pop()
        {
            var result = stack.Last();
            stack.Remove(result);
            return result;
        }

        public Style last()
        {
            return stack.Last();
        }

        public string last_command()
        {
            return stack.Last().command;
        }

        private List<Style> stack;
    }
}

