namespace la_mia_pizzeria.Database
{
    public interface IRepository<T>
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T originalEntity, T modifiedEntity);
        void Delete(T entity);

    }
}
