using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ChatContext _chatContext;

        public RepositoryBase(ChatContext chatContext)
        {
            _chatContext = chatContext;
        }

        public IQueryable<T> FindAll(bool trackChanges) =>
        !trackChanges ?
          _chatContext.Set<T>()
           .AsNoTracking() :
          _chatContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges) =>
        !trackChanges ?
        _chatContext.Set<T>()
        .Where(expression)
        .AsNoTracking() :
        _chatContext.Set<T>()
        .Where(expression);

        public void Create(T entity) => _chatContext.Set<T>().Add(entity);
        public void Update(T entity) => _chatContext.Set<T>().Update(entity);
        public void Delete(T entity) => _chatContext.Set<T>().Remove(entity);
    }
}
