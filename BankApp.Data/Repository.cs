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
        public async Task<T?> GetByIdAsync(int id)
        {
            // this will not load Accounts when we get a customer, we will need to use a query for that or similar
            
            return await dbContext.Set<T>().FindAsync(id);
        }

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