using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float prevZVelocity;
    private bool checkVelocity;
    private Rigidbody ballRb;

    public event EventHandler<Vector3> Removed;

    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        Invoke(nameof(ToggleCheckVelocity), 2);
        prevZVelocity = ballRb.velocity.z;
    }

    // Update is called once per frame
    void Update()
    {
        //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
        var velocity = ballRb.velocity;
        if (checkVelocity && Mathf.Abs(Vector3.Dot(velocity.normalized, Vector3.forward)) < 0.1f)
        {
            ballRb.velocity = new Vector3(velocity.x, 0, -prevZVelocity);
        }
        else if (checkVelocity && Mathf.Abs(velocity.z) < .001)
        {
            Debug.Log("Very low z velocity");
        }
        prevZVelocity = velocity.z;
    }

    public void Remove()
    {
        Removed?.Invoke(this, this.transform.position);
        Destroy(gameObject);
    }

    void ToggleCheckVelocity()
    {
        checkVelocity = !checkVelocity;
    }

}
