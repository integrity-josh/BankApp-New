namespace BankApp.Api.Features.CloseAccount
{
    public class CloseAccountResult
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public bool Succeeded { get; set; }
    }
}
