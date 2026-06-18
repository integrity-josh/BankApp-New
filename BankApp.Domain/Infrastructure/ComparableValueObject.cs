using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Domain.Infrastructure
{
    // base class for value objects that need to be compared, such as for sorting, etc. - not used very often
    // for sorting and comparing value objects, implements IComparable interface and puts in a scheme to compare properties of value objects (less than, greater, etc type stuff)
    
    // boiler code:
    [Serializable]
    public abstract class ComparableValueObject : ValueObject, IComparable, IComparable<ComparableValueObject>
    {
        protected abstract IEnumerable<IComparable> GetComparableEqualityComponents();

        protected sealed override IEnumerable<object> GetEqualityComponents() => GetComparableEqualityComponents();

        public virtual int CompareTo(ComparableValueObject? other)
        {
            if (other is null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            Type thisType = GetUnproxiedType(this);
            Type otherType = GetUnproxiedType(other);
            if (thisType != otherType)
                return string.Compare($"{thisType}", $"{otherType}", StringComparison.Ordinal);

            return
                GetComparableEqualityComponents().Zip(
                        other.GetComparableEqualityComponents(),
                        (left, right) =>
                            left?.CompareTo(right) ?? (right is null ? 0 : -1))
                    .FirstOrDefault(cmp => cmp != 0);
        }

        public virtual int CompareTo(object? other)
        {
            return CompareTo(other as ComparableValueObject);
        }
    }
}