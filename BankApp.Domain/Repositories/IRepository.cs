using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Infrastructure;

namespace BankApp.Domain.Repositories
{
    public interface IRepository<T> where T: AggregateRoot // where T : IAggregateRoot - means this repository can only be used for entities that are aggregate roots (customer)
    // instead of IAggregateRoot, we do a base class AggregateRoot, which is a type of Entity
    {
        Task<T?> GetByIdAsync(int id); // making this nullable, but could get around this with Null Object Pattern - create an instance of the class that represents that not existing (ex: customer with Id of -1), then in repo either return found customer or return the customer with -1, then in code instead of checking if returned customer is null, we instead check if the Id is -1 or <0
        
        Task SaveChangesAsync();
        
    }
}