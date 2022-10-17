using System;

public interface INotifyNodesColorForced
{
    event EventHandler<NodesColorChangedEventArgs> NodesColorForced;
}