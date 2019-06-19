using Myth.Net.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Myth.Net
{
    public class TCPClient
    {
        public Action<byte[]> OnReceived;
        public Action OnConnected;
        public Action OnDisConnected;
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected = false;
        private Socket sockClient = null;
        private Thread threadClient = null;
        private Thread initThread = null;
        private System.Timers.Timer myTimer = new System.Timers.Timer(5000);
        private string ip = string.Empty;
        private int port = 0;
        private bool loop = true;

        public TCPClient()
        {
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeOut);
        }

        public void Start(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            initThread = new Thread(InitNet);
            initThread.Start();
        }

        private void InitNet()
        {
            IPAddress ip = IPAddress.Parse(this.ip);
            IPEndPoint endPoint = new IPEndPoint(ip, this.port);
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sockClient.Connect(endPoint);
            }
            catch (Exception ex)
            {
                myTimer.Start();
                IsConnected = false;
                if (OnDisConnected != null)
                {
                    OnDisConnected.Invoke();
                }
                return;
            }
            IsConnected = true;
            if (OnConnected != null)
            {
                OnConnected.Invoke();
            }
            threadClient = new Thread(ProcessReceive);
            threadClient.Start();
            myTimer.Stop();
        }

        private void TimeOut(object source, System.Timers.ElapsedEventArgs e)
        {
            loop = true;
            CloseThread();
            initThread = new Thread(InitNet);
            initThread.Start();
            //Dispatcher.Invoke(() =>
            //{
            //    LogHelper.WriteLog("TcpClient ", "re-connected");
            //});
            Console.WriteLine("re-connecting");
        }

        void ProcessReceive()
        {
            while (loop)
            {
                try
                {
                    byte[] arrMsgRec = new byte[2048];
                    int recNum = sockClient.Receive(arrMsgRec);
                    if (OnReceived != null && recNum > 0)
                    {
                        OnReceived.Invoke(arrMsgRec);
                    }
                }
                catch (Exception)
                {
                    CloseClient();
                    myTimer.Start();
                }
            }
        }


        public void Send(byte[] data)
        {
            if (sockClient != null && sockClient.Connected)
            {
                sockClient.Send(data);
            }
        }

        public void Close()
        {
            CloseThread();
            myTimer.Stop();
            myTimer.Close();
            CloseClient();
            sockClient = null;
        }

        private void CloseClient()
        {
            loop = false;
            if (sockClient != null)
            {
                sockClient.Shutdown(SocketShutdown.Both);
                sockClient.Close();
            }
            try
            {
                if (threadClient != null)
                {
                    threadClient.Abort();
                }
            }
            catch (ThreadAbortException ex)
            {
                
            }
            threadClient = null;
        }

        private void CloseThread()
        {
            if (initThread != null)
            {
                initThread.Abort();
            }
            initThread = null;
        }
    }
}
