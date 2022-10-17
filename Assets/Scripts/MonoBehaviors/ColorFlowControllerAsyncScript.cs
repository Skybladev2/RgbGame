using System;
using System.Collections.Generic;
using UnityEngine;

[ScriptExecutionOrder(0)]
public class ColorFlowControllerAsyncScript : MonoBehaviour, INotifyColorFlowCompletedEvent, INotifyNodesColorChanged
{
    private ColorFlowControllerAsync colorFlowControllerAsync = null;

    public event EventHandler<NodesColorChangedEventArgs> NodesColorChanged
    {
        add
        {
            colorFlowControllerAsync.NodesColorChanged += value;
        }

        remove
        {
            colorFlowControllerAsync.NodesColorChanged -= value;
        }
    }

    public event EventHandler ColorFlowCompleted
    {
        add
        {
            colorFlowControllerAsync.ColorFlowCompleted += value;
        }

        remove
        {
            colorFlowControllerAsync.ColorFlowCompleted -= value;
        }
    }

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init(INotifyNodesColorForced nodesColorForcer,
        INotifyNodeLeft nodeLeaver,
        IEnumerable<INotifyNodeColorAdded> pipeAnimators,
        IHeroController heroController,
        IEnumerable<IPipe> pipes)
    {
        colorFlowControllerAsync = new ColorFlowControllerAsync(nodesColorForcer, nodeLeaver, pipeAnimators, heroController, pipes);
    }
}