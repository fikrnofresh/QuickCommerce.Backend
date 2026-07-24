using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Exceptions
{
    /// <summary>
    /// Represents validation failures in business requests.
    /// </summary>
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<string>();
        }

        public ValidationException(
            string message,
            List<string> errors)
            : base(message)
        {
            Errors = errors;
        }
    }
}