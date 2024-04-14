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
    public GameObject playerLinePrefab;
    public Transform linesParentTransform;
    [SerializeField] private float cornerTolerance = 0.3f;
    [SerializeField] private float minimumDistanceToDraw = 5f;
    private HashSet<(Node,Node)> _linesDrawn;
    private HashSet<(Node,Node)> _playerLinesDrawn;
    private List<Node> playerLine;
    private Vector2 _mousePosition;
    private Node _currentNode;
    private void Start()
    {
        _linesDrawn = new HashSet<(Node, Node)>();
        _playerLinesDrawn = new HashSet<(Node, Node)>();
        foreach (var node in Nodes)
        { 
            node.GetNeighbours();
        }
        DrawLinesBetweenNodes();
        _currentNode = Nodes[0];
    }

    private void FixedUpdate()
    {
        //Get Mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        _mousePosition = new Vector2(hit.point.x, hit.point.z);
        
        //get closest neighbour to mouse that is within a minimun range
        List<Node> nodeNeighbourNodes = _currentNode.neighbours;
        Node targetNode;
        float currentDistance = Vector2.Distance(_mousePosition, _currentNode.neighbours[0].position);
        targetNode = _currentNode.neighbours[0];
        foreach (var node in nodeNeighbourNodes)
        {
            float distance = Vector2.Distance(_mousePosition, node.position);
            if (distance < currentDistance && distance < minimumDistanceToDraw)
            {
                currentDistance = distance;
                targetNode = node;
            }
        }
        if (currentDistance < minimumDistanceToDraw)
        {
            //draw line
            DrawLineBetweenPoints(_currentNode, targetNode, playerLinePrefab, _playerLinesDrawn);
            _currentNode = targetNode;
        }
        
        bool isOnCorner = _currentNode.neighbours.Count <= 1;

        if (isOnCorner)
        {
            
        }
    }
    public void DrawLinesBetweenNodes()
    {
        foreach (var node in Nodes)
        {
            foreach (var link in node.Links)
            {
                if (WasLineDrawn(link)) { continue; }
                DrawLineBetweenPoints(link, linePrefab, _linesDrawn);
            }    
        }
    }
    
    private bool WasLineDrawn(Link link)
    {
        return _linesDrawn.Contains((link.OriginNode, link.DestinationNode)) ||
               _linesDrawn.Contains((link.DestinationNode, link.OriginNode));
    }
    private void DrawLineBetweenPoints(Link link, GameObject LinePrefab, HashSet<(Node,Node)> linesSet)
    {
        GameObject newLine = Instantiate(LinePrefab, linesParentTransform);
        link.LineRenderer = newLine.GetComponent<LineRenderer>();
        link.LineRenderer.positionCount = 2;
        link.LineRenderer.SetPosition(0,link.OriginNode.transform.position);
        link.LineRenderer.SetPosition(1,link.DestinationNode.transform.position);
        
        linesSet.Add((link.OriginNode,link.DestinationNode));
        
    }
    private void DrawLineBetweenPoints(Node from, Node to, GameObject LinePrefab, HashSet<(Node,Node)> linesSet)
    {
        if (linesSet.Contains((from,to)) || linesSet.Contains((to, from))) { return; }
        GameObject newLine = Instantiate(LinePrefab, linesParentTransform);
        LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(from.position.x, 1, from.position.y));
        lineRenderer.SetPosition(1, new Vector3(to.position.x, 1, to.position.y));
        
        linesSet.Add((from, to));
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(_mousePosition.x, 0, _mousePosition.y), 1);
    }
}
