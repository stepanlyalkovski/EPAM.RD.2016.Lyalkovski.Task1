using System;

namespace NetworkServiceCommunication
{
    public interface IReceiveEventGenerator<TEventArgs>
    {
        event EventHandler<TEventArgs> Added;
        event EventHandler<TEventArgs> Deleted;
    }
}