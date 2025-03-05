using System.Collections.Concurrent;

namespace WikiGuesser.Server.Services;

public class UserConnectionService
{
    private readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
    
    public void AddConnection(string username, string connectionId)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _userConnections[username] = connectionId;
        }
    }

    public void RemoveConnection(string username)
    {
        if(!string.IsNullOrEmpty(username))
        {
            _userConnections.TryRemove(username, out _);
        }
    }
    
    
}