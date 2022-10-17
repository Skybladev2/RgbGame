using UnityEngine;

[ExecuteInEditMode]
public class TargetColorScript : MonoBehaviour {

    public ColorType TargetColorType;
    private ITargetColor targetColor;

	void Start () {
        targetColor = new TargetColor(TargetColorType, GetComponentInParent<NodeScript>().GetNode());
	}

    internal ITargetColor GetTargetColor()
    {
        return targetColor;
    }
}
