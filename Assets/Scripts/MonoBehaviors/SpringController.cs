using UnityEngine;

public class SpringController : MonoBehaviour
{
    public SpringJoint2D SpringJoint;
    private LineRenderer lineRenderer;
    private Vector2 startPoint = Vector2.zero;
    private Vector2 connectedPoint = Vector2.zero;
    private Transform targetTransform;
    public ColorType ColorType;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Color color = ColorUtility.GetColor(ColorType);
        lineRenderer.SetColors(color, color);
    }

    public void EjectSpring(Transform connectedTransform)
    {
        this.targetTransform = connectedTransform;
        this.connectedPoint = connectedTransform.position;

        SpringJoint.connectedAnchor = connectedPoint;
        SpringJoint.anchor = Vector2.zero;

        SetLineStartPoint();
        SetLineHitPoint(connectedPoint);

        SpringJoint.enabled = true;
        lineRenderer.enabled = true;
    }

    private void SetLineHitPoint(Vector2 hitPoint)
    {
        lineRenderer.SetPosition(0, hitPoint);
    }

    public Vector3 GetConnectedPoint()
    {
        return this.connectedPoint;
    }

    public GameObject GetConnectedGameObject()
    {
        return this.targetTransform.gameObject;
    }

    public INode GetConnectedNode()
    {
        return this.targetTransform.gameObject.GetComponent<NodeScript>().GetNode();
    }

    void Update()
    {
        if (lineRenderer.enabled)
        {
            SetLineStartPoint();
        }
    }

    private void SetLineStartPoint()
    {
        startPoint = transform.TransformPoint(SpringJoint.anchor);
        lineRenderer.SetPosition(1, startPoint);
    }

    public void RetractSpring()
    {
        SpringJoint.enabled = false;
        lineRenderer.enabled = false;
        GetConnectedNode().DisconnectSpring();
    }
}
