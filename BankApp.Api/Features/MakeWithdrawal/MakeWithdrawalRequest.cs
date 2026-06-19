using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.ValueObjects;
using MediatR;

namespace BankApp.Api.Features.MakeWithdrawal
{
    public class MakeWithdrawalRequest : IRequest<MakeWithdrawalResult>
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; } // decimal here instead of money so error handling can be done in the API layer, and we can return a bad request if the amount is invalid, rather than having it throw an exception in the domain layer
    }
}