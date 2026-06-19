using BankApp.Api.Features.MakeWithdrawal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class WithdrawalsController(IMediator mediator) : BankAppControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MakeWithdrawal(MakeWithdrawalRequest request)
        {
            return await Execute(() => mediator.Send(request));
        }
    }
}