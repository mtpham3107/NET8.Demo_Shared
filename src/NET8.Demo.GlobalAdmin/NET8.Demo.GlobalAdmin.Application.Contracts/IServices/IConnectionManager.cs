namespace NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

public interface IConnectionManager
{
    ValueTask AddConnectionAsync(string userId, string connectionId);

    ValueTask RemoveConnectionAsync(string userId, string connectionId);

    ValueTask<IEnumerable<string>> GetConnectionIdAsync(string userId);
}
