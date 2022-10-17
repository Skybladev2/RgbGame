using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDescription {
    public int Row { get; set; }
    public int Column { get; set; }
    public ColorType? TargetColor { get; set; }
    public List<PipeDescription> PipeDescriptions { get; set; }
}
