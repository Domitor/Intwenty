using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Data
{
    public sealed class SingeltonCache
    {
        private static SingeltonCache instance = null;
        private static readonly object padlock = new object();

        SingeltonCache()
        {
        }

        public static SingeltonCache Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingeltonCache();
                        instance.CachedObjects = new ConcurrentDictionary<string, object>();
                    }
                    return instance;
                }
            }
        }

        private ConcurrentDictionary<string, object> CachedObjects { get; set; }

        public bool TryGetValue<T>(string key, out T value)
        {
            object cv = null;
            if (CachedObjects.TryGetValue(key, out cv))
            {
                value = (T)cv;
                return true;
            }

            value = default(T);
            return false;
        }

        public void Set<T>(string key, T value)
        {
            Remove(key);
            var res = CachedObjects.TryAdd(key, value);
            var x = "";
        }

        public void Remove(string key)
        {
            object cv = null;
            if (CachedObjects.ContainsKey(key))
                CachedObjects.Remove(key, out cv);


        }


    }
}
