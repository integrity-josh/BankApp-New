using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BankApp.Domain.Specifications;
using MediatR;
using BankApp.Domain.ValueObjects;

namespace BankApp.Api.Features.MakeWithdrawal
{
    public class MakeWithdrawalCommandHandler: IRequestHandler<MakeWithdrawalRequest, MakeWithdrawalResult> // implement the IRequestHandler interface from MediatR
    {
        private readonly IRepository<Customer> _customerRepository;

        public MakeWithdrawalCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<MakeWithdrawalResult> Handle(MakeWithdrawalRequest command, CancellationToken cancellationToken)
        {
            var customerQuery = new GetSingleCustomerWithAccounts(command.CustomerId); // implement specification
            var customer = await _customerRepository.GetByIdAsync(customerQuery); 
            

            if (customer is null)
            {
                throw new KeyNotFoundException($"Customer with Id {command.CustomerId} not found.");
            }

            var amount = new Money(command.Amount); 

            customer.MakeWithdrawal(command.AccountId, amount); 

            await _customerRepository.SaveChangesAsync(); 

            return new MakeWithdrawalResult
            {
                CustomerId = customer.Id,
                AccountId = command.AccountId, 
                Balance = customer.GetAccountBalance(command.AccountId), 
                Succeeded = true

            };
        }
    }
}