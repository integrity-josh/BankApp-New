using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Domain.Infrastructure
{
    // boiler ValueObject code:
   [Serializable] //why? - this allows us to serialize the value object, which is necessary if we want to store it in a database or send it over the network, as it allows us to convert the object into a format that can be easily stored or transmitted, and then reconstruct it later when we need to use it again
        // why do we want value objects to be serializable? - because they are often used as properties of entities, and entities are often stored in databases or sent over the network, so we want to be able to serialize the value objects as well, so that we can store them in the database or send them over the network along with the entities that they belong to
    public abstract class ValueObject
    {
        // define what the values are to check for equality
        // public abstract IEnumerable<object> GetEqualityCOmponents()  - override this in each value object to return the values that should be checked for equality, then in the base class we can implement equality checks based on those values, so we don't have to implement them in each value object

        // allows us to get two addresses and see if they're equal by comparing their values, rather than their reference in memory, which is the default behavior in .NET for classes, but for value objects we want to compare their values instead

        // if we didn't care about checking for equality, we could just make valueobject class and it'd be pretty much empty and used as a marker
        private int? _cachedHashCode;

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (GetUnproxiedType(this) != GetUnproxiedType(obj))
                return false;

            var valueObject = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
            {
                _cachedHashCode = GetEqualityComponents().Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
            }

            return _cachedHashCode.Value;
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }

        internal static Type GetUnproxiedType(object obj)
        {
            const string EFCoreProxyPrefix = "Castle.Proxies.";
            const string NHibernateProxyPostfix = "Proxy";

            var type = obj.GetType();
            var typeString = type.ToString();

            if (typeString.Contains(EFCoreProxyPrefix) || typeString.EndsWith(NHibernateProxyPostfix))
                return type.BaseType ?? type;

            return type;
        }
    }
}