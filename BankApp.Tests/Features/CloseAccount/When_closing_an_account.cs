using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using BankApp.Api.Features.CloseAccount;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using NSubstitute;
using BankApp.Domain.ValueObjects;
using BankApp.Domain.Specifications;

namespace BankApp.Tests.Features.CloseAccount
{
    public class When_closing_an_account
    {
        public static CloseAccountCommandHandler CloseAccountCommandHandler { get; set; }
        public static CloseAccountResult Result { get; set; }
        public static CloseAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        private readonly Account testAccount; // making this accessible to the test methods for status checking

        public When_closing_an_account()
        {
            // arrange/setup/initialization
            
            testAccount = new Account(new Money(0.00m), AccountType.Savings); 
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
            );

            Request = new CloseAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = testAccount.Id
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CloseAccountCommandHandler = new CloseAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_account_should_be_closed()
        {
            // Arrange - setup already done in constructor
            // Act
            await CloseAccountCommandHandler.Handle(Request, CancellationToken.None);
            // Assert
            testAccount.Status.Should().Be(AccountStatus.Closed);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success()
        {
            // Act
            Result = await CloseAccountCommandHandler.Handle(Request, CancellationToken.None);

            // Assert
            Result.Succeeded.Should().Be(true);
        }
    }

    public class When_closing_an_account_and_the_account_does_not_exist
    {
        public static CloseAccountCommandHandler CloseAccountCommandHandler { get; set; }
        public static CloseAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_closing_an_account_and_the_account_does_not_exist()
        {
            // arrange/setup/initialization
            
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
            );

            Request = new CloseAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = 1
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CloseAccountCommandHandler = new CloseAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
        }
    }

    public class When_closing_an_account_and_the_account_does_not_belong_to_the_customer
    {
        public static CloseAccountCommandHandler CloseAccountCommandHandler { get; set; }
        public static CloseAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_closing_an_account_and_the_account_does_not_belong_to_the_customer()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account(new Money(0.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
            );

            Request = new CloseAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = testAccount.Id // this account exists but does not belong to the customer
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CloseAccountCommandHandler = new CloseAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
            exception.Message.Should().Contain("Account");
        }
    }

    public class When_closing_an_account_and_the_account_balance_is_not_zero
    {
        public static CloseAccountCommandHandler CloseAccountCommandHandler { get; set; }
        public static CloseAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_closing_an_account_and_the_account_balance_is_not_zero()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account(new Money(100.00m), AccountType.Savings); // non-zero balance
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
            );

            Request = new CloseAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = testAccount.Id
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CloseAccountCommandHandler = new CloseAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_balance_must_be_zero()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => CloseAccountCommandHandler.Handle(Request, CancellationToken.None));
            exception.Message.Should().Contain("balance");
        }
    }
}
