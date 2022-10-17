using System;
using System.Collections.Generic;

public class NodesColorChangedEventArgs : EventArgs
{
    public IEnumerable<INode> Nodes { get; set; }
}