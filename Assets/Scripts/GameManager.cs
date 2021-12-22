using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly List<float> SpawnPositions = new List<float> { 
        -4.5f, -3.5f, -2.5f, -1.5f, -.5f, 4.5f, 3.5f, 2.5f, 1.5f, .5f
    };

    [SerializeField] private GameObject[] enemyPrefabs;

    private int movingEnemies;
    private int spawnNumber;
    private PlayerController playerController;
    private HashSet<Enemy> enemies;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.BallsReturned += OnPlayerBallsReturned;
        enemies = new HashSet<Enemy>();
        ShuffleSpawnPositions();

        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
        AddEnemy(enemyPrefabs[0]);
    }

    void AddEnemy(GameObject enemyPrefab)
    {

        float xSpawn = SpawnPositions[spawnNumber];
        spawnNumber = (spawnNumber + 1) % SpawnPositions.Count;
        
        var enemy = Instantiate(enemyPrefab, new Vector3(xSpawn, 0, 3.5f), enemyPrefab.transform.rotation);
        var enemyScript = enemy.GetComponent<Enemy>();

        enemyScript.Killed += OnEnemyKilled;
        enemyScript.MoveFinished += onEnemyMoveFinished;
        AddEnemyToSet(enemy.GetComponent<Enemy>());
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void onEnemyMoveFinished(object sender, EventArgs e)
    {
        movingEnemies--;
        if (movingEnemies <= 0)
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
        enemies.Remove(sender as Enemy);
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

}
