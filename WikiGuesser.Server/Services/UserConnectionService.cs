    using System.Collections.Concurrent;
    using System.Security.Claims;
    
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
    
        public string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User is null");
            }
        
            // Check if user.Identity is authenticated
            if (!user.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated");
                return null;
            }
        
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
            
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                // Log available claims for debugging
                var availableClaims = string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}"));
                Console.WriteLine($"Available claims: {availableClaims}");
                Console.WriteLine($"Could not find claim: {claimType}");
                return null;
            }
        
            return claim.Value;
        }
    }