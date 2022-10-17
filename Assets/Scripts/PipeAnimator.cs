using System;
using System.Collections;
using UnityEngine;

public class PipeAnimator : INotifyNodeColorAdded
{
    private IPipe pipe;
    private IArrowShaderScript motor;
    private ICoroutineStarter coroutineStarter;

    public event EventHandler<NodeColorAddEventArgs> ColorAdded;

    public PipeAnimator(IPipe pipe, IArrowShaderScript motor, ICoroutineStarter coroutineStarter)
    {
        this.pipe = pipe;
        this.motor = motor;
        this.coroutineStarter = coroutineStarter;

        pipe.ColorFlowStart += Pipe_ColorFlowStart;
    }

    private void Pipe_ColorFlowStart(object sender, ColorFlowStartEventArgs e)
    {
        coroutineStarter.StartCoroutine(FlowColor(e.ColorType));
    }

    private IEnumerator FlowColor(ColorType colorType)
    {
        motor.FlowColor(ColorUtility.GetColor(colorType));
        yield return new WaitForSeconds(motor.GetFlowDuration());

        INode endNode = pipe.GetEndNode();
        endNode.AddPipeColor(pipe, colorType);

        motor.SetCurrentColor(ColorUtility.GetColor(colorType));

        EventHandler<NodeColorAddEventArgs> handler = ColorAdded;
        if (handler != null)
        {
            handler(this, new NodeColorAddEventArgs() { ColorType = colorType, Pipe = pipe });
        }
    }
}
