using UnityEngine;

[ScriptExecutionOrder(0)]
[RequireComponent(typeof(PipeScript))]
public class PipeAnimatorScript : MonoBehaviour
{
    private PipeAnimator pipeAnimator;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init()
    {
        pipeAnimator = new PipeAnimator(
                    GetComponent<PipeScript>().GetPipe(),
                    GetComponentInChildren<PipeShaderScript>(),
                    new CoroutineStarter(this));
    }

    public PipeAnimator GetPipeAnimator()
    {
        return pipeAnimator;
    }
}
