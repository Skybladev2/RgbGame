using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[ScriptExecutionOrder(0)]
public class HeroController : MonoBehaviour, INotifyNodesColorForced, INotifyNodeArrived, INotifyNodeLeft, IHeroController
{
    public SpringController[] SpringControllers;
    public LayerMask defaultLayerMask;

    private Vector2 redDirection = new Vector2(-MathHelper.Cos30, MathHelper.Sin30);
    private Vector2 greenDirection = new Vector2(MathHelper.Cos30, MathHelper.Sin30);
    private Vector2 blueDirection = new Vector2(0, -1);

    public event EventHandler<NodeLeaveEventArgs> NodeLeft;
    public event EventHandler<NodeArriveEventArgs> NodeArrived;
    public event EventHandler<NodesColorChangedEventArgs> NodesColorForced;

    private float nodeDistance;
    private const int MAX_DISTANCE = 40;
    private bool frozen = false;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init()
    {
        nodeDistance = new Config().GetNodeDistance();

        EjectSpring(redDirection);
        EjectSpring(greenDirection);
        EjectSpring(blueDirection);

        EventHandler<NodesColorChangedEventArgs> colorizeHandler = NodesColorForced;

        if (colorizeHandler != null)
        {
            colorizeHandler(this, new NodesColorChangedEventArgs() { Nodes = SpringControllers.Select(n => n.GetConnectedNode()) });
        }
    }

    private void EjectSpring(Vector2 direction)
    {
        SpringController freeSpringController = GetFreeSpringController();

        if (freeSpringController != null)
        {
            if (TryEjectSpring(freeSpringController, direction))
            {
                INode nodeScript = freeSpringController.GetConnectedNode();
                nodeScript.ConnectSpring(freeSpringController.ColorType);
            }
        }
    }

    private SpringController GetFreeSpringController()
    {
        for (int i = 0; i < SpringControllers.Length; i++)
        {
            if (!SpringControllers[i].SpringJoint.enabled)
                return SpringControllers[i];
        }

        return null;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, redDirection, Color.red);
        Debug.DrawRay(transform.position, greenDirection, Color.green);
        Debug.DrawRay(transform.position, blueDirection, Color.blue);

        if (frozen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray pressRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(pressRay, Mathf.Infinity);

            if (hit)
            {
                for (int i = 0; i < SpringControllers.Length; i++)
                {
                    if (SpringControllers[i].GetConnectedGameObject() == hit.collider.gameObject)
                    {
                        return;
                    }
                }

                INode nodeScript = hit.collider.gameObject.GetComponent<NodeScript>().GetNode();

                Vector3 targetPosition = hit.collider.gameObject.transform.position;
                float maxDistance = 0;
                SpringController farthestController = SpringControllers[0];

                for (int i = 0; i < SpringControllers.Length; i++)
                {
                    float distance = (targetPosition - SpringControllers[i].GetConnectedPoint()).sqrMagnitude;

                    if (maxDistance < distance)
                    {
                        maxDistance = distance;
                        if (maxDistance > Mathf.Pow(MathHelper.Sin60 * 2 * nodeDistance, 2))
                        {
                            farthestController = null;
                        }
                        else
                        {
                            farthestController = SpringControllers[i];
                        }
                    }
                }

                if (farthestController)
                {
                    INode previousNode = farthestController.GetConnectedNode();
                    farthestController.RetractSpring();

                    EventHandler<NodeLeaveEventArgs> leaveHandler = NodeLeft;

                    if (leaveHandler != null)
                    {
                        leaveHandler(this, new NodeLeaveEventArgs() { Node = previousNode });
                    }

                    farthestController.EjectSpring(hit.transform);

                    EventHandler<NodeArriveEventArgs> arriveHandler = NodeArrived;

                    if (arriveHandler != null)
                    {
                        arriveHandler(this, new NodeArriveEventArgs() { Node = nodeScript });
                    }
                    
                    nodeScript.ConnectSpring(farthestController.ColorType);

                    EventHandler<NodesColorChangedEventArgs> colorizeHandler = NodesColorForced;

                    if (colorizeHandler != null)
                    {
                        colorizeHandler(this, new NodesColorChangedEventArgs() { Nodes = new List<INode>() { nodeScript } });
                    }
                }
            }
        }
    }

    public void TurnOffSprings()
    {
        for (int i = 0; i < SpringControllers.Length; i++)
        {
            SpringControllers[i].gameObject.SetActive(false);
        }
    }

    private bool TryEjectSpring(SpringController freeSpringController, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, HeroController.MAX_DISTANCE, defaultLayerMask);

        if (hit.collider)
        {
            freeSpringController.EjectSpring(hit.collider.transform);
            return true;
        }

        return false;
    }

    public void Freeze()
    {
        frozen = true;
    }

    public void UnFreeze()
    {
        frozen = false;
    }
}
