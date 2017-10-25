namespace GameStore.App.Data.Contracts
{
    using System.Collections.Generic;

    public interface IRepository
    {
        T Retrieve<T>(int id) where T : class;

        IEnumerable<T> Retrieve<T>() where T : class;

        void Add<T>(T item) where T : class;

        void Detele<T>(int id) where T : class;

        void Edit<T>(T item) where T : class;
    }
}
