namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IDatabase<T>
    {
        Task InsertAsync(T Data);

        Task UpdateAsync(T Data);

        Task DeleteAsync(T Data);

        Task<T> GetById(int Id);

        List<T> GetAll();
    }
}