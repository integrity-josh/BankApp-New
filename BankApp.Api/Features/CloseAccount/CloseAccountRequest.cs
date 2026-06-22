using MediatR;

namespace BankApp.Api.Features.CloseAccount
{
    public class CloseAccountRequest : IRequest<CloseAccountResult>
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
    }
}
