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
        public Account(Money balance, AccountType accountType) // force them to enter a balance, account status, account type when creating an account, because this is core functionality and these should always be explicitly set, never null and never implicit
        { 
            Balance = balance;
            Type = accountType;
        }

        // public Account(Money balance, AccountStatus status, AccountType accountType)
        //     : this(balance, status)
        // {
        //     Type = accountType;
        // }
        

        public int Id { get; private set; } // once entity base class is set up we will inherit the Id property from it
        public Money Balance { get; private set; } // balance can be negative so not enforcing positivity here
        public AccountStatus Status { get; private set; } = AccountStatus.Open; // default to open when account is created, but can be changed to closed later on
        public AccountType Type { get; private set; }

        public void Deposit(Money amount)
        {
            // if (amount <= 0)
            
            // {
            //     throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            // } // this conditional can be here or in customer model's makedeposit method - here it is closer to the actual operation, but in customer it is caught earlier on
            Balance += amount; // add the deposit amount to the balance
        }

        public void Withdraw(Money amount)
        {
            Balance -= amount; 
        }

        public void Close()
        {
            Status = AccountStatus.Closed;
        }
    }
}