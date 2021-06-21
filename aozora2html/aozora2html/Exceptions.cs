using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Aozora2html.Exceptions
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
        public UseCRLFException() : base("改行コードを、「CR+LF」にあらためてください") { }
        protected UseCRLFException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
