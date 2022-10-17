using UnityEngine;
using UnityEngine.UI;

public class MovesLeftTextScript : MonoBehaviour {

    private Text text;
    private LevelEndController levelEndController;

	void Start () {
        text = GetComponent<Text>();
        levelEndController = GameObject.FindObjectOfType<LevelEndController>();
        levelEndController.MoveMade += LevelEndController_MoveMade;
        int movesLeft = levelEndController.GetMovesLeft();
        SetMovesLeftTest(movesLeft);
	}

    void LevelEndController_MoveMade(object sender, MovesLeftEventArgs e)
    {
        SetMovesLeftTest(e.MovesLeft);
    }

    private void SetMovesLeftTest(int movesLeft)
    {
        text.text = "Moves left: " + (movesLeft >= 0 ? movesLeft.ToString() : "0");
    }
}
