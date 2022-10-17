using UnityEngine;

[ScriptExecutionOrder(0)]
public class PipeScript : MonoBehaviour
{
    private IPipe pipe = new Pipe();
    private NodeScript startNodeScript;
    [SerializeField]
    private NodeScript endNodeScript;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init()
    {
        startNodeScript = GetComponentInParent<NodeScript>();
        SetPipeNodes();
    }

    private void SetPipeNodes()
    {
        pipe.SetStartNode(startNodeScript.GetNode());
        pipe.SetEndNode(endNodeScript.GetNode());
    }

    public void SetEndNodeScript(NodeScript nodeScript)
    {
        endNodeScript = nodeScript;
    }

    public NodeScript GetEndNodeScript()
    {
        return endNodeScript;
    }

    public IPipe GetPipe()
    {
        return pipe;
    }
}
