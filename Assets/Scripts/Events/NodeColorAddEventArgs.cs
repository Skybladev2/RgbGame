using System;

public class NodeColorAddEventArgs : EventArgs {
    public IPipe Pipe { get; set; }
    public ColorType ColorType { get; set; }
}
