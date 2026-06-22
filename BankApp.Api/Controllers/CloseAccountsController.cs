using BankApp.Api.Features.CloseAccount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloseAccountsController(IMediator mediator) : BankAppControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CloseAccount(CloseAccountRequest request)
        {
            return await Execute(() => mediator.Send(request));
        }
    }
}
