using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Api.Features.MakeDeposit;
using BankApp.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // takes the controller name and makes that part of the route
    public class DepositsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MakeDeposit(MakeDepositRequest request)
        {
            // return Ok();
            // going to put everything into this method, but could do other options as well

            try
            {
                var result = await mediator.Send(request); // this will send the request to the mediator, which will then send it to the appropriate handler based on the type of the request, and then return the result from the handler back to the controller, which will then return it as a response to the client
                return Ok(result);

                // how do we get our handler in here?
                    // could make an interface for our handler and plug it in here
                        // just like how we set up the makedepositcommandhandler in the test, we could do the same thing here, but instead of using a mock repository, we would use the actual repository that we have set up in our data layer
                    // could also add all the handlers in here, but that would be coupled and make a long controller
                    // MEDIATOR PATTERN TO DECOUPLE HANDLERS - controller sends request to the mediator, and then the mediator sends the request to the handler
                        // there is a great library for this called MediatR, so you don't have to do most of the work for this pattern yourself
                        // nuget package: MediatR
                        // add mediatr to pipleine in program.cs - per mediatr documentation on github
                        // set up handler to implement IRequestHandler<MakeDepositRequest, MakeDepositResult> 
                        
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(DomainException ex) // domain exceptions are exceptions that we throw in our domain layer when something goes wrong with the business logic, like an invalid deposit amount
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            } // CUSTOM CONTROLLER BASE CLASS FOR THIS if we keep this up, every controller is going to have a try catch, so let's try to refactor this into more global solutions, like a controller base class that handles these exceptions, and making specific controllers like DepositsControllers of the type of that custom base controller (remember not to try using the default base class name of ControllerBase for this)
                // making that base controller class also will make it much easier for designing UI's around the errors handling, because we can have a consistent error response format for all/many controllers


            // try catches
                // when try catches fire, it's a significant performance hit, so
                // we only want to use them to handle exceptional things that we expect are possible
                // we don't want to use them for things that we expect to happen in the normal flow of our application
                // throwing errors up the stack will happen without a try catch, so we only want to use try catches when we want to catch specific exceptions and return specific responses based on those exceptions, otherwise we can just let the exceptions bubble up and be handled by the global exception handler, which will return a 500 error for unhandled exceptions
                
                // essentially, only use them for exceptions that could happen from issues outside the application, like database being down, and giving logic for what to do about that
                    // if you don't have anything to do about those exceptional situations, then don't try catch, just let it break

                // we always want the try catch in the place where the response to the exception is handled
                    // so for example, if we want to restore a database, we do that try catch in the data layer

                    // in the controller, we want to catch exceptions that we expect could happen from the business logic, like a customer not being found, or an account not being found, or a deposit amount being invalid, etc... and return specific responses based on those exceptions
        }
    }
}