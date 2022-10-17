using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScript : MonoBehaviour {

    public Text title;
    private Canvas canvas;

    void Start() 
    {
        canvas = GetComponent<Canvas>();
    }

    public void ShowMenu(string text, Color textColor)
    {
        title.text = text;
        title.color = textColor;
        canvas.enabled = true;
    }
}
