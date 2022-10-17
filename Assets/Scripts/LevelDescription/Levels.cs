using System;
using System.Collections.Generic;
using System.Linq;

public class Levels
{
    private static List<NodeDescription> nodes = null;

    private static List<LevelDescription> levelDescriptions = new List<LevelDescription>();

    public static List<LevelDescription> GetLevelDescriptions()
    {
        return levelDescriptions;
    }

    static Levels()
    {
        #region Level
        nodes = new List<NodeDescription>();

        nodes.Add(new NodeDescription() { Row = 0, Column = 0 });
        nodes.Add(new NodeDescription() { Row = 0, Column = 1 });

        nodes.Add(new NodeDescription() { Row = -2, Column = 0, TargetColor = ColorType.Red });
        nodes.Add(new NodeDescription() { Row = -2, Column = 1, TargetColor = ColorType.Green });

        nodes.AddRange(AddFillerNodes(nodes));

        levelDescriptions.Add(new LevelDescription()
        {
            MovesLimit = 3,
            Nodes = nodes
        });
        #endregion

        #region Level
        nodes = new List<NodeDescription>();

        nodes.Add(new NodeDescription() { Row = 0, Column = -1, TargetColor = ColorType.Green });
        
        nodes.Add(new NodeDescription()
        {
            Row = -1,
            Column = -1,
            PipeDescriptions = new List<PipeDescription>()
        {
            new PipeDescription() { EndRow = 0, EndColumn =-1 }
        }
        });
        nodes.Add(new NodeDescription()
        {
            Row = -1,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
        {
            new PipeDescription() { EndRow = -2, EndColumn = 1 }
        }
        });

        nodes.Add(new NodeDescription() { Row = -2, Column = 1, TargetColor = ColorType.Red });


        nodes.AddRange(AddFillerNodes(nodes));

        levelDescriptions.Add(new LevelDescription()
        {
            MovesLimit = 3,
            Nodes = nodes
        });
        #endregion

        #region Level
        nodes = new List<NodeDescription>();

        nodes.Add(new NodeDescription()
        {
            Row = 0,
            Column = 0,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndColumn = 1, EndRow = 0 },
                new PipeDescription () { EndColumn = 0, EndRow = -1}
            }
        });
        nodes.Add(new NodeDescription()
        {
            Row = 0,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndColumn = 2, EndRow = 0}
            }
        });
        nodes.Add(new NodeDescription()
        {
            Row = 0,
            Column = 2,
            TargetColor = ColorType.Blue
        });

        nodes.Add(new NodeDescription()
        {
            Row = -1,
            Column = 0,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndColumn = 1, EndRow = -2 }
            }
        });
        nodes.Add(new NodeDescription()
        {
            Row = -1,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription (){ EndColumn = 2, EndRow = 0 }
            }
        });

        nodes.Add(new NodeDescription()
        {
            Row = -2,
            Column = 1,
            TargetColor = ColorType.Green,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription () { EndColumn = 1, EndRow = -1 }
            }
        });

        nodes.AddRange(AddFillerNodes(nodes));

        levelDescriptions.Add(new LevelDescription()
        {
            MovesLimit = 5,
            Nodes = nodes
        });
        #endregion
    }

    private static IEnumerable<NodeDescription> AddFillerNodes(List<NodeDescription> nodes)
    {
        List<NodeDescription> fillerNodes = new List<NodeDescription>(400);

        for (int column = -10; column < 10; column++)
        {
            for (int row = -10; row < 10; row++)
            {
                if (!nodes
                    .Select(n => new Coords(n.Column, n.Row))
                    .Contains(new Coords(column, row)))
                {
                    fillerNodes.Add(new NodeDescription() { Column = column, Row = row });
                }
            }
        }

        return fillerNodes;
    }
}
