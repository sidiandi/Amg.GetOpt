using System;
using System.Runtime.Serialization;

namespace Amg.GetOpt
{
    [Serializable]
#pragma warning disable S3871 // Exception types should be "public"
    internal class NoDefaultCommandException : Exception
#pragma warning restore S3871 // Exception types should be "public"
    {
        public NoDefaultCommandException()
        {
        }

        public NoDefaultCommandException(string message) : base(message)
        {
        }

        public NoDefaultCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoDefaultCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}