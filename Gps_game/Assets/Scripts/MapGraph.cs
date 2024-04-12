using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class MapGraph : MonoBehaviour
{
    public List<Node> Nodes;
    public SplineContainer SplineContainer;
    public GameObject linePrefab;
    public Transform linesParentTransform;
    private HashSet<(Node,Node)> linesDrawn;
    private void Start()
    {
        linesDrawn = new HashSet<(Node, Node)>();
        foreach (var node in Nodes)
        { 
            node.GetNeighbours();
        }
        DrawLinesBetweenNodes();
        
    }

    public void DrawLinesBetweenNodes()
    {
        foreach (var node in Nodes)
        {
            foreach (var link in node.Links)
            {
                if (WasLineDrawn(link)) { continue; }
                DrawLineBetweenPoints(link);
            }    
        }
    }

    private bool WasLineDrawn(Link link)
    {
        return linesDrawn.Contains((link.OriginNode, link.DestinationNode)) ||
               linesDrawn.Contains((link.DestinationNode, link.OriginNode));
    }
    private void DrawLineBetweenPoints(Link link)
    {
        Instantiate(linePrefab, linesParentTransform);
        link.LineRenderer = linePrefab.GetComponent<LineRenderer>();
        link.LineRenderer.positionCount = 2;
        link.LineRenderer.SetPosition(0,link.OriginNode.transform.position);
        link.LineRenderer.SetPosition(1,link.DestinationNode.transform.position);
        
        linesDrawn.Add((link.OriginNode,link.DestinationNode));
        
    }
}
