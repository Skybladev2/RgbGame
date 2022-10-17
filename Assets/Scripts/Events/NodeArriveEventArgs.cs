using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NodeArriveEventArgs : EventArgs
{
    public INode Node { get; set; }
}