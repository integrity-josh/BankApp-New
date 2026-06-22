using System;
using System.Collections.Generic;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.ValueObjects
{
    public class AccountStatus : ComparableValueObject
    {
        private AccountStatus() { }

        private AccountStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static readonly AccountStatus Open = new AccountStatus(0, "Open");
        public static readonly AccountStatus Closed = new AccountStatus(1, "Closed");

        public int Id { get; private set; }
        public string Name { get; private set; }

        public static AccountStatus FromId(int id)
        {
            return id switch
            {
                0 => Open,
                1 => Closed,
                _ => throw new ArgumentException($"Unknown account status id: {id}", nameof(id))
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
