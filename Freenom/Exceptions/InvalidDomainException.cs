
namespace Freenom
{
    using System;

    public class InvalidDomainException : Exception
    {
        public InvalidDomainException() : base() { }

        public InvalidDomainException(string message) : base(message) { }
    }
}
