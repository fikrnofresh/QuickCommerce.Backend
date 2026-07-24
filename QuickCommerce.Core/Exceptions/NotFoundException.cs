using System;

namespace QuickCommerce.Core.Exceptions
{
    /// <summary>
    /// Thrown when a requested resource cannot be found.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}