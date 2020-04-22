using System;

namespace BiblosDS.Library.Common.Exceptions
{
    public class AttributeNotFoundException : Exception
    {

        public AttributeNotFoundException(string message)
            : base(message) { }
        public AttributeNotFoundException(string format, params object[] args)
            : base(string.Format(format, args)) { }

    }
}
