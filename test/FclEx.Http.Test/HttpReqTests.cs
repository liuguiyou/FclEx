using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FclEx.Http.Core;
using Xunit;
using FclEx.Http;

namespace FclEx.Http.Test
{
    public class HttpReqTests
    {
        public static string[] Hosts { get; } =
        {
            "localhost",
            "www.baidu.com",
            "127.0.0.1",
            "220.181.112.244"
        };

        public static int?[] Ports { get; } = { null, 80, 8080, 1234, };

        public static string[] HostPorts { get; } = Hosts.SelectMany(m => Ports, (h, p) => (Host: h, Port: p))
            .Select(m => m.Port.IsDefault() || m.Port == 80 ? m.Host : $"{m.Host}:{m.Port}").ToArray();

        public static IEnumerable<object[]> HostPortsPair { get; } = HostPorts
            .SelectMany(m => HostPorts, (i, j) => new object[] { i, j })
            .ToArray();
        

        [Theory]
        [MemberData(nameof(HostPortsPair))]
        public void TestSetHost(string host, string newHost)
        {
            var req = HttpReq.Get("http://" + host);
            Assert.Equal(host, req.Host);

            req.Host(host);
            Assert.Equal(host, req.Host);

            req.Host(newHost);
            Assert.Equal(newHost, req.Host);
        }
    }
}
