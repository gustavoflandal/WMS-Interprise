namespace WMS.Shared.Interfaces;

public interface IUserApplicationService
{
    Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class;
    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
}
