using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    [SerializeField] private Node levelObjectiveNode;
    [SerializeField] private Car car;
    [SerializeField] private UI UI;
    private bool _isGameOver;
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
        _isGameOver = false;
    }

    private void OnReachingDestination(Node node)
    {
        if (_isGameOver) {return; }
        if (node == levelObjectiveNode)
        {
            LevelCompleted();
        }
        else
        {
            LevelFailed();
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void LevelFailed()
    {
        UI.ShowGameOverScreen();
        _isGameOver = true;
    }
    private void LevelCompleted()
    {
        UI.ShowVictoryScreen();
    }
}
