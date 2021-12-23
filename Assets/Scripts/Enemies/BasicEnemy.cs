using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    private const float MoveSpeed = 5;

    private float targetZ;

    public override int Score => 1;

    private void Start()
    {
        targetZ = transform.position.z;
    }

    public override void Move()
    {
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        if (transform.position.z < targetZ)
        {
            transform.position = new Vector3(transform.position.x, 0, targetZ);
            EndMovement();
        }
    }

    public override void SelectMoveTarget()
    {
        targetZ = transform.position.z - 1;
    }

    public override void ApplyDamage()
    {
        Kill();
    }

}
