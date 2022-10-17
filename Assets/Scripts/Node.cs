using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Node : INode
{
    private Coords coords;
    private bool connectedToSpring = false;
    private ColorType? forcedColorType = null;
    private Dictionary<IPipe, ColorType> incomingColors = new Dictionary<IPipe, ColorType>();

    public event EventHandler TargetColorSet;
    public event EventHandler<NodeColorAddEventArgs> ColorAdded;

    public void ConnectSpring(ColorType colorType)
    {
        forcedColorType = colorType;
        connectedToSpring = true;
    }

    public void DisconnectSpring()
    {
        connectedToSpring = false;
    }

    public ColorType GetCurrentColorType()
    {
        if (connectedToSpring)
        {
            return GetForcedColor();
        }

        if (!incomingColors.Any())
        {
            return GetLastColor();
        }

        return GetPipesColor();
    }

    private ColorType GetPipesColor()
    {
        IEnumerable<ColorType> colorTypes = incomingColors.Values.AsEnumerable();
        bool colorsAreSame = colorTypes.All(c => c == colorTypes.FirstOrDefault());

        if (colorsAreSame)
        {
            return colorTypes.FirstOrDefault();
        }

        return ColorType.Neutral;
    }

    private ColorType GetLastColor()
    {
        if (forcedColorType.HasValue)
        {
            return forcedColorType.Value;
        }

        return ColorType.Neutral;
    }

    private ColorType GetForcedColor()
    {
        return forcedColorType.Value;
    }

    public ColorType GetInitialColorType()
    {
        throw new NotImplementedException();
    }

    public void AddPipeColor(IPipe pipe, ColorType colorType)
    {
        incomingColors[pipe] = colorType;
    }

    public bool HasTargetColorType()
    {
        throw new NotImplementedException();
    }

    public void SetCoords(int column, int row)
    {
        coords = new Coords(column, row);
    }

    public override string ToString()
    {
        return coords.ToString();
    }

    public override bool Equals(object obj)
    {
        Node other = obj as Node;
        if (other == null)
            return false;

        return ToString() == other.ToString();
    }

    public Coords GetCoords()
    {
        return coords;
    }
}
