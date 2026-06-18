using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BankApp.Domain.Infrastructure
{
 
        // base class for value objects that represent enumerations, such as account types, etc. - not used very often
        // when we want to make value objects that can act like enumerations, we can use this as a base class and then implement the logic to make them act like enumerations, such as having a list of possible values, etc.
        // when you need an enumeration that has more complex information attached to each option, such as number, name, but also a long description, or maybe a discount percentage, etc.

        // ask Phil for the boiler plate code he uses for this and the other value objects and entity base classes

    
    public abstract class EnumValueObject<TEnumeration, TId> : ComparableValueObject
        where TEnumeration : EnumValueObject<TEnumeration, TId>
        where TId : struct, IComparable
    {
        private int? _cachedHashCode;

        private static readonly Dictionary<TId, TEnumeration> EnumerationsById = GetEnumerations().ToDictionary(e => e.Id);
        private static readonly Dictionary<string, TEnumeration> EnumerationsByName = GetEnumerations().ToDictionary(e => e.Name);

        protected EnumValueObject(TId id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name cannot be null or empty");
            }

            Id = id;
            Name = name;
        }

        public TId Id { get; protected set; }

        public string Name { get; protected set; }

        public static bool operator ==(EnumValueObject<TEnumeration, TId> a, TId b)
        {
            if (a is null)
            {
                return false;
            }

            return a.Id.Equals(b);
        }

        public static bool operator !=(EnumValueObject<TEnumeration, TId> a, TId b)
        {
            return !(a == b);
        }

        public static bool operator ==(TId a, EnumValueObject<TEnumeration, TId> b)
        {
            return b == a;
        }

        public static bool operator !=(TId a, EnumValueObject<TEnumeration, TId> b)
        {
            return !(b == a);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (GetUnproxiedType(this) != GetUnproxiedType(obj))
                return false;

            var enumValueObject = (EnumValueObject<TEnumeration, TId>)obj;

            return GetEqualityComponents().SequenceEqual(enumValueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
            {
                _cachedHashCode = GetEqualityComponents()
                    .Aggregate(1, (current, obj) =>
                    {
                        unchecked
                        {
                            return current * 23 + (obj?.GetHashCode() ?? 0);
                        }
                    });
            }

            return _cachedHashCode.Value;
        }

        public static TEnumeration? FromId(TId id)
        {
            return EnumerationsById.TryGetValue(id, out TEnumeration? value) ? value : null;
        }

        public static TEnumeration? FromName(string name)
        {
            return EnumerationsByName.TryGetValue(name, out TEnumeration? value) ? value : null;
        }

        public static IReadOnlyCollection<TEnumeration> All = EnumerationsById.Values.OfType<TEnumeration>().ToList().AsReadOnly();

        public static bool Is(string possibleName) => All.Select(e => e.Name).Contains(possibleName);

        public static bool Is(TId possibleId) => All.Select(e => e.Id).Contains(possibleId);

        public override string ToString() => Name;

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Id;
        }

        private static TEnumeration[] GetEnumerations()
        {
            var enumerationType = typeof(TEnumeration);

            return enumerationType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => info.FieldType == typeof(TEnumeration))
                .Select(info => info.GetValue(null) as TEnumeration)
                .Where(value => value is not null)
                .ToArray()!;
        }
    }

    public abstract class EnumValueObject<TEnumeration>(int id, string name) : EnumValueObject<TEnumeration, int>(id, name)
        where TEnumeration : EnumValueObject<TEnumeration, int>
    { }
}