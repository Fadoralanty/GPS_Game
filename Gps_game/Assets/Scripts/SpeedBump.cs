using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Car car;
        if (other.TryGetComponent<Car>(out car))
        {
            if (!car.isWarned)
            {
                GameManager.Singleton.LevelFailed();
            }
        }
    }
}
