using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : Enemy
{
    private const int MaxSpacesToMove = 2;
    private const float MoveSpeed = 5;

    private float targetZ;
    private int spacesMoved = 0;

    public override int Score => 2;

    // Start is called before the first frame update
    void Start()
    {
        targetZ = transform.position.z;
    }

    public override void ApplyDamage()
    {
        Kill();
    }

    public override void Move()
    {
        transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        if (transform.position.z < targetZ)
        {
            transform.position = new Vector3(transform.position.x, 0, targetZ);
            spacesMoved++;

            if (spacesMoved >= 2)
            {
                EndMovement();
                return;
            }

            Ray ray = new Ray(transform.position, Vector3.back);

            if (Physics.Raycast(ray, out _, 1))
            {
                EndMovement();
            }
            else
            {
                targetZ = transform.position.z - 1;
            }
        }
    }

    public override void SelectMoveTarget()
    {
        targetZ = transform.position.z - 1;
        spacesMoved = 0;
    }
}
