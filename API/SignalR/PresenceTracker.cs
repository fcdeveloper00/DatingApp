using System.Security.Cryptography.X509Certificates;

namespace API.SignalR;

public class PresenceTracker
{
    static readonly Dictionary<string, List<string>> _onlineUsers = [];

    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool isOnline = false;
        lock (_onlineUsers)
        {
            if (_onlineUsers.ContainsKey(username))
            {
                //_onlineUsers.Add(username, [connectionId]);
                _onlineUsers[username].Add(connectionId);
            }
            else
            {
                _onlineUsers.Add(username, [connectionId]);
                isOnline = true;
            }
        }
        return Task.FromResult(isOnline);
    }

    public Task <bool>UserDisconnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (_onlineUsers)
        {
            if (!_onlineUsers.ContainsKey(username))
            {
                return Task.FromResult(isOffline);
            }
            _onlineUsers[username].Remove(connectionId);

            if (_onlineUsers[username].Count == 0)
            {
                _onlineUsers.Remove(username);
                isOffline = true;
            }
        }
        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (_onlineUsers)
        {
            onlineUsers = _onlineUsers.OrderBy(u => u.Key).Select(u => u.Key).ToArray();
            //onlineUsers = [.._onlineUsers.OrderBy(u => u.Key).Select(u => u.Key)];
        }
        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetUserConnectionIds(string username)
    {
        List<string> connectionIds;
        if (_onlineUsers.TryGetValue(username, out var fetchedConnectionIds))
        {
            lock (fetchedConnectionIds)
            {
                connectionIds = [.. fetchedConnectionIds];
            }
        }
        else
        {
            connectionIds = [];
        }
        return Task.FromResult(connectionIds);
    }

}
