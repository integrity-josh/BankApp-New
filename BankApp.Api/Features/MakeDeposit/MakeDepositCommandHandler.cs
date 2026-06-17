using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;

namespace BankApp.Api.Features.MakeDeposit
{
    public class MakeDepositCommandHandler
    {
        private readonly IRepository<Customer> _customerRepository;

        public MakeDepositCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<MakeDepositResult> HandleAsync(MakeDepositRequest command)
        {
            return new MakeDepositResult
            {
                CustomerId = command.CustomerId,
                AccountId = command.AccountId,
                Balance = 300.00m,
                Succeeded = true

            };
        }
    }
}