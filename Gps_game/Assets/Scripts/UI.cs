using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Action OnStartTravel;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject gameOverScreen;
    private void Awake()
    {
        victoryScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }    
    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void StartTravelButton()
    {
        OnStartTravel.Invoke();
    }
}
