using System;
using System.IO;

namespace Myth.Net
{
    public class DataBuffer
    {
        private MemoryStream ms = new MemoryStream();

        private BinaryWriter bwriter;
        private BinaryReader breader;
        internal void Close()
        {
            bwriter.Close();
            breader.Close();
            ms.Close();
        }
        /// <summary>
        /// 默认构造
        /// </summary>
        internal DataBuffer()
        {
            bwriter = new BinaryWriter(ms);
            breader = new BinaryReader(ms);
        }
        /// <summary>
        /// 支持传入初始数据的构造
        /// </summary>
        /// <param name="buff"></param>
        internal DataBuffer(byte[] buff)
        {
            ms = new MemoryStream(buff);
            bwriter = new BinaryWriter(ms);
            breader = new BinaryReader(ms);
        }

        /// <summary>
        /// 获取当前数据 读取到的下标位置
        /// </summary>
        internal int Position
        {
            get { return (int)ms.Position; }
        }

        /// <summary>
        /// 获取当前数据长度
        /// </summary>
        internal int Length
        {
            get { return (int)ms.Length; }
        }
        /// <summary>
        /// 当前是否还有数据可以读取
        /// </summary>
        internal bool Readable
        {
            get { return ms.Length > ms.Position; }
        }

        #region write
        internal void Write(int val)
        {
            bwriter.Write(val);
        }
        internal void Write(byte val)
        {
            bwriter.Write(val);
        }
        internal void Write(bool val)
        {
            bwriter.Write(val);
        }
        internal void Write(string val)
        {
            bwriter.Write(val);
        }
        internal void Write(byte[] val)
        {
            bwriter.Write(val);
        }

        internal void Write(double val)
        {
            bwriter.Write(val);
        }
        internal void Write(float val)
        {
            bwriter.Write(val);
        }
        internal void Write(long val)
        {
            bwriter.Write(val);
        }
        #endregion


        #region read
        public void ReadInt32(out int val)
        {
            val = breader.ReadInt32();
        }
        public void ReadByte(out byte val)
        {
            val = breader.ReadByte();
        }
        public void ReadBoolean(out bool val)
        {
            val = breader.ReadBoolean();
        }
        public void ReadString(out string val)
        {
            val = breader.ReadString();
        }
        public void ReadBytes(out byte[] val, int length)
        {
            val = breader.ReadBytes(length);
        }

        public void ReadDouble(out double val)
        {
            val = breader.ReadDouble();
        }
        public void ReadSingle(out float val)
        {
            val = breader.ReadSingle();
        }
        public void ReadInt64(out long val)
        {
            val = breader.ReadInt64();
        }

        public void Reset()
        {
            ms.Position = 0;
        }

        #endregion
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            return result;
        }
    }
}
