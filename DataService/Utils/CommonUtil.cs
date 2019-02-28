using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DataService.Utils
{
    public static class CommonUtil
    {
        public static int RoundTo5(this int n)
        {
            int remainder = n % 5;
            if (remainder != 0)
            {
                n += 5 - remainder;
            }
            return n;
        }

        public static Dictionary<TKey, List<TValue>> ClassifyToDictionary<TKey, TValue>(
            this List<TValue> source,
            Func<TValue, TKey> keySelector)
        {
            return source.GroupBy(keySelector)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}