using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Exceptions;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        private Money() { } // privatize the no argument constructor - only my named methods can make changes
        public Money(decimal amount)
        {
            int scale = (decimal.GetBits(amount)[3] >> 16) & 31; // The expression (decimal.GetBits(amount)[3] >> 16) & 31 extracts the scale (number of decimal places) of a C# decimal

            if (scale > 2) // if there are more than 2 decimal places, throw an exception, because we don't want to allow that, since money should only have 2 decimal places
            { 
                throw new DomainException("Amount must have at most 2 decimal places.", nameof(amount));
            }
            

            Amount = amount;
        }

        public decimal Amount { get; } // remove private setter to make it immutable - once a Money object is created, its amount cannot be changed by anything

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }

        public static implicit operator decimal(Money money)
        {
            if (money is null)
                throw new ArgumentNullException(nameof(money)); // don't except null as zero! - reject null values!

            return money.Amount; 
        } // this allows us to use a Money object in place of a decimal, and it will automatically convert it to the decimal amount when we do so

        public static Money operator +(Money left, Money right)
        {
            return new Money(left.Amount + right.Amount);
        }

        public static Money operator -(Money left, Money right)
        {
            return new Money(left.Amount - right.Amount);
        }

        public static Money operator *(Money left, decimal multiplier)
        {
            decimal roundedAmount = decimal.Round(
                left.Amount * multiplier,
                2,
                MidpointRounding.ToEven
            ); // round to 2 decimal places using banker's rounding (MidpointRounding.ToEven) to keep 2 decimal places after multiplication

            return new Money(roundedAmount);
        }

        public static Money operator /(Money left, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Divisor cannot be zero.");

            decimal roundedAmount = decimal.Round(
                left.Amount / divisor,
                2,
                MidpointRounding.ToEven
            ); // round to 2 decimal places using banker's rounding (MidpointRounding.ToEven) to keep 2 decimal places after multiplication

            return new Money(roundedAmount);
        }

        public override string ToString()
        {
            return Amount.ToString("0.00", CultureInfo.InvariantCulture);
        }


    }
}