using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using BankApp.Api.Features.CreateAccount;
using BankApp.Domain.Repositories;
using BankApp.Domain.Entities;
using BankApp.Domain.Exceptions;
using BankApp.Domain.Specifications;
using BankApp.Domain.ValueObjects;
using NSubstitute;

namespace BankApp.Tests.Features.CreateAccount
{
    public class When_creating_an_account
    {
        public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
        public static CreateAccountResult Result { get; set; }
        public static CreateAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }
        public static Customer TestCustomer { get; set; }

        public When_creating_an_account()
        {
            TestCustomer = new Customer(
                new PersonName("Test", "Customer"),
                []
            );

            Request = new CreateAccountRequest
            {
                CustomerId = TestCustomer.Id,
                AccountType = AccountType.Savings,
                InitialDeposit = new Money(200.00m)
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(TestCustomer));

            CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_account_should_be_created_for_the_customer()
        {
            Result = await CreateAccountCommandHandler.Handle(Request, CancellationToken.None);

            TestCustomer.Accounts.Should().HaveCount(1);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success()
        {
            Result = await CreateAccountCommandHandler.Handle(Request, CancellationToken.None);

            Result.Succeeded.Should().Be(true);
        }
    }

    public class When_creating_an_account_and_the_customer_does_not_exist
    {
        public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
        public static CreateAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_creating_an_account_and_the_customer_does_not_exist()
        {
            Request = new CreateAccountRequest
            {
                CustomerId = 1,
                AccountType = AccountType.Savings,
                InitialDeposit = new Money(200.00m)
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult<Customer?>(null));

            CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_customer_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }
    }

    public class When_creating_an_account_and_the_initial_deposit_is_less_than_100
    {
        public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
        public static CreateAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_creating_an_account_and_the_initial_deposit_is_less_than_100()
        {
            var existingSavingsAccount = new Account(new Money(200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"),
                [existingSavingsAccount]
            );

            Request = new CreateAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountType = AccountType.Checking,
                InitialDeposit = new Money(50.00m)
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<DomainException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_initial_deposit_must_be_at_least_100()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
            exception.ParamName.Should().Be("initialDeposit");
        }
    }

    public class When_creating_an_account_and_the_account_type_does_not_exist
    {
        public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
        public static CreateAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_creating_an_account_and_the_account_type_does_not_exist()
        {
            var existingSavingsAccount = new Account(new Money(200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"),
                [existingSavingsAccount]
            );

            Request = new CreateAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountType = null,
                InitialDeposit = new Money(200.00m)
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_type_does_not_exist()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }
    }

    public class When_creating_an_account_and_it_is_the_customers_first_account_and_the_account_type_is_not_savings
    {
        public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
        public static CreateAccountRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_creating_an_account_and_it_is_the_customers_first_account_and_the_account_type_is_not_savings()
        {
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"),
                []
            );

            Request = new CreateAccountRequest
            {
                CustomerId = testCustomer.Id,
                AccountType = AccountType.Checking,
                InitialDeposit = new Money(200.00m)
            };

            CustomerRepository = Substitute.For<IRepository<Customer>>();
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<DomainException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_first_account_must_be_savings()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() => CreateAccountCommandHandler.Handle(Request, CancellationToken.None));
            exception.ParamName.Should().Be("accountType");
        }
    }

// removed the following and tested it in the happy path test above, because it is not a separate test case, it is just part of the happy path
    // public class When_creating_an_account_and_it_is_the_customers_first_account_and_the_account_type_is_savings
    // {
    //     public static CreateAccountCommandHandler CreateAccountCommandHandler { get; set; }
    //     public static CreateAccountResult Result { get; set; }
    //     public static CreateAccountRequest Request { get; set; }
    //     public static IRepository<Customer> CustomerRepository { get; set; }
    //     public static Customer TestCustomer { get; set; }

    //     public When_creating_an_account_and_it_is_the_customers_first_account_and_the_account_type_is_savings()
    //     {
    //         TestCustomer = new Customer(
    //             new PersonName("Test", "Customer"),
    //             []
    //         );

    //         Request = new CreateAccountRequest
    //         {
    //             CustomerId = TestCustomer.Id,
    //             AccountType = AccountType.Savings,
    //             InitialDeposit = new Money(200.00m)
    //         };

    //         CustomerRepository = Substitute.For<IRepository<Customer>>();
    //         CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(TestCustomer));

    //         CreateAccountCommandHandler = new CreateAccountCommandHandler(CustomerRepository);
    //     }

    //     [Fact]
    //     public async Task The_account_should_be_created()
    //     {
    //         Result = await CreateAccountCommandHandler.Handle(Request, CancellationToken.None);

    //         TestCustomer.Accounts.Should().HaveCount(1);
    //     }

    //     [Fact]
    //     public async Task The_caller_should_be_notified_of_success()
    //     {
    //         Result = await CreateAccountCommandHandler.Handle(Request, CancellationToken.None);

    //         Result.Succeeded.Should().Be(true);
    //     }
    // }
}
