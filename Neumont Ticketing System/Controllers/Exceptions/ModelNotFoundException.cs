using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Controllers.Exceptions
{
    public class ModelNotFoundException : Exception
    {
        public ModelNotFoundException() : base()
        {
        }

        public ModelNotFoundException(string? message) : base(message)
        {
        }

        public ModelNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public ModelNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
