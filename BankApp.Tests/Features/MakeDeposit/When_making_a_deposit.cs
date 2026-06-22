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
using NSubstitute.ExceptionExtensions;
using BankApp.Domain.Specifications;

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
            
            var testAccount = new Account (new Money (200.00m), AccountStatus.Open);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
                );

            Request = new MakeDepositRequest
            {
                CustomerId = testCustomer.Id, 
                AccountId = testAccount.Id,
                Amount = new Money(100.00m)
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
            // Result = await MakeDepositCommandHandler.Handle(Request); // can make Handle async, so lets do that, just because we can
            Result = await MakeDepositCommandHandler.Handle(Request, CancellationToken.None);


            // Then (result/assert) - should abosolutely never have to modify the assert, unless the expected business logic changes
            Result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_success() // caller wording not customer just because we really don't know if the initiaator of this will be a customer or some other source/user
        {
            
            // Result = await MakeDepositCommandHandler.Handle(Request);
            Result = await MakeDepositCommandHandler.Handle(Request, CancellationToken.None);

            Result.Succeeded.Should().Be(true);
        }

    }
    public class When_making_a_deposit_and_the_account_does_not_exist
    {
        public static MakeDepositCommandHandler MakeDepositCommandHandler { get; set; }
        public static MakeDepositResult Result { get; set; }
        public static MakeDepositRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

// QA ANSWER: for this repeat code, could clean up with a base class, for arranging, setting up common test methods like given then when, etc... with deposit/withdrawal specific or wherever works for more test specific base as well
    // withsubject pattern - look into
        public When_making_a_deposit_and_the_account_does_not_exist()
        {
            // arrange/setup/initialization
            
            // var testAccount = new Account (200.00m);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
                );

            Request = new MakeDepositRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = 1,
                Amount = new Money(100.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            // CustomerRepository.GetByIdAsync(Request.CustomerId).Returns(Task.FromResult<Customer?>(null));
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            // Result = await MakeDepositCommandHandler.Handle(Request, CancellationToken.None);
            // Result.Succeeded.Should().Be(false);

            // or?
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));

        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));
            
            // for this sort of test, try to get away from specific wording on error messages if you can, as these can be changed later on by developers
                // if a dev changes the type of error that is thrown by this though, then it should be broken, and should be failed on this test, so changes to types of error message, etc. should be known and intentional

            // we currently are basing this sort of logic on exceptions, but it would make sense based on the business requirement of succeeded true, to also do:
                // succeeded false, with a message in that response object that's returned on failure, instead of based on exceptions
                // we may speak with the client to decide on this, as they didn't specifically ask for it, but it would make sense
                // in most cases, from all controller methods we would want to return an envelope that has a set structure, response object, collection of error messages, date/time/etc info
                    // create this envelope perhaps in the controller base class
                    // ok returns envelope with success response, error returns either response data with failure, or if more appropriate, returns no response data

                        // would generally do this unless their current setup requires otherwise, but we try to stick to doing it the best way, and refactor the existing code around it, if not too extensive
    


            // QA - how to do this with fluent assertions?

            // Result = await MakeDepositCommandHandler.Handle(Request);
            // Result.Should().BeOfType<KeyNotFoundException>();

        }
    }

    public class When_making_a_deposit_and_the_account_does_not_belong_to_the_customer
    {
        public static MakeDepositCommandHandler MakeDepositCommandHandler { get; set; }
        public static MakeDepositResult Result { get; set; }
        public static MakeDepositRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_deposit_and_the_account_does_not_belong_to_the_customer()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account (new Money (200.00m), AccountStatus.Open);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                []
                );

            Request = new MakeDepositRequest
            {
                CustomerId = testCustomer.Id,
                AccountId = testAccount.Id, // this account exists but does not belong to the customer
                Amount = new Money(100.00m)
            };
            

            CustomerRepository = Substitute.For<IRepository<Customer>>(); // NSubstitute package for mock data
            // CustomerRepository.GetByIdAsync(Request.CustomerId).Returns(Task.FromResult<Customer?>(null));
            CustomerRepository.GetByIdAsync(Arg.Any<GetSingleCustomerWithAccounts>()).Returns(Task.FromResult(testCustomer));

            MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));

        }

        [Fact]
        public async Task The_caller_should_be_told_the_account_is_not_available()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));
            // should be told on this - don't want to check a the specific message, but we could check that the exception is a KeyNotFoundException and that it contains the accountId in the message
            // or should we add a paramname to the exception and check that (less specific than the message)
        }
    }

    public class When_making_a_deposit_and_the_amount_is_less_than_or_equal_to_zero
    {
        public static MakeDepositCommandHandler MakeDepositCommandHandler { get; set; }
        public static MakeDepositResult Result { get; set; }
        public static MakeDepositRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_deposit_and_the_amount_is_less_than_or_equal_to_zero()
        {
            // this is the arrange/setup/initialization for the test class, so that we don't have to repeat it in each test, and if we need to change it later we only have to change it in one place
            
            var testAccount = new Account (new Money (200.00m), AccountStatus.Open);
            var testCustomer = new Customer(
                new PersonName("Test", "Customer"), 
                [testAccount]
                );

            Request = new MakeDepositRequest
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

            MakeDepositCommandHandler = new MakeDepositCommandHandler(CustomerRepository);
        }

        [Fact]
        public async Task The_caller_should_be_notified_of_failure()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));
        }

        [Fact]
        public async Task The_caller_should_be_told_the_amount_must_be_above_zero()
        {
            // await Assert.ThrowsAsync<ArgumentException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));
          
            // this and similar should_be_told tests can be done in a specific way to check the parameter name to make sure the error was for the right reason, but not too specific to the point that the specific message matters
            // should we do this at all or is this even too specific and we should jsut remove these should be told tests? 

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => MakeDepositCommandHandler.Handle(Request, CancellationToken.None));
            exception.ParamName.Should().Be("amount");
            // this does what we need for this test, but if we wanted to be more specific and check that the parameter name is also correct, we could do something like this instead:
            // exception.ParamName.Should().Be("amount");
            // this is specific, but it also ensures that the exception is being thrown for the correct reason

            // note this will break if the parameter name is not set
                // when choosing the scenario for test assertion, make sure the failure scenario we get is the one we want
                // test something that should not be changed when refactoring the related code
        }
    }

    
}