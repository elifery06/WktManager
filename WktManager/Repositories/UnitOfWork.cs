using WktManager.Data;

namespace WktManager.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_context);//generic repo üretriyoree
        }

        public int Commit()
        {
            return _context.SaveChanges();//yapılan değşiiklikleri kaydediyor bu eleman
        }

        public void Dispose()
        {
            _context.Dispose();//contexi kapatır.
        }
    }
}
