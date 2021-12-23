using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    public event EventHandler TriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TriggerEntered?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }
    }
}
