using Myth.Net.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Myth.Net
{
    public class TCPServer : Singleton<TCPServer>
    {
        private Socket socket = null;
        private Thread threadWatch = null;
        private bool loop = true;

        private Dictionary<string, Socket> onlineTable = null;
        public Action<string> OnConnected;
        public Action<byte[]> OnReceived;
        public Action OnDisconnected;
        public TCPServer() { }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="localIp"></param>
        /// <param name="port"></param>
        public void Start(string localIp, int port)
        {
            IPAddress address = IPAddress.Parse(localIp);
            IPEndPoint endPoint = new IPEndPoint(address, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(20);

            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            onlineTable = new Dictionary<string, Socket>();
        }

        /// <summary>
        /// 监听链接
        /// </summary>
        private void WatchConnecting()
        {
            while (loop)
            {
                Socket newClient = socket.Accept();

                string clientIp = (newClient.RemoteEndPoint as IPEndPoint).Address.ToString();
                if (!onlineTable.ContainsKey(clientIp))
                {
                    onlineTable.Add(clientIp, newClient);
                }

                Thread thr = new Thread(ProcessMessage);
                thr.IsBackground = true;
                thr.Start(clientIp);
                if (OnConnected != null)
                {
                    OnConnected.Invoke(clientIp);
                }
            }
        }

        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="obj"></param>
        private void ProcessMessage(object obj)
        {
            string clientKey = obj.ToString();
            Socket sokClient = onlineTable[clientKey] as Socket;

            while (true)
            {
                byte[] arrMsgRec = new byte[2048];
                try
                {
                    sokClient.Receive(arrMsgRec);
                }
                catch (Exception)
                {
                    if (OnDisconnected != null)
                    {
                        OnDisconnected.Invoke();
                    }
                    if(onlineTable.ContainsKey(clientKey))
                    {
                        onlineTable.Remove(clientKey);
                    }
                    break;
                }
                if (OnReceived != null)
                {
                    OnReceived.Invoke(arrMsgRec);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            try
            {
                foreach (var vv in onlineTable)
                {
                    vv.Value.Send(data);
                }
            }
            catch(Exception)
            {
                Close();
            }
        }

        /// <summary>
        /// 关闭所有链接
        /// </summary>
        public void Close()
        {
            foreach (var v in onlineTable)
            {
                v.Value.Close();
                v.Value.Shutdown(SocketShutdown.Both);
            }
            if (socket != null)
            {
                socket.Close();
            }
            socket = null;
            onlineTable.Clear();
            onlineTable = null;
        }
    }
}
