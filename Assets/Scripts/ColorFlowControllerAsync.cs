using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ColorFlowControllerAsync : INotifyNodesColorChanged, INotifyColorFlowCompletedEvent
{
    private uint generation = 0;

    private IEnumerable<IPipe> affectedPipes;

    private IEnumerable<IPipe> flowOriginalGenerationPipes;
    private IEnumerable<IPipe> flowGenerationPipes;

    private IEnumerable<IPipe> dryUpGenerationPipes;
    private IEnumerable<IPipe> dryUpOriginalGenerationPipes;

    private IDictionary<INode, ColorType> nextNodesColor;
    private IHeroController heroController;
    private IEnumerable<IPipe> pipes;
    private IEnumerable<INode> pipedNodes;

    public event EventHandler<NodesColorChangedEventArgs> NodesColorChanged;
    public event EventHandler ColorFlowCompleted;

    public ColorFlowControllerAsync(
        INotifyNodesColorForced nodesColorForcer,
        INotifyNodeLeft nodeLeaver,
        IEnumerable<INotifyNodeColorAdded> pipeAnimators,
        IHeroController heroController,
        IEnumerable<IPipe> pipes)
    {
        nodesColorForcer.NodesColorForced += ColorForcer_NodesColorForced;
        nodeLeaver.NodeLeft += NodeLeaver_NodeLeft;
        NodesColorChanged += ColorFlowControllerAsync_NodesColorChanged;

        foreach (var animator in pipeAnimators)
        {
            animator.ColorAdded += Node_ColorAdded;
        }

        pipedNodes = GetPipedNodes(pipes);

        this.heroController = heroController;
        this.pipes = pipes;
    }

    private void NodeLeaver_NodeLeft(object sender, NodeLeaveEventArgs e)
    {
        DryUpPipes(e.Node);
    }

    private void DryUpPipes(INode startNode)
    {
        //throw new NotImplementedException();
    }

    private IEnumerable<INode> GetPipedNodes(IEnumerable<IPipe> pipes)
    {
        return pipes
            .Select(p => p.GetStartNode())
            .Union(pipes.Select(p => p.GetEndNode()))
            .Distinct();
    }

    void ColorFlowControllerAsync_NodesColorChanged(object sender, NodesColorChangedEventArgs e)
    {
        StartNextGenerationColorFlow(e.Nodes);
    }

    private void Node_ColorAdded(object sender, NodeColorAddEventArgs e)
    {
        IEnumerable<IPipe> eventPipe = flowGenerationPipes.Where(p => p == e.Pipe).ToList();

        flowGenerationPipes = flowGenerationPipes.Except(eventPipe).ToList();

        if (!flowGenerationPipes.Any())
        {
            CompleteGenerationFlow();
        }
    }

    private void CompleteGenerationFlow()
    {
        generation++;

        if (AreNodesColorChanged())
        {
            IEnumerable<INode> changedNodes = GetChangedNodes().ToList();
            GenerateNodesColorChangedEvent(changedNodes);
        }
        else
        {
            EndColorFlow();
        }
    }

    private void EndColorFlow()
    {
        //PrintPipedNodeColors();
        //PrintAffectedPipes();
        affectedPipes = null;
        UnFreezeHero();
        OnColorFlowCompleted();
    }

    private void PrintAffectedPipes()
    {
        foreach (var pipe in affectedPipes
            .OrderByDescending(p => p.GetStartNode().GetCoords().Row)
            .ThenBy(p => p.GetStartNode().GetCoords().Column))
        {
            Debug.Log("Affected pipe: " + pipe.ToString());
        }
    }

    private void PrintPipedNodeColors(IEnumerable<IPipe> pipes)
    {
        var pipedNodes = GetPipedNodes(pipes)
            .OrderByDescending(n => n.GetCoords().Row)
            .ThenBy(n => n.GetCoords().Column);

        foreach (var node in pipedNodes)
        {
            Debug.Log(node.ToString() + ": " + node.GetCurrentColorType());
        }
    }

    private void OnColorFlowCompleted()
    {
        EventHandler handler = ColorFlowCompleted;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    private void GenerateNodesColorChangedEvent(IEnumerable<INode> changedNodes)
    {
        EventHandler<NodesColorChangedEventArgs> handler = NodesColorChanged;
        if (handler != null)
        {
            handler(this, new NodesColorChangedEventArgs() { Nodes = changedNodes });
        }
    }

    private void UnFreezeHero()
    {
        heroController.UnFreeze();
    }

    private void FreezeHero()
    {
        heroController.Freeze();
    }

    private IEnumerable<INode> GetChangedNodes()
    {
        ICollection<INode> changedNodes = new List<INode>();

        foreach (var node in GetGenerationEndNodes())
        {
            if (IsNodeColorChanged(node))
            {
                changedNodes.Add(node);
            }
        }

        return changedNodes;
    }

    private bool IsNodeColorChanged(INode node)
    {
        return node.GetCurrentColorType() != nextNodesColor[node];
    }

    private bool AreNodesColorChanged()
    {
        return GetGenerationEndNodes().Any(IsNodeColorChanged);
    }

    private IEnumerable<INode> GetGenerationEndNodes()
    {
        return flowOriginalGenerationPipes.Select(p => p.GetEndNode());
    }

    private void ColorForcer_NodesColorForced(object sender, NodesColorChangedEventArgs e)
    {
        FreezeHero();
        generation = 0;
        StartNextGenerationColorFlow(e.Nodes);
    }

    private void StartNextGenerationColorFlow(IEnumerable<INode> nodes)
    {
        flowGenerationPipes = GetGeneratoinPipes(nodes);

        if (!flowGenerationPipes.Any())
        {
            EndColorFlow();
            return;
        }

        SaveFlowGenerationPipes();
        SaveNextNodesColor();
        StartColorFlow();
    }

    private void StartColorFlow()
    {
        foreach (var pipe in flowGenerationPipes)
        {
            pipe.StartColorFlow();
        }
    }

    private void SaveFlowGenerationPipes()
    {
        flowOriginalGenerationPipes = flowGenerationPipes;
    }

    private void RefreshAffectedPipes()
    {
        if (affectedPipes == null)
            affectedPipes = flowGenerationPipes;
        else
            affectedPipes = affectedPipes.Union(flowGenerationPipes).Distinct();
    }

    private IEnumerable<IPipe> GetGeneratoinPipes(IEnumerable<INode> nodes)
    {
        return pipes.Where(p => nodes.Contains(p.GetStartNode())).ToList();
    }

    private void PrintGenerationPipes()
    {
        foreach (var pipe in flowGenerationPipes
            .OrderByDescending(p => p.GetStartNode().GetCoords().Row)
            .ThenBy(p => p.GetStartNode().GetCoords().Column))
        {
            Debug.Log(String.Format("Generation {0} pipe: {1}", generation, pipe.ToString()));
        }
    }

    private void SaveNextNodesColor()
    {
        nextNodesColor = GetNextNodesColor(flowGenerationPipes);
    }

    private IDictionary<INode, ColorType> GetNextNodesColor(IEnumerable<IPipe> pipes)
    {
        IDictionary<INode, ColorType> nodesColor = new Dictionary<INode, ColorType>();

        foreach (var pipe in pipes)
        {
            INode node = pipe.GetEndNode();
            nodesColor[node] = node.GetCurrentColorType();
        }

        return nodesColor;
    }
}