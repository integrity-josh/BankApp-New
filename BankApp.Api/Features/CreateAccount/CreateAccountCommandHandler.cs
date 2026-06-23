using BankApp.Domain.Entities;
using BankApp.Domain.Repositories;
using BankApp.Domain.Specifications;
using MediatR;

namespace BankApp.Api.Features.CreateAccount
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountRequest, CreateAccountResult>
    {
        private readonly IRepository<Customer> _customerRepository;

        public CreateAccountCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CreateAccountResult> Handle(CreateAccountRequest command, CancellationToken cancellationToken)
        {
            var customerQuery = new GetSingleCustomerWithAccounts(command.CustomerId);
            var customer = await _customerRepository.GetByIdAsync(customerQuery);

            if (customer is null)
            {
                throw new KeyNotFoundException($"Customer with Id {command.CustomerId} not found.");
            }

            var account = customer.CreateAccount(command.AccountType, command.InitialDeposit);

            await _customerRepository.SaveChangesAsync();

            return new CreateAccountResult
            {
                CustomerId = customer.Id,
                AccountId = account.Id,
                Balance = account.Balance,
                Succeeded = true
            };
        }
    }
}
