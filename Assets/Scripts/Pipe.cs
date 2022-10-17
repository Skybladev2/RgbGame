using UnityEngine;
using System;

[Serializable]
public class Pipe : IPipe
{
    [SerializeField]
    private INode endNode;
    private INode startNode;

    public event EventHandler<ColorFlowStartEventArgs> ColorFlowStart;

    public void SetStartNode(INode node)
    {
        startNode = node;
    }

    public void SetEndNode(INode node)
    {
        endNode = node;
    }

    public INode GetStartNode()
    {
        return startNode;
    }

    public INode GetEndNode()
    {
        return endNode;
    }

    public void StopColorFlow()
    {
        throw new NotImplementedException();
    }

    public void StartColorFlow()
    {
        ColorType colorType = GetStartNode().GetCurrentColorType();

        EventHandler<ColorFlowStartEventArgs> handler = ColorFlowStart;
        if (handler != null)
        {
            handler(this, new ColorFlowStartEventArgs() { ColorType = colorType });
        }
    }

    public override string ToString()
    {
        return GetStartNode().ToString() + "->" + GetEndNode().ToString();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Pipe))
            return false;

        Pipe other = (Pipe)obj;

        return other.GetStartNode() == GetStartNode() && other.GetEndNode() == GetEndNode();
    }
}
