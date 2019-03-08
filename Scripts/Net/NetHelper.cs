using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Myth.Net
{
    public class NetHelper
    {
        /// <summary>
        /// 获取已经占用的端口
        /// </summary>
        /// <returns></returns>
        private static List<int> GetUsedPorts()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            List<int> allPort = new List<int>();

            foreach (IPEndPoint ep in ipsTCP)
            {
                allPort.Add(ep.Port);
            }

            foreach (IPEndPoint ep in ipsUDP)
            {
                allPort.Add(ep.Port);
            }

            foreach (TcpConnectionInformation conn in tcpConnInfoArray)
            {
                allPort.Add(conn.LocalEndPoint.Port);
            }

            return allPort;

        }
        /// <summary>
        /// 获取可用端口
        /// </summary>
        /// <returns></returns>
        public static int GetUsablePort()
        {
            var list = GetUsedPorts();

            Random ran = new Random((int)DateTime.Now.Ticks);

            bool ranloop = true, result = true;
            int port = 10000;
            while (ranloop)
            {
                port = ran.Next(10000, 30000);
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] == port)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                if (!result)
                {
                    ranloop = false;
                }
            }
            return port;
        }

        /// <summary>
        /// 获取本机ip
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            string hostname = Dns.GetHostName();//得到本机名
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            IPAddress[] localaddrs = localhost.AddressList;
            for (int i = 0; i < localaddrs.Length; i++)
            {
                string tempStr = localaddrs[i].ToString();
                if (Regex.IsMatch(tempStr, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$"))
                {
                    return tempStr;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            NetworkInterface[] nn = NetworkInterface.GetAllNetworkInterfaces();
            for(int i=0;i<nn.Length;i++)
            {
                string str = nn[i].GetPhysicalAddress().ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            return string.Empty;
        }
    }
}
