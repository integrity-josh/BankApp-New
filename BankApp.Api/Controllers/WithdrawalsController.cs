using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Api.Features.MakeWithdrawal;
using BankApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class WithdrawalsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MakeWithdrawal(MakeWithdrawalRequest request)
        {
            
            try
            {
                var result = await mediator.Send(request); 
                return Ok(result);
                        
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(DomainException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            } 
        }
    }
}