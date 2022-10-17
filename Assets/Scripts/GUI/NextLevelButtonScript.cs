using UnityEngine;
using UnityEngine.UI;

public class NextLevelButtonScript : MonoBehaviour {
    public Sprite ColorfulButtonSprite;
    public Sprite CrossedOutButtonSprite;

    private Image image;
    private Animator animator;
    private LevelEndController levelEndController;
    private bool levelPassed = false;

    void Start () {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        levelEndController = GameObject.FindObjectOfType<LevelEndController>();
        levelEndController.LevelPass += LevelEndController_LevelPass;
        levelEndController.LevelFail += LevelEndConroller_LevelFail;
	}

    void LevelEndConroller_LevelFail(object sender, System.EventArgs e)
    {
        image.sprite = CrossedOutButtonSprite;
    }

    void LevelEndController_LevelPass(object sender, System.EventArgs e)
    {
        image.sprite = ColorfulButtonSprite;
        animator.enabled = true;
        levelPassed = true;
    }

    public void MoveToNextLevel()
    {
        if (levelPassed)
            levelEndController.MoveToNextLevel();
    }
	
}
