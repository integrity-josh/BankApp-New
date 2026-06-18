using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using BankApp.Domain.Entities;

namespace BankApp.Domain.Specifications
{
    public class GetSingleCustomerWithAccounts : SingleResultSpecification<Customer>
    {
        public GetSingleCustomerWithAccounts(int customerId)
        {
            Query.Where(c => c.Id == customerId) // filter to get the customer with the specified id
                .Include(c => c.Accounts); // include the accounts when we get the customer, so that we have access to them when we get the customer, and we don't have to make a separate query to get the accounts for the customer later, which would be less efficient and more complex to implement, so this is a good example of how specifications can help us to keep our queries efficient and our code clean by allowing us to specify exactly what we want to include in our queries in a reusable way
        }

        // we could also mess with dto's in here - we could have a CustomerWithAccountsDto that has the customer properties and a list of accounts, then we could map the customer and accounts to that dto in this specification, so that we can return that dto from our repository instead of the customer entity, which would be a good way to decouple our domain entities from our application layer, and it would also allow us to shape the data that we return from our repository in a way that is more convenient for our application layer, so this is another example of how specifications can help us to keep our code clean and maintainable by allowing us to shape our queries and the data that we return from our repositories in a reusable way

        // we could also create a specification for the amount being greater than 0 and run that as a validation method
        
    }
}