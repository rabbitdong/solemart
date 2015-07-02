using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Solemart.WebUtil
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class CacheUtil<T>
    {
        /// <summary>
        /// AddCache 添加Cache
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="objValue"></param>
        /// <param name="tsNum">tsNum 秒</param>
        public static void Add(string CacheKey, T CacheValue, double Seconds)
        {
            System.Web.HttpRuntime.Cache.Add(CacheKey, CacheValue, null, DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        public static void Insert(string CacheKey, T CacheValue, double Seconds)
        {
            System.Web.HttpRuntime.Cache.Insert(CacheKey, CacheValue, null, DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        public static void Remove(string CacheKey)
        {
            System.Web.HttpRuntime.Cache.Remove(CacheKey);
        }

        public static bool Exist(string key)
        {
            return System.Web.HttpRuntime.Cache[key] != null;
        }

        public static T Get(string key)
        {
            return (T)System.Web.HttpRuntime.Cache[key];
        }

        public static void Clear()
        {
            List<string> keys = new List<string>();
            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = System.Web.HttpRuntime.Cache.GetEnumerator();
            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                System.Web.HttpRuntime.Cache.Remove(keys[i]);
            }
        }
    }

    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class CacheUtil
    {
        /// <summary>
        /// AddCache 添加Cache
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="objValue"></param>
        /// <param name="tsNum">tsNum 秒</param>
        public static void Add<T>(string CacheKey, T CacheValue, double Seconds)
        {
            System.Web.HttpRuntime.Cache.Add(CacheKey, CacheValue, null, DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        public static void Insert<T>(string CacheKey, T CacheValue, double Seconds)
        {
            System.Web.HttpRuntime.Cache.Insert(CacheKey, CacheValue, null, DateTime.Now.AddSeconds(Seconds), TimeSpan.Zero, CacheItemPriority.Default, null);
        }

        public static void Remove(string CacheKey)
        {
            System.Web.HttpRuntime.Cache.Remove(CacheKey);
        }

        public static bool Exist(string key)
        {
            return System.Web.HttpRuntime.Cache[key] != null;
        }

        public static T Get<T>(string key)
        {
            return (T)System.Web.HttpRuntime.Cache[key];
        }

        public static void Clear()
        {
            List<string> keys = new List<string>();
            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = System.Web.HttpRuntime.Cache.GetEnumerator();
            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                System.Web.HttpRuntime.Cache.Remove(keys[i]);
            }
        }
    }
}
