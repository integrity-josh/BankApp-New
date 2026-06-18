using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BankApp.Domain.Specifications;
using MediatR;

namespace BankApp.Api.Features.MakeDeposit
{
    public class MakeDepositCommandHandler: IRequestHandler<MakeDepositRequest, MakeDepositResult>
    {
        private readonly IRepository<Customer> _customerRepository;

        public MakeDepositCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // after implementing MediatR, we need to change the method signature to match the IRequestHandler interface, which requires us to implement a Handle method that takes in a MakeDepositRequest and returns a MakeDepositResult, and we also need to add the async keyword to the method signature, and we need to change the return type to Task<MakeDepositResult> to indicate that this is an asynchronous method that will return a MakeDepositResult when it completes
        // change from HandleAsync to Handle to match the IRequestHandler interface
        public async Task<MakeDepositResult> Handle(MakeDepositRequest command, CancellationToken cancellationToken)
        {
            // return new MakeDepositResult
            // {
            //     CustomerId = command.CustomerId,
            //     AccountId = command.AccountId,
            //     Balance = 300.00m,
            //     Succeeded = true

            // };
            var customerQuery = new GetSingleCustomerWithAccounts(command.CustomerId); // implement specification
            var customer = await _customerRepository.GetByIdAsync(customerQuery); 
            
            // var customer = await _customerRepository.GetByIdAsync(command.CustomerId);

            if (customer is null)
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