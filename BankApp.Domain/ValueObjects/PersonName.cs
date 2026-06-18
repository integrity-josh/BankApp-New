using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.ValueObjects
{
    public class PersonName : ValueObject
    {
        private PersonName() { } // privatize the no argument constructor - only my named methods can make changes
        public PersonName(string firstName, string lastName)
        {
            // validation for business logic - make sure first and last name are both entered and not just whitespace
            // could also put something like a length limit here, or whatever - but I put this in CustomerMapping instead, because this is more of a database concern than a domain concern, and we want to keep our domain as clean as possible from infrastructure concerns, but this is a bit of a gray area, so it's really up to preference
            if(string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name cannot be null or empty", nameof(firstName));
            }
            if(string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));
            }
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        // add this once value object base class is implemented/populated
        // protected override IEnumerable<object> GetEqualityComponents()
        // {
        //     throw new NotImplementedException(); // this is where we would implement the equality checks for the value object, by returning the values that should be checked for equality, so that the base class can implement the equality checks based on those values, but we haven't implemented it yet since we haven't decided how we want to handle equality for this value object yet, but when we do, we'll just return the first and last name here, since those are the values that should be checked for equality for a person name
           
        // }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}