using System;

interface INotifyNodeArrived
{
    event EventHandler<NodeArriveEventArgs> NodeArrived;
}
