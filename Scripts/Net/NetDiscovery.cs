using Myth.Net.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Myth.Net
{
    public class NetDiscovery : SingleInstance<NetDiscovery>
    {
        private Socket udpClient = null;
        private IPEndPoint ipEnd;
        private EndPoint recIpEnd;
        private int port = 9999;
        private System.Timers.Timer broadTimer = null;
        private Thread receiveThread = null;
        private bool loop = true;
        private int interval = 5000;
        private byte[] broadBuffer = null;
        public Action<byte[]> OnReceivedBroadcast;

        public string RemoteIP
        {
            get
            {
                return (udpClient.RemoteEndPoint as IPEndPoint).Address.ToString();
            }
        }
        public NetDiscovery()
        {
           
        }


        public void InitBroadcast()
        {
            udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            recIpEnd = new IPEndPoint(IPAddress.Any, port);
            udpClient.Bind(recIpEnd);
        }
        public void Start()
        {
            ipEnd = new IPEndPoint(IPAddress.Broadcast, port);
            broadTimer = new System.Timers.Timer(interval);
            broadTimer.Elapsed += new System.Timers.ElapsedEventHandler(BroadCast);
            broadTimer.AutoReset = true;
            broadTimer.Start();
            receiveThread = new Thread(ProcessReceive);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        public void StartReceive()
        {
            receiveThread = new Thread(ProcessReceive);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        private void BroadCast(object source, System.Timers.ElapsedEventArgs e)
        {
            if(udpClient!=null&& broadBuffer!=null)
            {
                udpClient.SendTo(broadBuffer, ipEnd);
            }
        }

        private void ProcessReceive()
        {
            byte[] recBuffer = null;
            while (loop)
            {
                try
                {
                    recBuffer = new byte[1024];
                    udpClient.ReceiveFrom(recBuffer,ref recIpEnd);
                }
                catch(Exception)
                {
                    Close();
                    return;
                }

                if(OnReceivedBroadcast!=null)
                {
                    OnReceivedBroadcast.Invoke(recBuffer);
                }
            }
        }
        public void SetBroadData(byte[] data)
        {
            this.broadBuffer = data;
        }


        public void Stop()
        {
            this.broadBuffer = null;
            recIpEnd = null;
            ipEnd = null;
            if (broadTimer!=null)
            {
                broadTimer.Elapsed -= new System.Timers.ElapsedEventHandler(BroadCast);
                broadTimer.Stop();
                broadTimer.Dispose();
            }
            loop = false;
            if(receiveThread!=null)
            {
                receiveThread.Abort();
            }
            receiveThread = null;
        }


        public void Close()
        {
            Stop();
            if (udpClient!=null)
            {
                udpClient.Close();
            }
            udpClient = null;
            broadTimer = null;
        }
    }
}
