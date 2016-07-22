using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
