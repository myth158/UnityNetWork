/*****************
author：myth
*****************/
using Myth.Net;
using System.Text;

public class NetManager : SingleInstance<NetManager>
{

    public void Init()
    {
        TCPClient.Instance.Start("192.168.1.1",10000);
        TCPClient.Instance.OnConnected += () =>
        {

        };
        TCPClient.Instance.OnDisConnected += () =>
        {

        };
        TCPClient.Instance.OnReceived += OnReceivedData;
    }
    protected override void OnDestroy()
    {
        TCPClient.Instance.Close();
        TCPClient.Instance.OnDisConnected = null;
        TCPClient.Instance.OnConnected = null;
        TCPClient.Instance.OnReceived = null;
        base.OnDestroy();
    }
    private void OnReceivedData(byte[] data)
    {
        string str = Encoding.UTF8.GetString(data);
        //数据解析
    }
    private void SendMsg(string str)
    {
        TCPClient.Instance.Send(Encoding.UTF8.GetBytes(str));
    }
}
