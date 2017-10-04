using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Markup.Cache
{
    public static class CacheManager
    {
        private static string cachePrefix = "episerver.markup.";
        private static TimeSpan defaultCacheTimeout = new TimeSpan(1, 0, 0);

        private static SortedSet<string> keys = new SortedSet<string>();
        private static object locker = new object();

        public static void Add(string key, object item)
        {
            key = GetFullKey(key);
            lock (locker)
            {
                keys.Add(key);
            }
            HttpContext.Current.Cache.Insert(key, item, null, System.Web.Caching.Cache.NoAbsoluteExpiration, defaultCacheTimeout);
        }

        public static void Remove(string key)
        {
            key = GetFullKey(key);
            if (Exists(key))
            {
                keys.Remove(key);
                HttpContext.Current.Cache.Remove(key);
            }
        }

        public static void RemoveByPrefix(string prefix)
        {
            prefix = GetFullKey(prefix);
            keys
                .Where(k => k.StartsWith(prefix))
                .ToList()
                .ForEach(k => Remove(k));
        }

        public static T Get<T>(string key)
        {
            key = GetFullKey(key);
            if (!Exists(key))
            {
                return default(T);
            }
            return (T)HttpContext.Current.Cache[key];
        }

        public static bool Exists(string key)
        {
            key = GetFullKey(key);
            return (HttpContext.Current.Cache[key] != null);
        }

        private static string GetFullKey(string key)
        {
            return string.Concat(cachePrefix, key);
        }
    }
}