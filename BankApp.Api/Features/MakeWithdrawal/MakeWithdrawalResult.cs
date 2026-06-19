using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.ValueObjects;

namespace BankApp.Api.Features.MakeWithdrawal
{
    public class MakeWithdrawalResult
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Balance { get; set; } 
        public bool Succeeded { get; set; }
    }
}