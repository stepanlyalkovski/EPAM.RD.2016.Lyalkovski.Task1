using System;

namespace NetworkServiceCommunication.Entities
{
    public enum MessageType
    {
        Add = 0,
        Delete = 1
    }

    [Serializable]
    public class ServiceMessage<TEntity>
    {
        public TEntity Entity { get; set; }
        public MessageType MessageType { get; set; }
    }
}
