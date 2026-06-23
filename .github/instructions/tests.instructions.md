```
Description: "This file describes the testing conventions for the BankApp project."
applyTo: '**/BankApp.Tests/**'
```
## Testing Conventions

- Use xUnit, FluentAssertions, and NSubstitute for tests in the `BankApp.Tests` project.
- Tests should work with the public API of the domain and not reach into private fields or methods.
- Follow the existing `When_...` naming style for test classes.
- Prefer arrange/act/assert structure and keep tests focused on one behavior.
- Use tests to verify business rules and edge cases, not just happy paths, and use tests as a blueprint to guide the software design.
- Tests should never be deleted as they are the blueprint for the business rules and software design. If a test is not passing, the software should be fixed, not the test deleted.

## Good example of a test class

```csharp
public class When_making_a_withdrawal
    {
        public static MakeWithdrawalCommandHandler MakeWithdrawalCommandHandler { get; set; }
        public static MakeWithdrawalResult Result { get; set; }
        public static MakeWithdrawalRequest Request { get; set; }
        public static IRepository<Customer> CustomerRepository { get; set; }

        public When_making_a_withdrawal()
        {
            // arrange/setup/initialization
            
            var testAccount = new Account (new Money (200.00m), AccountStatus.Open);
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
```


## Bad example of a test class

```csharp
 public class When_creating_an_account
    {
        [Fact]
        public void The_account_should_be_created_for_the_customer()
        {
            var customerId = 5;
            var initialDeposit = 525.00m;
            var accountType = new { Id = 1, Name = "Savings" };

            Assert.True(customerId > 0);
            Assert.True(initialDeposit >= 100m);
            Assert.NotNull(accountType);
            Assert.True(false, "Scenario not implemented yet");
        }

        [Fact]
        public void The_caller_should_be_notified_of_success()
        {
            var succeeded = false;

            Assert.True(succeeded);
            Assert.True(false, "Scenario not implemented yet");
        }
    }
```