using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EFRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext dataContext;
        private readonly DbSet<T> entity;

        public EFRepository(DataContext dataContext) 
        {
            this.dataContext = dataContext;
            this.entity = this.dataContext.Set<T>();
        }
        public Task<IEnumerable<T>> GetAllAsync()
        {            
            return Task.FromResult(this.entity.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(this.entity.FirstOrDefault(x => x.Id == id));
        }
    }
}
