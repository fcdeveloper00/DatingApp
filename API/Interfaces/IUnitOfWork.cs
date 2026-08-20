namespace API.Interfaces;

public interface IUnitOfWork<T> 
{
    public T Repository { get; }
    Task<bool> CompleteAsync();
    bool HasChanges();
}
