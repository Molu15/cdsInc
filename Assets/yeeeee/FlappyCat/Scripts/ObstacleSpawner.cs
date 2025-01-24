using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;

    //public int spawnRate = 5; // Time between spawns
    public float spawnDistance = 6f; // Distance in front of the camera to spawn obstacles

    private readonly float[] spawnPositionsX = { -0.5f, 0f, 0.5f }; // Left, Center, Right positions
    private Vector3 cameraSpawn;
    private int counter = 0;
    private GameObject lastPrefab;

    float timer = 0;

    private void Start()
    {
        // Start the coroutine to delay obstacle spawning
        StartCoroutine(StartSpawningAfterDelay(2f));
        cameraSpawn = Camera.main.transform.position;

    }

    private IEnumerator StartSpawningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // After the delay, start the Update loop for spawning
        enabled = true;
    }


    void Update()
    {
        if (FlappyCatController.Instance.active == true)
        {
            timer += Time.deltaTime;
            if (timer > 3)
            {
                SpawnObstacleRow();
                timer = 0;
            }
        }
    }


    private void SpawnObstacleRow()
    {
        if (counter < FlappyCatController.Instance.maxObstacles)
        {
            // Create a new instance of the Random class
            System.Random random = new System.Random();

            // Get a random number between 1 and 3 (inclusive)
            int randomNumber = random.Next(1, 4);
            for (int i = 1; i < 4; i++) // Spawn two obstacles per row
            {
                if (randomNumber != i)
                {
                    Vector3 spawnPosition = new Vector3(
                        cameraSpawn.x + spawnPositionsX[i-1],
                        cameraSpawn.y - 0.5f,
                        cameraSpawn.z + spawnDistance
                    );

                    //GameObject obstacle = Instantiate(RandomObject(), spawnPosition, Quaternion.identity);
                    GameObject obstacle = Instantiate(RandomObject(), spawnPosition, Quaternion.Euler(0, Random.Range(0, 36) * 10, 0));
                    obstacle.transform.SetParent(null); // Explicitly remove any parent
                    lastPrefab = obstacle;
                }

            }
            counter++;
        }
        else
        {
            FlappyCatController.Instance.FinishGame(lastPrefab);
        }
    }

    private GameObject RandomObject ()
    {
        int random = Random.Range(1, obstacles.Length);

        return obstacles[random-1];
    }

}
