using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NodeScript))]
[ScriptExecutionOrder(0)]
public class NodeColorizerScript : MonoBehaviour
{
    private INode node;
    private new SpriteRenderer renderer;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init(INotifyNodesColorForced heroController, INotifyNodesColorChanged colorFlowController)
    {
        heroController.NodesColorForced += HeroController_NodesColorForced;
        colorFlowController.NodesColorChanged += ColorFlowController_NodesColorChanged;
        node = GetComponent<NodeScript>().GetNode();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void ColorFlowController_NodesColorChanged(object sender, NodesColorChangedEventArgs e)
    {
        ColorizeNode(e);
    }

    private void HeroController_NodesColorForced(object sender, NodesColorChangedEventArgs e)
    {
        ColorizeNode(e);
    }

    private void ColorizeNode(NodesColorChangedEventArgs e)
    {
        if (!e.Nodes.Contains(node))
            return;

        renderer.color = ColorUtility.GetColor(node.GetCurrentColorType());
    }
}
