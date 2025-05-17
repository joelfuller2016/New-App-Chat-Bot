using System;
using System.Collections.Generic;

namespace ChatBot.Data.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}