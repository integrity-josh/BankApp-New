using BankApp.Domain.ValueObjects;
using MediatR;

namespace BankApp.Api.Features.CreateAccount
{
    public class CreateAccountRequest : IRequest<CreateAccountResult>
    {
        public int CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public Money InitialDeposit { get; set; }
    }
}
