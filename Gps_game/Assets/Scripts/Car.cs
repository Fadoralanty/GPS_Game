using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Car : MonoBehaviour
{
    public bool isWarned;
    public Action<Node> OnReachingDestination;
    public SplineAnimate splineAnimate;
    [SerializeField] private float warningDuration = 5f;
    private bool _hasReachedDestination;
    private Node _destinationNode;
    private void Awake()
    {
        splineAnimate = GetComponent<SplineAnimate>();
    }

    public void GoToMarkedDestination(Node node)
    {
        _destinationNode = node;
        splineAnimate.Play();
    }

    public void WarnDriver()
    {
        StartCoroutine(WarnDriverRoutine());
    }

    private IEnumerator WarnDriverRoutine()
    {
        isWarned = true;
        yield return new WaitForSeconds(warningDuration);
        isWarned = false;
    }

    private void Update()
    {
        if (_hasReachedDestination) { return; }
        if (splineAnimate.ElapsedTime >= splineAnimate.Duration)
        {
            _hasReachedDestination = true;
            OnReachingDestination?.Invoke(_destinationNode);
        }
    }
    
}
