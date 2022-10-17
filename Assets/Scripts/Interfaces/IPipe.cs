using System;

public interface IPipe {
    INode GetStartNode();
    INode GetEndNode();
    void SetStartNode(INode node);
    void SetEndNode(INode node);
    void StopColorFlow();
    void StartColorFlow();
    event EventHandler<ColorFlowStartEventArgs> ColorFlowStart;
}