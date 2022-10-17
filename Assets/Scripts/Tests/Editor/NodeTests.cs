using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Moq;

public class NodeTests {

    [Test]
    public void GetCurrentColorType_NoPipes()
    {
        Node node = new Node();
        
        Assert.AreEqual(ColorType.Neutral, node.GetCurrentColorType());
    }

    [Test]
    public void GetCurrentColorType_NoPipes_AfterConnection()
    {
        Node node = new Node();
        node.ConnectSpring(ColorType.Blue);

        Assert.AreEqual(ColorType.Blue, node.GetCurrentColorType());
    }

    [Test]
    public void GetCurrentColorType_NoPipes_AfterDisconnection()
    {
        Node node = new Node();
        node.ConnectSpring(ColorType.Blue);
        node.DisconnectSpring();

        Assert.AreEqual(ColorType.Blue, node.GetCurrentColorType());
    }

    [Test]
    public void GetCurrentColorType_OnePipe()
    {
        Node node = new Node();
        IPipe pipeMock = new Mock<IPipe>().Object;

        node.AddPipeColor(pipeMock, ColorType.Blue);

        Assert.AreEqual(ColorType.Blue, node.GetCurrentColorType());
    }

    [Test]
    public void GetCurrentColorType_TwoPipes()
    {
        Node node = new Node();
        IPipe pipeMock1 = new Mock<IPipe>().Object;
        IPipe pipeMock2 = new Mock<IPipe>().Object;

        node.AddPipeColor(pipeMock1, ColorType.Blue);
        node.AddPipeColor(pipeMock2, ColorType.Green);

        Assert.AreEqual(ColorType.Neutral, node.GetCurrentColorType());
    }

    [Test]
    public void Colorize()
    {
        Node node = new Node();
        IPipe pipeMock1 = new Mock<IPipe>().Object;
        IPipe pipeMock2 = new Mock<IPipe>().Object;

        node.AddPipeColor(pipeMock1, ColorType.Blue);
        node.AddPipeColor(pipeMock2, ColorType.Green);

        node.ConnectSpring(ColorType.Red);

        Assert.AreEqual(ColorType.Red, node.GetCurrentColorType());
    }
}
