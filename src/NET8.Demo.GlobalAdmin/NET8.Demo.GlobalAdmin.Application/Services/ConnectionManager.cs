using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using System.Collections.Concurrent;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _connections = new();

    public ValueTask AddConnectionAsync(string userId, string connectionId)
    {
        var connections = _connections.GetOrAdd(userId, _ => new ConcurrentBag<string>());
        connections.Add(connectionId);
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveConnectionAsync(string userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            var updatedConnections = new ConcurrentBag<string>(connections.Where(id => id != connectionId));
            if (!updatedConnections.Any())
            {
                _connections.TryRemove(userId, out _);
            }
            else
            {
                _connections[userId] = updatedConnections;
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<IEnumerable<string>> GetConnectionIdAsync(string userId)
    {
        var connections = _connections.GetOrAdd(userId, _ => new ConcurrentBag<string>());
        return ValueTask.FromResult(connections.AsEnumerable());
    }
}
