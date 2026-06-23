using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using BankApp.Api.Features.MakeWithdrawal;
using BankApp.Domain.Entities;
using BankApp.Domain.Repositories;
using BankApp.Domain.Specifications;
using BankApp.Domain.ValueObjects;
using NSubstitute;
using BankApp.Domain.Exceptions;

namespace BankApp.Tests.Features.MakeWIthdrawal
{
    public class When_making_a_withdrawal
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account (new Money (200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount] // aka new list<Account> { testAccount }
                );

            Request = new MakeWithdrawalRequest
            {
                CustomerId = testCustomer.Id, 
                AccountId = testAccount.Id,
                Amount = new Money(100.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); 
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));
                // when handler calls GetByIdAsync
                // on any GetSingleCustomerWithAccounts specification
                // return the testCustomer that we created above

            MakeWithdrawalCommandHandler = new MakeWithdrawalCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_balance_should_decrease_by_the_withdrawal_amount()
        {
            // Given (arrange) - implementation of business logic - this is what we expect to change
            
            var expectedBalance = 100.00m;

            Result = await MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None);

            // Then (result/assert)
            
            Result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success() // caller wording not customer just because we really don't know if the initiaator of this will be a customer or some other source/user
        {
            
            Result = await MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None);

            Result.Succeeded.Should().Be(true);
        }

    }
    public class When_making_a_withdrawal_and_the_account_does_not_exist
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal_and_the_account_does_not_exist()
        {
            // arrange/setup/initialization
            
            // var testAccount = new Account (200.00m);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
                );

            Request = new MakeWithdrawalRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = 1,
                Amount = new Money(100.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            MakeWithdrawalCommandHandler = new MakeWithdrawalCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            // Result = await MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None);
            // Result.Succeeded.Should().Be(false);

            // or?
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));

        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
        
            // QA - how to do this with fluid assertions?

            // Result = await MakeWithdrawalCommandHandler.Handle(Request);
            // Result.Should().BeOfType<KeyNotFoundException>();

        }
    }

    public class When_making_a_withdrawal_and_the_account_does_not_belong_to_the_customer
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal_and_the_account_does_not_belong_to_the_customer()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account (new Money (200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
                );

            Request = new MakeWithdrawalRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = testAccount.Id, // this account exists but does not belong to the customer
                Amount = new Money(100.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            // CustomerRepository.GetByIdAsync(Request.CustomerId).Returns(Task.FromResult<Customer?>(null));
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            MakeWithdrawalCommandHandler = new MakeWithdrawalCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));

        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
            // QA
            // should be told on this - don't want to check a the specific message, but we could check that the exception is a KeyNotFoundException and that it contains the accountId in the message
            // or should we add a paramname to the exception and check that (less specific than the message)
        }
    }

    public class When_making_a_withdrawal_and_the_amount_is_less_than_or_equal_to_zero
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal_and_the_amount_is_less_than_or_equal_to_zero()
        {
            // this is the arrange/setup/initialization for the test class, so that we don't have to repeat it in each test, and if we need to change it later we only have to change it in one place
            
            var testAccount = new Account (new Money (200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
                );

            Request = new MakeWithdrawalRequest
            {
                CustomerId = testCustomer.Id, 
                AccountId = testAccount.Id,
                Amount = new Money(0.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            // no longer able to do this with the specific customerId from the request, because we are now using a specification to get the customer in the handler
            // CustomerRepository.GetByIdAsync(Request.CustomerId).Returns(Task.FromResult(testCustomer));
                // when handler calls GetByIdAsync
                // with the specific customerId from the request
                // return the testCustomer that we created above
            // so we replace the above with this to match the new way we are getting the customer in the handler with the specification
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));
                // when handler calls GetByIdAsync
                // on any GetSingleCustomerWithAccounts specification
                // return the testCustomer that we created above

            MakeWithdrawalCommandHandler = new MakeWithdrawalCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_amount_must_be_above_zero()
        {
            // await Assert.ThrowsAsync<ArgumentException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
          
            // this and similar should_be_told tests can be done in a specific way to check the parameter name to make sure the error was for the right reason, but not too specific to the point that the specific message matters
            // should we do this at all or is this even too specific and we should jsut remove these should be told tests? 

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
            exception.ParamName.Should().Be("amount");
            // this does what we need for this test, but if we wanted to be more specific and check that the parameter name is also correct, we could do something like this instead:
            // exception.ParamName.Should().Be("amount");
            // this is specific, but it also ensures that the exception is being thrown for the correct reason
        }
    }
    public class When_making_a_withdrawal_and_the_amount_is_greater_than_the_balance
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal_and_the_amount_is_greater_than_the_balance()
        {
            // this is the arrange/setup/initialization for the test class, so that we don't have to repeat it in each test, and if we need to change it later we only have to change it in one place
            
            var testAccount = new Account (new Money (200.00m), AccountType.Savings);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
                );

            Request = new MakeWithdrawalRequest
            {
                CustomerId = testCustomer.Id, 
                AccountId = testAccount.Id,
                Amount = new Money(300.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); 
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));
                // when handler calls GetByIdAsync
                // on any GetSingleCustomerWithAccounts specification
                // return the testCustomer that we created above

            MakeWithdrawalCommandHandler = new MakeWithdrawalCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<DomainException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_amount_cannot_be_greater_than_the_balance()
        {
            var exception = await Assert.ThrowsAsync<DomainException>(() => MakeWithdrawalCommandHandler.Handle(Request, CancellationToken.None));
            exception.ParamName.Should().Be("amount");
            
            // QA ANSWER: for should be told - could do look for specific test, or could put error messages in a constants file so tests can refer to the constant instead of the strings of the error message, also could throw and check different types of exceptions (like subclasses of the domain exception)
                // if I did the above, probably wouldn't make domain exception a child of the argument exception
            
        }
    }
}