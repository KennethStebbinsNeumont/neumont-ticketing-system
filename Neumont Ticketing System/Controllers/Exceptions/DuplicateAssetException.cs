using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Controllers.Exceptions
{
    public class DuplicateAssetException : Exception
    {
        public Asset Asset { get; private set; }

        public DuplicateAssetException(Asset asset) : base()
        {
            Asset = asset;
        }

        public DuplicateAssetException(Asset asset, string? message) : base(message)
        {
            Asset = asset;
        }

        public DuplicateAssetException(Asset asset, string? message, Exception? innerException) 
            : base(message, innerException)
        {
            Asset = asset;
        }

        public DuplicateAssetException(Asset asset, System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) 
            : base(info, context)
        {
            Asset = asset;
        }
    }
}
