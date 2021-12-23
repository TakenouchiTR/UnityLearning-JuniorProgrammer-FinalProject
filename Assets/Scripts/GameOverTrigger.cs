using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    public event EventHandler TriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered game over area");
        if (other.gameObject.CompareTag("Enemy"))
        {
            TriggerEntered?.Invoke(this, EventArgs.Empty);
            Debug.Log("Enemy entered game over area");
            gameObject.SetActive(false);
        }
    }
}
