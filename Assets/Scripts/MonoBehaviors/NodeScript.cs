using UnityEngine;
using System;

[ScriptExecutionOrder(0)]
public class NodeScript : MonoBehaviour
{
    private INode node = new Node();
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private int column;
    [SerializeField]
    private int row;

    public event EventHandler TargetColorSet;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        node.SetCoords(column, row);
    }

    public void SetCoords(int column, int row)
    {
        this.column = column;
        this.row = row;
    }

    public ColorType GetInitialColorType()
    {
        return node.GetInitialColorType();
    }

    public void AddPipeColor(IPipe pipe, ColorType colorType)
    {
        node.AddPipeColor(pipe, colorType);
    }

    public INode GetNode()
    {
        return node;
    }
}
