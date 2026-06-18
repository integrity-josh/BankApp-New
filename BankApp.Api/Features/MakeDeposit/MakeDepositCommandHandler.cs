using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            // return new MakeDepositResult
            // {
            //     CustomerId = command.CustomerId,
            //     AccountId = command.AccountId,
            //     Balance = 300.00m,
            //     Succeeded = true

            // };
            var customer = await _customerRepository.GetByIdAsync(command.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with Id {command.CustomerId} not found.");
            }

            customer.MakeDeposit(command.AccountId, command.Amount); // add this here - this will call the MakeDeposit method in the customer class, which will find the account and then call the Deposit method in the account class, which will update the balance

            await _customerRepository.SaveChangesAsync(); // add this here - this will call the SaveAsync method in the base class, which will call the SaveAsync method in the repository, which will save the changes to the database

            return new MakeDepositResult
            {
                CustomerId = customer.Id,
                AccountId = command.AccountId, // this is just the accountId from the request, we could also get it from the account that we find in the MakeDeposit method, but just personal preference to use the one from the request here
                    // it's fine to just get it from the command, because if the MakeDeposit failed, we wouldn't even get to this point, so we already have confirmation that the accountid from the command is the correct one based on that succeeding
                Balance = customer.GetAccountBalance(command.AccountId), // could also just do something based on the account - access the account from the accountId, then call the accounts deposit method, get the balance from a method there - just personal preference/up to the team to decide which is preferred
                // could also instead have the above MakeDeposit method return a balance when it completes, so we could do var balance = customer.MakeDeposit(...)
                Succeeded = true

            };
        }
    }
}