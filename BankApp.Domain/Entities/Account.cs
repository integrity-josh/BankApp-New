using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Infrastructure;
using BankApp.Domain.ValueObjects;

namespace BankApp.Domain.Entities
{
    public class Account : Entity
    {
        // constructor private so change can only be made through named methods in the class
        private Account() { }
        public Account(Money balance) // force them to enter a balance when creating an account, because it can't be null - null doesn't equal zero inherently
        {
            Balance = balance;
        }

        public int Id { get; private set; } // once entity base class is set up we will inherit the Id property from it
        public Money Balance { get; private set; } // balance can be negative so not enforcing positivity here

        public void Deposit(Money amount)
        {
            // if (amount <= 0)
            
            // {
            //     throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            // } // this conditional can be here or in customer model's makedeposit method - here it is closer to the actual operation, but in customer it is caught earlier on
            Balance += amount; // add the deposit amount to the balance
        }
    }
}