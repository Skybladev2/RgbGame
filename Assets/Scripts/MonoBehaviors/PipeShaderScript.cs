using UnityEngine;

public class PipeShaderScript : MonoBehaviour, IArrowShaderScript
{
    [SerializeField]
    private float arrowsAnimationSpeed = 1f;
    [SerializeField]
    private float flowDuration = 1f;

    private float animationOffset = 0;
    private float flowOffset = 0;

    private Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        AnimateArrows();
        AnimateColorFlow();
    }

    private void AnimateColorFlow()
    {
        flowOffset += Time.deltaTime / flowDuration;
        flowOffset = Mathf.Clamp01(flowOffset);
        material.SetFloat("_Offset", flowOffset);
    }

    private void AnimateArrows()
    {
        animationOffset += Time.deltaTime * arrowsAnimationSpeed;
        animationOffset = Mathf.Repeat(animationOffset, 11);
        material.SetTextureOffset("_MainTex", new Vector2(-animationOffset, 0));
    }

    public void FlowColor(Color color)
    {
        material.SetColor("_FillColor", color);
        flowOffset = 0;
    }

    public void SetCurrentColor(Color color)
    {
        material.SetColor("_CurrentColor", color);
    }

    public float GetFlowDuration()
    {
        return flowDuration;
    }
}
