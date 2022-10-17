using System.Collections.Generic;

internal class LevelObjects
{
    public IEnumerable<Pipe> pipes;
    public IEnumerable<PipeAnimator> pipeAnimators;
    public Dictionary<NodeDescription, Node> nodes;
}