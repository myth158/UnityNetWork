using Myth.Net;
using Myth.Net.Models;

namespace Myth.Net
{
    public class MessageHelper
    {
        public static  byte[] Encoded(MessageModel model)
        {
            DataBuffer dbuffer = new DataBuffer();
            dbuffer.Write(model.ModuleType);
            dbuffer.Write(model.Cmd);
            dbuffer.Write(model.Message);
            return dbuffer.GetData();

        }
        public static MessageModel Decode(byte[] data)
        {
            DataBuffer dbuffer = new DataBuffer(data);
            byte mtype; int cmd; string msg;
            dbuffer.ReadByte(out  mtype);
            dbuffer.ReadInt32(out cmd);
            dbuffer.ReadString(out msg);
            MessageModel mm = new MessageModel(mtype, cmd, msg);
            return mm;
        }
    }
}
