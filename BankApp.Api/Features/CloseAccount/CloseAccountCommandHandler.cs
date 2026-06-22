using BankApp.Domain.Entities;
using BankApp.Domain.Repositories;
using BankApp.Domain.Specifications;
using MediatR;

namespace BankApp.Api.Features.CloseAccount
{
    public class CloseAccountCommandHandler : IRequestHandler<CloseAccountRequest, CloseAccountResult>
    {
        private readonly IRepository<Customer> _customerRepository;

        public CloseAccountCommandHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CloseAccountResult> Handle(CloseAccountRequest command, CancellationToken cancellationToken)
        {
            var customerQuery = new GetSingleCustomerWithAccounts(command.CustomerId);
            var customer = await _customerRepository.GetByIdAsync(customerQuery);

            if (customer is null)
            {
                throw new KeyNotFoundException($"Customer with Id {command.CustomerId} not found.");
            }

            customer.CloseAccount(command.AccountId);

            await _customerRepository.SaveChangesAsync();

            return new CloseAccountResult
            {
                CustomerId = customer.Id,
                AccountId = command.AccountId,
                Succeeded = true
            };
        }
    }
}
