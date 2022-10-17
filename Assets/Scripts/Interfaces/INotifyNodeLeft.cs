using System;

public interface INotifyNodeLeft
{
    event EventHandler<NodeLeaveEventArgs> NodeLeft;
}