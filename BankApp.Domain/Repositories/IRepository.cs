using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.Repositories
{
    public interface IRepository<T> where T: AggregateRoot // where T : IAggregateRoot - means this repository can only be used for entities that are aggregate roots (customer)
    // instead of IAggregateRoot, we do a base class AggregateRoot, which is a type of Entity
    {
        Task<T?> GetByIdAsync(int id); // making this nullable, but could get around this with Null Object Pattern - create an instance of the class that represents that not existing (ex: customer with Id of -1), then in repo either return found customer or return the customer with -1, then in code instead of checking if returned customer is null, we instead check if the Id is -1 or <0
        Task<T?> GetByIdAsync(ISingleResultSpecification<T> specification); // this is the method we will use in our code, and it will call the above method to get the customer by id, but it will also include the accounts when it gets the customer, so that we have access to them when we get the customer, and we don't have to make a separate query to get the accounts for the customer later, which would be less efficient and more complex to implement, so this is a good example of how specifications can help us to keep our queries efficient and our code clean by allowing us to specify exactly what we want to include in our queries in a reusable way
        Task SaveChangesAsync();
        
    }
}