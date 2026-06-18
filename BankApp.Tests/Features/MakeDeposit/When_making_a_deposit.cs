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
using BankApp.Domain.ValueObjects;

namespace BankApp.Tests.Features.MakeDeposit
{
    public class When_making_a_deposit
    {
        public static MakeDepositCommandHandler MakeDepositCommandHandler { get; set; }
        public static MakeDepositResult Result { get; set; }
        public static MakeDepositRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_deposit()
        {
            // this is the arrange/setup/initialization for the test class, so that we don't have to repeat it in each test, and if we need to change it later we only have to change it in one place
            
            var testAccount = new Account (200.00m);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
                );

            Request = new MakeDepositRequest
            {
                CustomerId = testCustomer.Id, 
                AccountId = testAccount.Id,
                Amount = 100.00m
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            CustomerRepository.GetByIdAsync(Request.CustomerId).Returns(testCustomer);

            MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_balance_should_increase_by_the_deposit_amount()
        {
            // Given (arrange) - implementation of business logic - this is what we expect to change
            // var command = new MakeDepositRequest
            // {
            //     CustomerId = 5,
            //     AccountId = 17,
            //     Amount = 100.00m
            // };
            // var testAccount = new Account (200.00m);
            // var testCustomer = new Customer([testAccount]);  // moved to constructor

            var expectedBalance = 300.00m;

            // CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            // CustomerRepository.GetByIdAsync(command.CustomerId).Returns(testCustomer);

            // var MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);

            // When (action/act) - should almost never have to modify the act or assert, unless the expected business logic changes
            Result = await MakeDepositCommandHandler.HandleAsync(Request); // can make Handle async, so lets do that, just because we can

            // Then (result/assert) - should abosolutely never have to modify the assert, unless the expected business logic changes
            Result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success() // caller wording not customer just because we really don't know if the initiaator of this will be a customer or some other source/user
        {
            
            Result = await MakeDepositCommandHandler.HandleAsync(Request);
            Result.Succeeded.Should().Be(true);
        }

    }
    public class When_making_a_deposit_and_the_account_does_not_exist
    {
        [Fact]
        public void The_caller_should_be_notified_of_failure()
        {

        }

        [Fact]
        public void The_caller_should_be_told_the_account_is_not_available()
        {
        }
    }

    public class When_making_a_deposit_and_the_account_does_not_belong_to_the_customer
    {
        [Fact]
        public void The_caller_should_be_notified_of_failure()
        {
        }

        [Fact]
        public void The_caller_should_be_told_the_account_is_not_available()
        {
        }
    }

    public class When_making_a_deposit_and_the_amount_is_less_than_or_equal_to_zero
    {
        [Fact]
        public void The_caller_should_be_notified_of_failure()
        {
        }

        [Fact]
        public void The_caller_should_be_told_the_amount_must_be_above_zero()
        {
        }
    }

    
}