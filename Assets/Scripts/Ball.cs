using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ball : MonoBehaviour
{
    private float prevZVelocity;
    private int collisionListIndex;
    private bool checkVelocity;
    private List<float> wallZCollisions;
    private Rigidbody ballRb;

    public event EventHandler<Vector3> Removed;

    // Start is called before the first frame update
    void Start()
    {
        wallZCollisions = new List<float>();
        ballRb = GetComponent<Rigidbody>();
        prevZVelocity = ballRb.velocity.z;

        for (int i = 0; i < 5; i++)
        {
            wallZCollisions.Add(-100);
        }

        Invoke(nameof(ToggleCheckVelocity), 2);
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            wallZCollisions[collisionListIndex] = transform.position.z;
            collisionListIndex = (collisionListIndex + 1) % wallZCollisions.Count;

            float differences = 0;
            for (int i = 1; i < wallZCollisions.Count; i++)
            {
                differences += Mathf.Abs(wallZCollisions[i] - wallZCollisions[i - 1]);
            }

            if (differences / (wallZCollisions.Count - 1) <= .01f)
            {
                ballRb.AddForce(Vector3.back, ForceMode.Impulse);
            }
        }
    }
}
