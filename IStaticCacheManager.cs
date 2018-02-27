using System;
using System.Collections.Concurrent;

namespace RoboUI
{
    public interface IStaticCacheManager
    {
        TResult Get<TResult>(string key, Func<string, TResult> acquire);
    }

    public class StaticCacheManager : IStaticCacheManager
    {
        private readonly ConcurrentDictionary<string, object> caches = new ConcurrentDictionary<string, object>();

        public TResult Get<TResult>(string key, Func<string, TResult> acquire)
        {
            if (caches.ContainsKey(key))
            {
                return (TResult)caches[key];
            }

            var data = acquire(key);

            caches[key] = data;

            return data;
        }
    }
}
