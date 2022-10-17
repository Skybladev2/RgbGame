using System;

public interface INotifyNodesColorChanged
{
    event EventHandler<NodesColorChangedEventArgs> NodesColorChanged;
}