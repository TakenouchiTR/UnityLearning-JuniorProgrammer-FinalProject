using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float XBounds = 4;
    private const float NormalZ = -4.5f;
    private const float RetreatZ = -6.1f;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject ballPrefab;

    private bool firstBallRemoved;
    private int activeBalls = 0;
    private int prevActiveBalls = 0;
    private int burstAmount = 3;
    private int burstRemaining = 3;
    private float moveSpeed = 5;
    private float burstDelay = .25f;
    private Vector3 targetPos;
    private GameObject barrelPivot;
    private HashSet<Ball> balls;

    public PlayerState State { get; set; }

    public event EventHandler BallsReturned;

    void Start()
    {
        barrelPivot = transform.Find("BarrelPivot").gameObject;
        balls = new HashSet<Ball>();
    }

    void Update()
    {
        switch (State)
        {
            case PlayerState.Aiming:
                UpdateAiming();
                break;
            case PlayerState.Shooting:
                break;
            case PlayerState.Retreating:
                UpdateRetreating();
                break;
            case PlayerState.Waiting:
                UpdateWaiting();
                break;
            case PlayerState.Returning:
                UpdateReturning();
                break;
        }
    }

    private void UpdateAiming()
    {
        AimAtMouse();

        if (Input.GetMouseButtonDown(0))
        {
            burstRemaining = burstAmount;
            Shoot();
            State = PlayerState.Shooting;
        }
    }

    private void Shoot()
    {
        burstRemaining--;
        activeBalls++;
        firstBallRemoved = false;

        Vector3 ballSpawnPosition = barrelPivot.transform.Find("BallSpawn").transform.position;
        GameObject ball = Instantiate(ballPrefab, ballSpawnPosition, ballPrefab.transform.rotation);
        ball.GetComponent<Rigidbody>().AddForce(barrelPivot.transform.forward * 10, ForceMode.Impulse);

        Ball ballScript = ball.GetComponent<Ball>();
        ballScript.Removed += OnBallRemoved;
        AddBallToSet(ballScript);

        if (burstRemaining <= 0)
        {
            State = PlayerState.Retreating;
        }
        else
        {
            Invoke(nameof(Shoot), burstDelay);
        }
    }

    private void UpdateRetreating()
    {
        if (transform.position.z > RetreatZ)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else
        {
            State = PlayerState.Waiting;
        }
    }

    private void UpdateWaiting()
    {
        if (activeBalls == 0 && prevActiveBalls != 0)
        {
            transform.position = targetPos;
            BallsReturned?.Invoke(this, EventArgs.Empty);
        }

        prevActiveBalls = activeBalls;
    }

    private void UpdateReturning()
    {
        AimAtMouse();
        if (transform.position.z < NormalZ)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            State = PlayerState.Aiming;
        }
    }

    private void AimAtMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = AngleBetweenPoints(barrelPivot.transform.position, mouseWorldPosition);
        angle = Mathf.Clamp(-angle - 90, -80, 80);

        barrelPivot.transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 180));
    }

    private void AddBallToSet(Ball ball)
    {
        foreach (Ball storedBall in balls)
        {
            Physics.IgnoreCollision(storedBall.GetComponent<Collider>(), ball.GetComponent<Collider>());
        }
        balls.Add(ball);
    }

    private float AngleBetweenPoints(Vector3 a, Vector3 b)
    {
        float angle = Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnBallRemoved(object sender, Vector3 ballPosition)
    {
        Ball ball = sender as Ball;
        if (!firstBallRemoved)
        {
            firstBallRemoved = true;
            targetPos = new Vector3( Mathf.Clamp(ballPosition.x, -XBounds, XBounds), 0, RetreatZ );
        }

        activeBalls--;
        balls.Remove(ball);
    }
}

public enum PlayerState
{
    Aiming,
    Shooting,
    Retreating,
    Waiting,
    Returning
}