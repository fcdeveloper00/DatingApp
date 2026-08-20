using API.Interfaces;

namespace API.Data;

public class UnitOfWork<T>(T repository, DataContext context) : IUnitOfWork<T>
{
    public T Repository => repository;

    public async Task<bool> CompleteAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
