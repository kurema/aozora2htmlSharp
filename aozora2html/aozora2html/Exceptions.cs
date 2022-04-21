﻿using System;
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
    }

    [Serializable]
    public class UseCRLFException : AozoraException
    {
        public UseCRLFException() : base(Helpers.I18n.MSG["use_crlf"]) { }
        protected UseCRLFException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class AuthorTwiceException : AozoraException
    {
        public AuthorTwiceException() : base("parser encounted author twice") { }
        protected AuthorTwiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TagSyntaxException : AozoraException
    {
        public TagSyntaxException() : base(Helpers.I18n.MSG["tag_syntax_error"]) { }
        protected TagSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidFontSizeException : AozoraException
    {
        public InvalidFontSizeException() : base(Helpers.I18n.MSG["invalid_font_size"]) { }
        protected InvalidFontSizeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UndefinedHeaderException : AozoraException
    {
        public UndefinedHeaderException() : base(Helpers.I18n.MSG["undefined_header"]) { }
        protected UndefinedHeaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontAllowTripleRubyException : AozoraException
    {
        public DontAllowTripleRubyException() : base(Helpers.I18n.MSG["dont_allow_triple_ruby"]) { }
        protected DontAllowTripleRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontUseDoubleRubyException : AozoraException
    {
        public DontUseDoubleRubyException() : base(Helpers.I18n.MSG["dont_use_double_ruby"]) { }
        protected DontUseDoubleRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class TerminateInStyleException : AozoraException
    {
        public TerminateInStyleException(string arg1) : base(string.Format(Helpers.I18n.MSG["TerminateInStyleException"], arg1)) { }
        protected TerminateInStyleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidClosingException : AozoraException
    {
        public InvalidClosingException(string arg1) : base(string.Format(Helpers.I18n.MSG["invalid_closing"], arg1)) { }
        protected InvalidClosingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class ErrorStopException : AozoraException
    {
        public ErrorStopException(int arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["error_stop"], arg1, arg2)) { }
        protected ErrorStopException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UnsupportedRubyException : AozoraException
    {
        public UnsupportedRubyException() : base(Helpers.I18n.MSG["unsupported_ruby"]) { }
        protected UnsupportedRubyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class WarnOnebyteException : AozoraException
    {
        public WarnOnebyteException(int arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["warn_onebyte"], arg1, arg2)) { }
        protected WarnOnebyteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class WarnChukiException : AozoraException
    {
        public WarnChukiException(int arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["warn_chuki"], arg1, arg2)) { }
        protected WarnChukiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class WarnJisGaijiException : AozoraException
    {
        public WarnJisGaijiException(int arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["warn_jis_gaiji"], arg1, arg2)) { }
        protected WarnJisGaijiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DontCrlfInStyleException : AozoraException
    {
        public DontCrlfInStyleException(string arg1) : base(string.Format(Helpers.I18n.MSG["dont_crlf_in_style"], arg1)) { }
        protected DontCrlfInStyleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidNestingException : AozoraException
    {
        public InvalidNestingException(string arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["invalid_nesting"], arg1, arg2)) { }
        protected InvalidNestingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class WarnUnexpectedTerminatorException : AozoraException
    {
        public WarnUnexpectedTerminatorException(int arg1) : base(string.Format(Helpers.I18n.MSG["warn_unexpected_terminator"], arg1)) { }
        protected WarnUnexpectedTerminatorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class WarnUndefinedCommandException : AozoraException
    {
        public WarnUndefinedCommandException(int arg1, string arg2) : base(string.Format(Helpers.I18n.MSG["warn_undefined_command"], arg1, arg2)) { }
        protected WarnUndefinedCommandException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
