using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Aozora.Exceptions
{
    [Serializable]
    public class TerminateException : Exception
    {
        public TerminateException()
        {
        }

        public TerminateException(string message) : base(message)
        {
        }

        public TerminateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TerminateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class AozoraException : Exception
    {
        public AozoraException() : base() { }
        public AozoraException(string message) : base(message) { }
        public AozoraException(string message, Exception innerException) : base(message, innerException) { }

        protected AozoraException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string GetMessageAozora(int n = 0)
        {
            //kurema:(?:)系の正規表現にミスがあったので、手軽で確実な置換に置き換えました。
            return string.Format(System.Text.RegularExpressions.Regex.Replace(Resources.Resource.ErrorStop, @"\r\n?", "\n").Replace("\n", "\r\n"), n, this.Message);
        }
    }

    [Serializable]
    public class UseCRLFException : AozoraException
    {
        public UseCRLFException() : base(Resources.Resource.UseCrlf) { }
        protected UseCRLFException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class AuthorTwiceException : AozoraException
    {
        public AuthorTwiceException() : base(Resources.Resource.AuthorTwice) { }
        protected AuthorTwiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class EncountUndefinedConditionException : AozoraException
    {
        public EncountUndefinedConditionException() : base(Resources.Resource.EncountUndefinedCondition) { }
        protected EncountUndefinedConditionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TagSyntaxException : AozoraException
    {
        public TagSyntaxException() : base(Resources.Resource.TagSyntaxError) { }
        protected TagSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidFontSizeException : AozoraException
    {
        public InvalidFontSizeException() : base(Resources.Resource.InvalidFontSize) { }
        protected InvalidFontSizeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UndefinedHeaderException : AozoraException
    {
        public UndefinedHeaderException() : base(Resources.Resource.UndefinedHeader) { }
        protected UndefinedHeaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontAllowTripleRubyException : AozoraException
    {
        public DontAllowTripleRubyException() : base(Resources.Resource.DontAllowTripleRuby) { }
        protected DontAllowTripleRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontUseDoubleRubyException : AozoraException
    {
        public DontUseDoubleRubyException() : base(Resources.Resource.DontUseDoubleRuby) { }
        protected DontUseDoubleRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TerminateInStyleException : AozoraException
    {
        public TerminateInStyleException(string arg1) : base(string.Format(Resources.Resource.TerminateInStyle, arg1)) { }
        protected TerminateInStyleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidClosingException : AozoraException
    {
        public InvalidClosingException(string arg1) : base(string.Format(Resources.Resource.InvalidClosing, arg1)) { }
        protected InvalidClosingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UnsupportedRubyException : AozoraException
    {
        public UnsupportedRubyException() : base(Resources.Resource.UnsupportedRuby) { }
        protected UnsupportedRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontCrlfInStyleException : AozoraException
    {
        public DontCrlfInStyleException(string arg1) : base(string.Format(Resources.Resource.DontCrlfInStyle, arg1)) { }
        protected DontCrlfInStyleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidNestingException : AozoraException
    {
        public InvalidNestingException(string arg1, string arg2) : base(string.Format(Resources.Resource.InvalidNesting, arg1, arg2)) { }
        protected InvalidNestingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
