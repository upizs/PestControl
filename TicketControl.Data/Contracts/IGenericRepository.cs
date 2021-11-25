using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Contracts
{
    public interface IGenericRepository <T> where T : class
    {
        /// <summary>
        /// Get all or get all with filter
        /// </summary>
        /// <param name="expression"> Lambda bool expression</param>
        /// <returns>Collection of Entities. Empty Collection if no result were found</returns>
        Task<ICollection<T>> GetAll(Expression<Func<T, bool>> expression = null);
        /// <summary>
        /// Gets one entry by search criteria
        /// </summary>
        /// <param name="expression">Lambda bool expression</param>
        /// <exception cref="ArgumentNullException">If Argument is null</exception>
        /// <returns>One Entity or Null if result not found</returns>
        Task<T> GetOne(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Insert one Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <exception cref="ArgumentNullException">If Argument is null</exception>
        /// <returns>True if sucessful and false if not</returns>
        Task<bool> Insert(T entity);
        /// <summary>
        /// Update one Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <exception cref="ArgumentNullException">If Argument is null</exception>
        /// <returns>True if sucessful and false if not</returns>
        Task<bool> Update(T entity);
        /// <summary>
        /// Delete one Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <exception cref="ArgumentNullException">If Argument is null</exception>
        /// <returns>True if sucessful and false if not</returns>
        Task<bool> Delete(T entity);
    }
}
