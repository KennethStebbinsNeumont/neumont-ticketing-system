using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() : base() { }

        public RoleNotFoundException(string? message) : base(message) { }

        public RoleNotFoundException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public RoleNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
