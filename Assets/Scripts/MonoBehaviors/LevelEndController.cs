using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[ScriptExecutionOrder(0)]
public class LevelEndController : MonoBehaviour
{
    public event EventHandler LevelPass;
    public event EventHandler LevelFail;
    public event EventHandler<MovesLeftEventArgs> MoveMade;

    private IEnumerable<ITargetColor> targets;
    private uint movesMade;
    private bool levelPassed = false;
    private LevelCreator levelCreator;
    private IConfig config = new Config();

    [SerializeField]
    private uint movesLimit;
    [SerializeField]
    private int levelIndex = 0;

    void Start()
    {
        ScriptInitManager.GetInstance().AddScript(this);
    }

    internal void Init(IEnumerable<ITargetColor> targets,
        INotifyColorFlowCompletedEvent colorFlowController,
        INotifyNodeArrived heroController)
    {
        levelCreator = new LevelCreator(config.GetNodeDistance(),
                    Levels.GetLevelDescriptions(),
                     prefabName =>
                     {
                         GameObject clone = (GameObject)Instantiate(Resources.Load(prefabName));
                         clone.name = clone.name.Replace("(Clone)", "");
                         return clone;
                     });
        movesMade = 0;

        this.targets = targets;
        colorFlowController.ColorFlowCompleted += ColorFlowController_ColorFlowCompleted;
        heroController.NodeArrived += HeroController_NodeArrived;

        SetMovesLeft();
    }

    void HeroController_NodeArrived(object sender, NodeArriveEventArgs e)
    {
        movesMade++;
    }

    void ColorFlowController_ColorFlowCompleted(object sender, EventArgs e)
    {
        if (levelPassed)
            return;

        SetMovesLeft();

        if (targets.All(t => t.MatchesNodeColor()))
        {
            if (GetMovesLeft() >= 0)
            {
                PassLevel();
            }
        }

        if (GetMovesLeft() <= 0)
        {
            FailLevel();
        }
    }

    internal void MoveToNextLevel()
    {
        ScriptInitManager.GetInstance().Reset();
        levelCreator.CreateLevel(levelIndex + 1);
    }

    internal void RestartLevel()
    {
        ScriptInitManager.GetInstance().Reset();
        levelCreator.CreateLevel(levelIndex);
    }

    private void SetMovesLeft()
    {
        EventHandler<MovesLeftEventArgs> handler = MoveMade;
        if (handler != null)
        {
            handler(this, new MovesLeftEventArgs() { MovesLeft = GetMovesLeft() });
        }
    }

    private void FailLevel()
    {
        Debug.Log("LEVEL FAIL");

        EventHandler handler = LevelFail;

        if (handler != null)
        {
            handler(this, new EventArgs());
        }
    }

    internal int GetMovesLeft()
    {
        return (int)(movesLimit - movesMade);
    }

    private void PassLevel()
    {
        levelPassed = true;
        Debug.Log("LEVEL END");

        EventHandler handler = LevelPass;

        if (handler != null)
        {
            handler(this, new EventArgs());
        }
    }

    public void SetMovesLimit(uint movesLimit)
    {
        this.movesLimit = movesLimit;
    }

    public void SetLevelIndex(int levelIndex)
    {
        this.levelIndex = levelIndex;
        Debug.Log("Level index " + this.levelIndex);
    }

}
