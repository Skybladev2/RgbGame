using UnityEngine;

public class RestartButtonScript : MonoBehaviour {

    private Animator animator;
    private LevelEndController levelEndController;

	void Start () {
        levelEndController = Object.FindObjectOfType<LevelEndController>();
        levelEndController.LevelFail += LevelEndController_LevelFail;
        animator = GetComponent<Animator>();
	}

    void LevelEndController_LevelFail(object sender, System.EventArgs e)
    {
        animator.enabled = true;
    }
    
	public void RestartLevel()
    {
        levelEndController.RestartLevel();
    }
}
