using System;
using System.Collections.Generic;

namespace MediatorLib
{
    public class ValidationException : Exception
    {
        public ValidationException(IDictionary<string, string> errors)
        {
            Errors = errors;
        }

        public IDictionary<string, string> Errors { get; private set; }
    }
}