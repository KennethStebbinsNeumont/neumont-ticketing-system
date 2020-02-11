using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Controllers.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException() : base()
        {
        }

        public DuplicateException(string? message) : base(message)
        {
        }

        public DuplicateException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public DuplicateException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class DuplicateException<T> : DuplicateException
    {
        public T Duplicate { get; private set; }

        public DuplicateException(T duplicate) : base()
        {
            Duplicate = duplicate;
        }

        public DuplicateException(T duplicate, string? message) : base(message)
        {
            Duplicate = duplicate;
        }

        public DuplicateException(T duplicate, string? message, Exception? innerException)
            : base(message, innerException)
        {
            Duplicate = duplicate;
        }

        public DuplicateException(T duplicate, System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            Duplicate = duplicate;
        }
    }
}
