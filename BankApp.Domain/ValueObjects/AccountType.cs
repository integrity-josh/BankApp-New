using System;
using System.Collections.Generic;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.ValueObjects
{
    public class AccountType : ComparableValueObject
    {
        private AccountType() { }

        private AccountType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static readonly AccountType Savings = new AccountType(0, "Savings");
        public static readonly AccountType Checking = new AccountType(1, "Checking");

        public int Id { get; private set; }
        public string Name { get; private set; } = default!;

        public static AccountType FromId(int id)
        {
            return id switch
            {
                0 => Savings,
                1 => Checking,
                _ => throw new ArgumentException($"Unknown account type id: {id}", nameof(id))
            };
        }

        protected override IEnumerable<IComparable> GetComparableEqualityComponents()
        {
            yield return Id;
            yield return Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
