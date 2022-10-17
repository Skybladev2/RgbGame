using UnityEngine;
using System.Collections;

public class ColorUtility
{
    public static Color GetColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Red:
                return Color.red;
            case ColorType.Green:
                return Color.green;
            case ColorType.Blue:
                return Color.blue;
            case ColorType.Neutral:
                return new Color(0.8f, 0.8f, 0.8f);
            default:
                return Color.black;
        }
    }
}
