using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToughEnemy : Enemy
{
    private const int BaseHealth = 3;
    private const float MoveSpeed = 3;

    [SerializeField] private Material[] colors;

    private int health;
    private float targetZ;

    public override int Score => 3;

    private void Start()
    {
        targetZ = transform.position.z;
        health = BaseHealth;
        ApplyColor();
    }

    private void ApplyColor()
    {
        GetComponent<MeshRenderer>().material = colors[health - 1];
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
        health--;

        if (health <= 0)
        {
            Kill();
        }
        else
        {
            ApplyColor();
        }
    }

}
