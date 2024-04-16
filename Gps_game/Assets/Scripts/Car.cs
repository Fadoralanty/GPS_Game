using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Car : MonoBehaviour
{
    public Action<Node> OnReachingDestination;
    private SplineAnimate _splineAnimate;
    private bool _hasReachedDestination;
    private Node _destinationNode;
    private void Awake()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    public void GoToMarkedDestination(Node node)
    {
        _destinationNode = node;
        _splineAnimate.Play();
    }

    private void FixedUpdate()
    {
        if (_hasReachedDestination) { return; }
        if (_splineAnimate.ElapsedTime >= _splineAnimate.Duration)
        {
            _hasReachedDestination = true;
            OnReachingDestination?.Invoke(_destinationNode);
        }

    }
}
