using System;

public interface INode {
    Coords GetCoords();
    void SetCoords(int column, int row);

    void ConnectSpring(ColorType colorType);
    void DisconnectSpring();

    ColorType GetCurrentColorType();
    ColorType GetInitialColorType();

    void AddPipeColor(IPipe pipe, ColorType colorType);

    bool HasTargetColorType();
    event EventHandler TargetColorSet;
}
