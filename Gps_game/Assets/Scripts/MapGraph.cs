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
    [SerializeField] private Transform playerLinesParentTransform;
    [SerializeField] private float cornerTolerance = 0.3f;
    [SerializeField] private float minimumDistanceToDraw = 5f;
    [SerializeField] private UI ui;
    [SerializeField] private Car car;
    private HashSet<(Node,Node)> _mapLinesSet;
    private HashSet<(Node,Node)> _playerLinesSet;
    private Dictionary<(Node,Node), GameObject> _playerLinesDictionary;
    private List<Node> _playerLine;
    private Vector2 _mousePosition;
    private Node _currentNode;
    private bool _isDrawingPlayerLine;
    private Camera _mainCamera;

    private void Awake()
    {
        _mapLinesSet = new HashSet<(Node, Node)>();
        _playerLinesSet = new HashSet<(Node, Node)>();
        _playerLine = new List<Node>();
        _playerLinesDictionary = new Dictionary<(Node, Node), GameObject>();
        
        _currentNode = Nodes[0];
        _playerLine.Add(_currentNode);
        
        Vector3 position = new Vector3(_playerLine[0].position.x, 0, _playerLine[0].position.y);
        BezierKnot newBezierKnot = new BezierKnot(position);
        SplineContainer.Spline.Add(newBezierKnot);
    }

    private void Start()
    {
        foreach (var node in Nodes)
        { 
            node.GetNeighbours();
        }
        DrawLinesBetweenNodes();
        _mainCamera = Camera.main;
        ui.OnStartTravel += OnStartTravel;
    }

    private void OnStartTravel()
    {
        _isDrawingPlayerLine = false;
        //generate spline path for the car to follow
        foreach (var node in _playerLine)
        {
            Vector3 position = new Vector3(node.position.x, 0, node.position.y);
            BezierKnot newBezierKnot = new BezierKnot(position);
            SplineContainer.Spline.Add(newBezierKnot);
        }
        car.GoToMarkedDestination(_currentNode);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDrawingPlayerLine = true;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ResetPlayerLine();
            _isDrawingPlayerLine = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_isDrawingPlayerLine) { return; }
        //Get Mouse position
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
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
            if (WasLineDrawn(_currentNode, targetNode, _playerLinesSet))
            {
                RemoveLine(_currentNode, targetNode, _playerLinesSet);
            }
            else
            {
                //draw player line(Red Line)
                DrawPlayerLine(_currentNode, targetNode, playerLinePrefab, _playerLinesSet);
                _currentNode = targetNode;
                _playerLine.Add(targetNode);
            }
        }
        
        // bool isOnCorner = _currentNode.neighbours.Count <= 1;
        //
        // if (isOnCorner)
        // {
        //     
        // }
    }
    public void DrawLinesBetweenNodes()
    {
        foreach (var node in Nodes)
        {
            foreach (var link in node.Links)
            {
                if (WasLineDrawn(link, _mapLinesSet)) { continue; }
                DrawLineBetweenPoints(link, linePrefab, _mapLinesSet);
            }    
        }
    }
    private bool WasLineDrawn(Link link, HashSet<(Node, Node)> LineSet)
    {
        return LineSet.Contains((link.OriginNode, link.DestinationNode)) ||
               LineSet.Contains((link.DestinationNode, link.OriginNode));
    }    
    private bool WasLineDrawn(Node from, Node to, HashSet<(Node, Node)> LineSet)
    {
        return LineSet.Contains((from, to)) ||
               LineSet.Contains((to, from));
    }

    private void RemoveLine(Node from, Node to, HashSet<(Node, Node)> LineSet)
    {
        
        
        if (LineSet.Contains((from, to)))
        {
            GameObject line = _playerLinesDictionary[(from, to)];
            _playerLinesDictionary.Remove((from, to));
            Destroy(line);
            _playerLine.RemoveAt(_playerLine.Count-1);
            LineSet.Remove((from, to));
            _currentNode = to;
        }        
        if (LineSet.Contains((to, from)))
        {
            GameObject line = _playerLinesDictionary[(to, from)];
            _playerLinesDictionary.Remove((to, from));
            Destroy(line);
            _playerLine.RemoveAt(_playerLine.Count-1);
            LineSet.Remove((to, from));
            _currentNode = to;
        }
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
    private void DrawPlayerLine(Node from, Node to, GameObject LinePrefab, HashSet<(Node,Node)> linesSet)
    {
        if (WasLineDrawn(from, to, linesSet)) { return; }
        GameObject newLine = Instantiate(LinePrefab, playerLinesParentTransform);
        LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(from.position.x, 1, from.position.y));
        lineRenderer.SetPosition(1, new Vector3(to.position.x, 1, to.position.y));
        
        linesSet.Add((from, to));
        _playerLinesDictionary.Add((from,to), newLine);
    }

    private void ResetPlayerLine()
    {
        for(int i = playerLinesParentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(playerLinesParentTransform.GetChild(i).gameObject);
        }
        _playerLine.Clear();
        _playerLinesSet.Clear();
        
        _currentNode = Nodes[0];
        _playerLine.Add(_currentNode);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(_mousePosition.x, 0, _mousePosition.y), 1);
    }
}
