using UnityEngine;

[ScriptExecutionOrder(0)]
public class TargetBackgroundScript : MonoBehaviour
{
    private INode parentNode;
    private Animator animator;
    private ColorType targetColorType;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init(INotifyNodeLeft heroController)
    {
        animator = GetComponent<Animator>();
        parentNode = GetParentNode();
        parentNode.TargetColorSet += Node_TargetColorSet;

        heroController.NodeLeft += HeroController_NodeLeft;

        targetColorType = transform.parent.GetComponent<TargetColorScript>().TargetColorType;
        SetColor();
    }

    private void SetColor()
    {
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.color = ColorUtility.GetColor(targetColorType);
        }
    }

    private INode GetParentNode()
    {
        return transform.parent.parent.GetComponent<NodeScript>().GetNode();
    }

    private void HeroController_NodeLeft(object sender, NodeLeaveEventArgs e)
    {
        if (e.Node == parentNode
            &&
            parentNode.GetCurrentColorType() != targetColorType)
        {
            animator.SetTrigger("isLeft");
        }
    }

    private void Node_TargetColorSet(object sender, System.EventArgs e)
    {
        if ((NodeScript)sender == parentNode)
        {
            animator.SetTrigger("isTouched");
        }
    }
}