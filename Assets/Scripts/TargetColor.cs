internal class TargetColor : ITargetColor
{
    private INode node;
    private ColorType colorType;

    public TargetColor(ColorType colorType, INode node)
    {
        this.colorType = colorType;
        this.node = node;
    }

    public bool MatchesNodeColor()
    {
        return node.GetCurrentColorType() == colorType;
    }
}