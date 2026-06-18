using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BankApp.Api.Features.MakeDeposit
{
    public class MakeDepositRequest : IRequest<MakeDepositResult>
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}