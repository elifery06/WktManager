using Microsoft.EntityFrameworkCore;
using WktManager.Data;
using System.Collections.Generic;
using System.Linq;

namespace WktManager.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        //public List<T> GetAll() => _dbSet.ToList();
        public IQueryable<T> GetAll() => _dbSet.AsQueryable();


        public T GetById(int id) => _dbSet.Find(id);

        public void Add(T entity) => _dbSet.Add(entity);

        public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

       
    }
}
