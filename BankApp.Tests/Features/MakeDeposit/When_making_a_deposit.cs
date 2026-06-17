using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using BankApp.Api.Features.MakeDeposit;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using NSubstitute;

namespace BankApp.Tests.Features.MakeDeposit
{
    public class When_making_a_deposit
    {
        public static MakeDepositCommandHandler MakeDepositCommandHandler { get; set; }
        public static MakeDepositResult Result { get; set; }
        public static MakeDepositRequest Request { get; set; }

        public static IRepository<Customer> CustomerRepository { get; set; }

        [Fact]
        public async Task The_balance_should_increase_by_the_deposit_amount()
        {
            // Given (arrange) - implementation of business logic - this is what we expect to change
            var command = new MakeDepositRequest
            {
                CustomerId = 5,
                AccountId = 17,
                Amount = 100.00m
            };
            var testAccount = new Account (200.00m);
            var testCustomer = new Customer([testAccount]);

            var expectedBalance = 300.00m;

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            CustomerRepository.GetByIdAsync(command.CustomerId).Returns(testCustomer);

            var MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);

            // When (action/act) - should almost never have to modify the act or assert, unless the expected business logic changes
            Result = await MakeDepositCommandHandler.HandleAsync(command); // can make Handle async, so lets do that, just because we can

            // Then (result/assert) - should abosolutely never have to modify the assert, unless the expected business logic changes
            Result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success() // caller wording not customer just because we really don't know if the initiaator of this will be a customer or some other source/user
        {
            var command = new MakeDepositRequest
            {
                CustomerId = 5,
                AccountId = 17,
                Amount = 100.00m
            };
            var testAccount = new Account (200.00m);
            var testCustomer = new Customer([testAccount]);


            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            CustomerRepository.GetByIdAsync(command.CustomerId).Returns(testCustomer);

            var MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);
            
            Result = await MakeDepositCommandHandler.HandleAsync(command);
            Result.Succeeded.Should().Be(true);
        }
    }

    
}