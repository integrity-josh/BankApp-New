using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Infrastructure;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BankApp.Data
{
    public class Repository<T>(BankAppDbContext dbContext) : IRepository<T> where T : AggregateRoot
    {
        // if argument is an Id, we will just get the entity by that Id, and if the argument is a specification, we will use that specification to get the entity, which allows us to have more complex queries that can include related entities and filter based on specific criteria
        public async Task<T?> GetByIdAsync(int id)
        {
            // this will not load Accounts when we get a customer, we will need to use a query for that or similar
            
            return await dbContext.Set<T>().FindAsync(id);
        }

        // using a specification means this will return the customer with the accounts included, so we can access the accounts when we get the customer, and we don't have to make a separate query to get the accounts for the customer later
        public async Task<T?> GetByIdAsync(ISingleResultSpecification<T> specification)
        {
            return await SpecificationEvaluator.Default.GetQuery(dbContext.Set<T>(), specification).FirstOrDefaultAsync();
            // could then chain multiple specifications on to this beyond what it already as, as GetQuery still returns an IQueryable
        }
       
        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}