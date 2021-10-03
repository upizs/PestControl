using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<ICollection<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);

        //returns bool to let me know if the action was succesful
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T updatedEntity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> SaveAsync();

    }
}
