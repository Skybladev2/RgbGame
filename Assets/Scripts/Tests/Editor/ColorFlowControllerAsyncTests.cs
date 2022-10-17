using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;
using System.Collections;

public class ColorFlowControllerAsyncTests
{
    // http://www.agile-code.com/blog/unit-test-asynchronous-methods-in-csharp-and-nunit/
    [Test]
    public void FlowColorAsync()
    {
        var nodesColorForcer = new Mock<INotifyNodesColorForced>();
        var nodeLeaver = new Mock<INotifyNodeLeft>();
        LevelObjects objects = CreateTestLevel();

        Mock<IHeroController> heroControllerMock = new Mock<IHeroController>();
        heroControllerMock.Setup(h => h.Freeze());

        ColorFlowControllerAsync controller = new ColorFlowControllerAsync(
            nodesColorForcer.Object,
            nodeLeaver.Object,
            objects.pipeAnimators.Cast<INotifyNodeColorAdded>(),
            heroControllerMock.Object,
            objects.pipes.Cast<IPipe>());

        var waitHandle = new AutoResetEvent(false);
        controller.ColorFlowCompleted += (sender, args) =>
        {
            waitHandle.Set();
        };

        objects.nodes.FirstOrDefault(n => n.Key.Column == 0 && n.Key.Row == 0).Value.ConnectSpring(ColorType.Red);
        objects.nodes.FirstOrDefault(n => n.Key.Column == 1 && n.Key.Row == 0).Value.ConnectSpring(ColorType.Green);
        objects.nodes.FirstOrDefault(n => n.Key.Column == 0 && n.Key.Row == -1).Value.ConnectSpring(ColorType.Blue);

        nodesColorForcer.Raise(m => m.NodesColorForced += null, new NodesColorChangedEventArgs()
        {
            Nodes = new List<INode>()
            {
                objects.nodes.FirstOrDefault(n => n.Key.Column == 0 && n.Key.Row == 0).Value,
                objects.nodes.FirstOrDefault(n => n.Key.Column == 1 && n.Key.Row == 0).Value,
                objects.nodes.FirstOrDefault(n => n.Key.Column == 0 && n.Key.Row == -1).Value
            }
        });

        double WAIT_TIME = 5;
        var pulse = waitHandle.WaitOne(TimeSpan.FromSeconds(WAIT_TIME));

        if (!pulse)
        {
            var msg = string.Format("Timeout reached: {0} sec.", WAIT_TIME);
            Assert.Fail(msg);
        }

        Dictionary<Coords, ColorType> expectedColors = new Dictionary<Coords, ColorType>();
        expectedColors.Add(new Coords(0, 0), ColorType.Red);
        expectedColors.Add(new Coords(1, 0), ColorType.Green);

        expectedColors.Add(new Coords(0, -1), ColorType.Blue);
        expectedColors.Add(new Coords(1, -1), ColorType.Neutral);
        expectedColors.Add(new Coords(2, -1), ColorType.Neutral);

        expectedColors.Add(new Coords(1, -2), ColorType.Blue);

        foreach (var expectedColor in expectedColors)
        {
            var node = objects.nodes
                .FirstOrDefault(n =>
                    n.Key.Column == expectedColor.Key.Column
                    && n.Key.Row == expectedColor.Key.Row)
                .Value;

            if (node.GetCurrentColorType() != expectedColor.Value)
            {
                Assert.Fail(String.Format("Node {0} has {1} color instead of {2}",
                    expectedColor.Key,
                    node.GetCurrentColorType(),
                    expectedColor.Value));
            }
        }

        Assert.Pass();
    }

    private static LevelObjects CreateTestLevel()
    {
        //https://docs.google.com/drawings/d/1dwT-olGCUHbCSBNpBtPbLqzO4IV4PUzMYOEJAxytEMw/edit
        List<NodeDescription> nodeDescriptions = new List<NodeDescription>();

        NodeDescription[] row0Description = new NodeDescription[2];

        nodeDescriptions.Add(new NodeDescription() { Row = 0, Column = 0 });
        row0Description[0] = nodeDescriptions.Last();

        nodeDescriptions.Add(new NodeDescription()
        {
            Row = 0,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndRow = -1, EndColumn = 1 }
            }
        });
        row0Description[1] = nodeDescriptions.Last();

        NodeDescription[] row1Description = new NodeDescription[3];

        nodeDescriptions.Add(new NodeDescription()
        {
            Row = -1,
            Column = 0,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndRow = -1, EndColumn = 1 },
                new PipeDescription() { EndRow = -2, EndColumn = 1 }
            }
        });
        row1Description[0] = nodeDescriptions.Last();

        nodeDescriptions.Add(new NodeDescription()
        {
            Row = -1,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndRow = -1, EndColumn = 2 }
            }
        });
        row1Description[1] = nodeDescriptions.Last();

        nodeDescriptions.Add(new NodeDescription() { Row = -1, Column = 2 });
        row1Description[2] = nodeDescriptions.Last();

        NodeDescription[] row2Description = new NodeDescription[1];

        nodeDescriptions.Add(new NodeDescription()
        {
            Row = -2,
            Column = 1,
            PipeDescriptions = new List<PipeDescription>()
            {
                new PipeDescription() { EndRow = -1, EndColumn = 1 }
            }

        });
        row2Description[0] = nodeDescriptions.Last();

        LevelCreator creator = new LevelCreator(1, null, null);
        Dictionary<NodeDescription, Node> nodes = new Dictionary<NodeDescription, Node>();
        nodes.AddRange(creator.CreateRowNodes(row0Description));
        nodes.AddRange(creator.CreateRowNodes(row1Description));
        nodes.AddRange(creator.CreateRowNodes(row2Description));

        IEnumerable<Pipe> pipes = creator.CreatePipes(nodes);

        ICoroutineStarter coroutineStarter = new TestCoroutineStarter();
        IEnumerable<PipeAnimator> pipeAnimators = creator.CreatePipeAnimators(pipes, coroutineStarter);

        LevelObjects objects = new LevelObjects();
        objects.nodes = nodes;
        objects.pipes = pipes;
        objects.pipeAnimators = pipeAnimators;

        return objects;
    }
}
