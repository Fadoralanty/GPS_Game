using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Link> Links;
    public List<Node> neighbours;
    public LayerMask NodeLayer;
    public Vector2 position;
    public float detectionRadius = 5;
    private Collider _myCollider;
    private void Awake()
    {
        _myCollider = GetComponent<Collider>();
        neighbours = new List<Node>();
        position = new Vector2(transform.position.x, transform.position.z);
    }

    public void GetNeighbours()
    {
        Links = new List<Link>();
        _myCollider.enabled = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, NodeLayer);
        _myCollider.enabled = true;
        foreach (var collider in colliders)
        {
            Node NeighbourNode = collider.GetComponent<Node>();
            neighbours.Add(NeighbourNode);
            Link newLink = new Link(this, NeighbourNode);
            Links.Add(newLink);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
