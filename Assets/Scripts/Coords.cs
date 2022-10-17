using System;

public class Coords
{
    private readonly int column;
    private readonly int row;

    public int Column
    {
        get
        {
            return column;
        }
    }

    public int Row
    {
        get
        {
            return row;
        }
    }

    public Coords(int column, int row)
    {
        this.column = column;
        this.row = row;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        Coords other = obj as Coords;

        if (other == null)
            return false;

        return this.Column == other.Column && this.row == other.row;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Column.GetHashCode();
            hash = hash * 23 + row.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return String.Format("({0},{1})", column, row);
    }
}