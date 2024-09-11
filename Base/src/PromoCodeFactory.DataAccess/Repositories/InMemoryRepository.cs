using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

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

        public Task AddAsync(T entity)
        {
            var _data = Data.ToList();

            _data.Add(entity);

            Data = _data;

            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var _localData = Data.ToList();

            var _getObject = _localData.Where(x => x.Id == id).FirstOrDefault();

            if (_getObject == null) return Task.FromResult(false);            

            _localData.Remove(_getObject);

            Data = _localData;

            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(Guid id,T entity)
        {
            var _localData = Data.ToList();
            
            var _getObjectIndex = _localData.FindIndex(x => x.Id == id);

            if (_getObjectIndex == -1) return Task.FromResult(false);

            _localData[_getObjectIndex] = entity;

            Data = _localData;

            return Task.FromResult(true);
        }
    }
}