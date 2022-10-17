using UnityEngine;
using System.Collections;

public class NodeCoordsCalculator {

	public Vector2 GetCoords(int column, int row, float nodeDistance)
    {
        float x = 0;

        if (row % 2 == 0)
        {
            x = nodeDistance * column;
        }
        else
        {
            x = nodeDistance * column + nodeDistance * MathHelper.Cos60;
        }

        float y = 0;

        y = nodeDistance * MathHelper.Sin60 * row;

        return new Vector2(x, y);
    }
}
