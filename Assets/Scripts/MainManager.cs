using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private static readonly List<float> SpawnPositions = new List<float> {
        -4.5f, -3.5f, -2.5f, -1.5f, -.5f, 4.5f, 3.5f, 2.5f, 1.5f, .5f
    };

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private TextMeshProUGUI scoreLabel;

    private int movingEnemies;
    private int spawnNumber;
    private int score;
    private bool isGameOver;
    private PlayerController playerController;
    private HashSet<Enemy> enemies;

    // Start is called before the first frame update
    void Start()
    {
        GameOverTrigger trigger = FindObjectOfType<GameOverTrigger>();
        playerController = FindObjectOfType<PlayerController>();

        enemies = new HashSet<Enemy>();
        ShuffleSpawnPositions();
        UpdateScore();

        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);

        playerController.BallsReturned += OnPlayerBallsReturned;
        trigger.TriggerEntered += OnGameOverTriggerEntered;
    }

    void AddEnemy(GameObject enemyPrefab)
    {
        float xSpawn = SpawnPositions[spawnNumber];
        spawnNumber = (spawnNumber + 1) % SpawnPositions.Count;

        var enemy = Instantiate(enemyPrefab, new Vector3(xSpawn, 0, 3.5f), enemyPrefab.transform.rotation);
        var enemyScript = enemy.GetComponent<Enemy>();

        enemyScript.Killed += OnEnemyKilled;
        enemyScript.MoveFinished += OnEnemyMoveFinished;
        AddEnemyToSet(enemy.GetComponent<Enemy>());
    }

    private void AddEnemyToSet(Enemy enemy)
    {
        foreach (Enemy storedEnemy in enemies)
        {
            Physics.IgnoreCollision(storedEnemy.GetComponent<Collider>(), enemy.GetComponent<Collider>());
        }
        enemies.Add(enemy);
    }

    private void ShuffleSpawnPositions()
    {
        SpawnPositions.Shuffle();
        spawnNumber = 0;
    }

    private void UpdateScore()
    {
        scoreLabel.text = $"Score: {score}";
    }

    private void OnEnemyMoveFinished(object sender, EventArgs e)
    {
        movingEnemies--;
        if (!isGameOver && movingEnemies <= 0)
        {
            playerController.State = PlayerState.Returning;
            ShuffleSpawnPositions();
            AddEnemy(enemyPrefabs[0]);
            AddEnemy(enemyPrefabs[0]);
            AddEnemy(enemyPrefabs[0]);
            AddEnemy(enemyPrefabs[0]);
            AddEnemy(enemyPrefabs[0]);
        }
    }

    private void OnEnemyKilled(object sender, EventArgs e)
    {
        Enemy enemy = sender as Enemy;

        enemies.Remove(enemy);
        score += enemy.Score;

        UpdateScore();
    }

    private void OnPlayerBallsReturned(object sender, EventArgs e)
    {
        if (enemies.Count == 0)
        {
            playerController.State = PlayerState.Returning;
            AddEnemy(enemyPrefabs[0]);
        }

        foreach (var enemy in enemies)
        {
            enemy.SelectMoveTarget();
            enemy.State = Enemy.EnemyState.Moving;
            movingEnemies++;
        }
    }

    private void OnGameOverTriggerEntered(object sender, EventArgs e)
    {
        isGameOver = true;
        Debug.Log("Enemy entered game over area (event)");
    }
}
