using System;

public interface INotifyNodeColorAdded
{
    event EventHandler<NodeColorAddEventArgs> ColorAdded;
}