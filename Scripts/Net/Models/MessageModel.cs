namespace Myth.Net.Models
{
    using Extend;
    public class MessageModel
    {
        /// <summary>
        /// 模块
        /// </summary>
        public byte ModuleType { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public int Cmd { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        public MessageModel(byte module)
        {
            this.ModuleType = module;
        }
        public MessageModel(byte module,int cmd)
        {
            this.ModuleType = module;
            this.Cmd = cmd;
        }
        public MessageModel(byte module, int cmd,string msg)
        {
            this.ModuleType = module;
            this.Cmd = cmd;
            this.Message = msg;
        }
        public T GetMessage<T>()
        {
            return this.Message.Deserialize<T>();
        }
    }
}
