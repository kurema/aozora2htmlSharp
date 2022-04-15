using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Aozora.Exceptions
{
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
}
