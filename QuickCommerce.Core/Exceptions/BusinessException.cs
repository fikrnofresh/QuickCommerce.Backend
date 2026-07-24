using System;

namespace QuickCommerce.Core.Exceptions
{
    /// <summary>
    /// Represents a business rule violation.
    /// These exceptions are expected and are returned
    /// to the client as friendly API responses.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}