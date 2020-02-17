using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base()
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class NotFoundException<T> : NotFoundException
    {
        public Type Type => typeof(T);
        public string TypeName => typeof(T).Name;

        public NotFoundException() : base()
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
