namespace GameStore.App.Data
{
    using Contracts;
    using System.Collections.Generic;

    public class DbContextRepository : IRepository
    {
        private readonly GameStoreDbContext context;

        public DbContextRepository()
        {
            this.context = new GameStoreDbContext();
        }

        public void Add<T>(T item) where T : class
        {
            context.Add<T>(item);
            context.SaveChanges();
        }

        public void Detele<T>(int id) where T : class
        {
            context.Remove<T>(context.Find<T>(id));
            context.SaveChanges();
        }

        public void Edit<T>(T item) where T : class
        {
            context.Update<T>(item);
            context.SaveChanges();
        }

        public T Retrieve<T>(int id) where T : class
        {
            return context.Find<T>(id);
        }

        public IEnumerable<T> Retrieve<T>() where T : class
        {
            return context.Set<T>();
        }
    }
}
