using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System;

namespace NewCAKV
{
    public class RedisHandler
    {
        public static void SetInCache(string key, string value)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDbConnection = redisConnection.GetDatabase();
            redisDbConnection.StringSet(key, value);
            Console.WriteLine("Inserted:");
        }
        public static RedisValue GetFromCache(string key)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDbConnection = redisConnection.GetDatabase();
            var keyValue = redisDbConnection.StringGet(key);

            return keyValue;
        }

        public static bool KeyExists(string key)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDbConnection = redisConnection.GetDatabase();
            return redisDbConnection.KeyExists(key);
        }
        public static bool KeyDelete(string key)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDbConnection = redisConnection.GetDatabase();
            if(redisDbConnection.KeyExists(key))
            {
               return redisDbConnection.KeyDelete(key);
            }
            else
            {
                Console.WriteLine("Key Does not Exist");
                return false;
            }
        }

        public static void HashInsert(HashEntry[] MasterObj, string HashKey)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDbConnection = redisConnection.GetDatabase();

            redisDbConnection.HashSet(HashKey, MasterObj);
            var allHash = redisDbConnection.HashGetAll(HashKey);
            foreach (var item in allHash)
            {
                Console.WriteLine(string.Format("key : {0}, value : {1}", item.Name, item.Value));
            }
        }

}





    
}


