
namespace Freenom
{
    using System;

    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base() { }

        public AuthorizationException(string message) : base(message) { }
    }
}
