/*****************
author：myth
*****************/

namespace Myth.Net
{
    public class BroadMessageModel
    {
        public string ServerIP { get; set; }
        public int TcpServerPort { get; set; }

        //1--connected  2--disconnected
        public byte Status { get; set; }
    }
    public enum BroadStatus : byte
    {
        Connected = 1,
        Disconnected = 2
    }
}

