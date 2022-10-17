using UnityEngine;

public interface IArrowShaderScript
{
    float GetFlowDuration();
    void FlowColor(Color color);
    void SetCurrentColor(Color color);
}