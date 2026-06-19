using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Domain.Exceptions
{
    public class DomainException : ArgumentException
    {
        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, string paramName)
            : base(message, paramName)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    
    }
}