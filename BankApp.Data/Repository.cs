using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Domain.Repositories;
using BankApp.Domain.Infrastructure;


namespace BankApp.Data
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        public Task<T>? GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}