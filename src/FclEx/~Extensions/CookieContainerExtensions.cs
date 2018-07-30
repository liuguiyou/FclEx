using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace FclEx
{
    public static class CookieContainerExtensions
    {
        public static List<Cookie> GetAllCookies(this CookieContainer cookieJar)
        {
            var list = new List<Cookie>(cookieJar.Count);

            var table = cookieJar.GetType().InvokeMember("m_domainTable",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance,
                null,
                cookieJar,
                null).CastTo<Hashtable>();

            var cookieLists = new List<SortedList>();
            lock (table.SyncRoot)
            {
                foreach (var pathList in table.Values)
                {
                    var cookieList = pathList.GetType().InvokeMember("m_list",
                        BindingFlags.NonPublic |
                        BindingFlags.GetField |
                        BindingFlags.Instance,
                        null,
                        pathList,
                        new object[] { }).CastTo<SortedList>();

                    cookieLists.Add(cookieList);
                }
            }

            foreach (var cookieList in cookieLists)
            {
                lock (cookieList.SyncRoot)
                {
                    foreach (CookieCollection cookieCollection in cookieList.Values)
                    {
                        list.AddRange(cookieCollection.Cast<Cookie>());
                    }
                }
            }

            return list;
        }
    }
}
