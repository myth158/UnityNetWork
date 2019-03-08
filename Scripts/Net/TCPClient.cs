using Myth.Net.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Myth.Net
{
    public class TCPClient : Singleton<TCPClient>
    {
        public Action<byte[]> OnReceived;
        public Action OnConnected;
        public Action OnDisConnected;

        private Socket sockClient = null;
        private Thread threadClient = null;
        private Thread initThread = null;

        private string ip = string.Empty;
        private int port = 0;
        private bool loop = true;
        public TCPClient() { }
        public void Start(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            initThread = new Thread(InitNetThread);
            initThread.IsBackground = true;
            initThread.Start();
        }

        private void InitNetThread()
        {
            IPAddress ip = IPAddress.Parse(this.ip);
            IPEndPoint endPoint = new IPEndPoint(ip, this.port);
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sockClient.Connect(endPoint);
            }
            catch (SocketException)
            {
                System.Timers.Timer t = new System.Timers.Timer(5000);
                t.Elapsed += new System.Timers.ElapsedEventHandler(TimeOut);
                t.AutoReset = false;
                t.Enabled = true;
                if(OnDisConnected!=null)
                {
                    OnDisConnected.Invoke();
                }
                return;
            }

            if (OnConnected != null)
            {
                OnConnected.Invoke();
            }
            threadClient = new Thread(ProcessReceive);
            threadClient.IsBackground = true;
            threadClient.Start();
        }

        private void TimeOut(object source, System.Timers.ElapsedEventArgs e)
        {
            CloseThread();
            initThread = new Thread(InitNetThread);
            initThread.IsBackground = true;
            initThread.Start();
        }

        void ProcessReceive()
        {
            while (loop)
            {
                byte[] arrMsgRec = null;
                try
                {
                    arrMsgRec = new byte[2048];
                    sockClient.Receive(arrMsgRec);
                }
                catch (Exception)
                {
                    Close();
                    return;
                }
                if (OnReceived != null)
                {
                    OnReceived.Invoke(arrMsgRec);
                }
            }
        }
        public void Send(byte[] data)
        {
            if (sockClient != null&& sockClient.Connected)
            {
                sockClient.Send(data);
            }
        }
        public void Close()
        {
            CloseThread();

            loop = false;
            if (threadClient != null)
            {
                threadClient.IsBackground = false;
                threadClient.Interrupt();
                threadClient.Abort();
            }
            if (sockClient != null)
            {
                sockClient.Close();
            }
            initThread = null;
            sockClient = null;
            threadClient = null;
        }

        private void CloseThread()
        {
            if (initThread != null)
            {
                initThread.Interrupt();
                initThread.Abort();
            }
        }
    }
}
