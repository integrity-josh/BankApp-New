using BankApp.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Api.Controllers
{
    public abstract class BankAppControllerBase : ControllerBase
    {
        protected async Task<IActionResult> Execute<TResponse>(Func<Task<TResponse>> action)
        {
            try
            {
                var result = await action();
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            // catch (DomainException ex)
            // {
            //     return BadRequest(ex.Message);
            // } // domain exception now a child of argument exception, so we don't need to catch it separately
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            // could also inject imediator to reduce code further than this base controller currently does
        }
    }
}