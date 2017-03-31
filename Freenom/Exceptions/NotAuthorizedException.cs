
namespace Freenom
{
    using System;

    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException() : base() { }

        public NotAuthorizedException(string message) : base(message) { }
    }
}
