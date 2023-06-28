using Asoode.Shared.Abstraction.Helpers;
using StackExchange.Redis;

namespace Asoode.Shared.Core.Implementations;

internal class RedisHandler  
{                  
    static RedisHandler()
    {
        var host = EnvironmentHelper.Get("APP_CACHE_SERVER")!;
        var port = EnvironmentHelper.Get("APP_CACHE_PORT")!;
        var password = EnvironmentHelper.Get("APP_CACHE_PASS")!;
        var connectionString = $"{host}:{port},password={password}";
        
        LazyConnection = new Lazy<ConnectionMultiplexer>(() => 
            ConnectionMultiplexer.Connect(connectionString));  
    }  
      
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection;          
  
    public static ConnectionMultiplexer Connection => LazyConnection.Value;
} 