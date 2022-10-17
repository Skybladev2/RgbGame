using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ScriptExecutionOrder(9999)]
class ScriptInitManager : MonoBehaviour
{
    private ColorFlowControllerAsyncScript colorFlowController;
    private HeroController heroController;
    private LevelEndController levelEndController;
    private ICollection<NodeColorizerScript> nodeColorizerScripts;
    private ICollection<NodeScript> nodeScripts;
    private ICollection<PipeAnimatorScript> pipeAnimatorScripts;
    private ICollection<PipeScript> pipeScripts;
    private ICollection<TargetBackgroundScript> targetBackgroundScripts;

    private static ScriptInitManager instance;

    internal static ScriptInitManager GetInstance()
    {
        if (instance == null)
        {
            GameObject managerObject = GameObject.Find("ScriptInitManager");
            if (managerObject != null)
            {
                instance = managerObject.GetComponent<ScriptInitManager>();
            }
            else
            {
                instance = (new GameObject("ScriptInitManager")).AddComponent<ScriptInitManager>();
            }

            instance.Reset();
        }

        return instance;
    }

    void Start()
    {
        foreach (var nodeScript in nodeScripts)
        {
            nodeScript.Init();
        }

        foreach (var pipeAnimatorScript in pipeAnimatorScripts)
        {
            pipeAnimatorScript.Init();
        }

        foreach (var pipeScript in pipeScripts)
        {
            pipeScript.Init();
        }

        foreach (var targetBackgroundScript in targetBackgroundScripts)
        {
            targetBackgroundScript.Init(heroController);
        }

        colorFlowController.Init(heroController,
            heroController,
            pipeAnimatorScripts.Select(s => s.GetPipeAnimator() as INotifyNodeColorAdded),
            heroController,
            pipeScripts.Select(s => s.GetPipe()));

        foreach (var nodeColorizerScript in nodeColorizerScripts)
        {
            nodeColorizerScript.Init(heroController, colorFlowController);
        }

        heroController.Init();

        var targetColors = GameObject.FindObjectsOfType<TargetColorScript>().Select(n => n.GetTargetColor());
        levelEndController.Init(targetColors, colorFlowController, heroController);
    }

    internal void Reset()
    {
        colorFlowController = null;
        heroController = null;
        levelEndController = null;
        nodeColorizerScripts = new List<NodeColorizerScript>();
        nodeScripts = new List<NodeScript>();
        pipeAnimatorScripts = new List<PipeAnimatorScript>();
        pipeScripts = new List<PipeScript>();
        targetBackgroundScripts = new List<TargetBackgroundScript>();
    }

    internal void AddScript(ColorFlowControllerAsyncScript colorFlowController)
    {
        this.colorFlowController = colorFlowController;
    }

    internal void AddScript(HeroController heroController)
    {
        this.heroController = heroController;
    }

    internal void AddScript(LevelEndController levelEndController)
    {
        this.levelEndController = levelEndController;
    }

    internal void AddScript(NodeColorizerScript nodeColorizerScript)
    {
        nodeColorizerScripts.Add(nodeColorizerScript);
    }

    internal void AddScript(NodeScript nodeScript)
    {
        nodeScripts.Add(nodeScript);
    }

    internal void AddScript(PipeAnimatorScript pipeAnimatorScript)
    {
        pipeAnimatorScripts.Add(pipeAnimatorScript);
    }

    internal void AddScript(PipeScript pipeScript)
    {
        pipeScripts.Add(pipeScript);
    }

    internal void AddScript(TargetBackgroundScript targetBackgroundScript)
    {
        targetBackgroundScripts.Add(targetBackgroundScript);
    }
}