using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Link
{
    public Node OriginNode;
    public Node DestinationNode;
    public LineRenderer LineRenderer;
    public Link(Node originNode, Node destinationNode)
    {
        OriginNode = originNode;
        DestinationNode = destinationNode;
    }
}
