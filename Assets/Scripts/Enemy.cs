using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Waiting,
        Moving
    }

    public EnemyState State { get; set; }

    public event EventHandler MoveFinished;

    public event EventHandler Killed;

    private void Update()
    {
        switch (State)
        {
            case EnemyState.Waiting:
                break;
            case EnemyState.Moving:
                Move();
                break;
        }
    }

    public abstract void SelectMoveTarget();

    public abstract void Move();

    public abstract void ApplyDamage();

    protected void EndMovement()
    {
        State = EnemyState.Waiting;
        MoveFinished?.Invoke(this, EventArgs.Empty);
    }

    protected void Kill()
    {
        Killed?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyDamage();
    }
}
