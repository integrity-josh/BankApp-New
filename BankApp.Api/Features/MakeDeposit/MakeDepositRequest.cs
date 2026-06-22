using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.ValueObjects;
using MediatR;

namespace BankApp.Api.Features.MakeDeposit
{
    public class MakeDepositRequest : IRequest<MakeDepositResult>
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; } // decimal here instead of money so error handling can be done in the API layer, and we can return a bad request if the amount is invalid, rather than having it throw an exception in the domain layer
        // DTO's don't really matter to modeling the domain, so the type here doesn't really matter - modeling/enforcing business types here is not something we need to worry about
            // think of these like a flattened set of data

            // however we tend to really on the Domain layer for business rule validation for things like this Money type, don't want to get too far into the layers without checking it - check closest to domain as possible for business logic
            // there is also the scenario where we would do data validation on requests to just make sure it's a valid request to begin with
            // also fine to do it in both
    }
}