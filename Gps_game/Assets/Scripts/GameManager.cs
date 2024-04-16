using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    [SerializeField] private Node levelObjectiveNode;
    [SerializeField] private Car car;
    [SerializeField] private UI UI;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        car.OnReachingDestination += OnReachingDestination;
    }

    private void OnReachingDestination(Node node)
    {
        if (node == levelObjectiveNode)
        {
            LevelCompleted();
        }
        else
        {
            LevelFailed();
        }
    }

    private void LevelFailed()
    {
        UI.ShowGameOverScreen();
    }
    private void LevelCompleted()
    {
        UI.ShowVictoryScreen();
    }
}
