namespace BigDataIndexing.WebApi;
using StackExchange.Redis;
using System;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }

    public IDatabase GetDatabase() => _database;
    public ConnectionMultiplexer GetConnectionMultiplexer() => _redis;
}
