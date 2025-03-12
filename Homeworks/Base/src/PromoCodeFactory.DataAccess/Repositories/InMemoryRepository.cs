using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        protected IEnumerable<T> GetRepositoryWithoutId(Guid id) => Data.Where(x => !x.Id.Equals(id));        

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task SetAsync(IEnumerable<T> items)
        {
            Data = items;
            return Task.FromResult<object>(null);
        }

        public Task<T> AddAsync(T item)
        {
            if (item == null) 
                throw new ArgumentNullException(nameof(item));            
            
            return Task.Run(() => {
                if (Data.SingleOrDefault(x => x.Id.Equals(item.Id)) != null)
                    throw new ArgumentException($"Идентификатор '{item.Id}' должен быть уникальным!");

                Data = Data.Append(item);

                return item;
            });
        }

        public Task DeleteByIdAsync(Guid id)
        {
            return Task.Run(() => Data = GetRepositoryWithoutId(id));
        }

        public Task UpdateAsync(T item)
        {
            return Task.Run(() =>
            {                
                Data = GetRepositoryWithoutId(item.Id).Append(item);
                return;
            });
        }
    }
}