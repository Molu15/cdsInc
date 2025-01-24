using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class TiltControl : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject playerInstance;


    private float targetPositionX = 0f;  // Target X position based on tilt
    private float currentPositionX = 0f; // Current X position for smooth movement
    private float movementX = 0.5f;

    private GameObject[] lanes;
    public GameObject lanePrefab;

    private Animator animator;

    public TMP_Text debug;


    void Start()
    {
        // Instantiate player prefab at the starting position
        Vector3 spawnPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z + 2);
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        currentPositionX = spawnPosition.x;
        targetPositionX = spawnPosition.x;

        FlappyCatController.Instance.setPlayer(playerInstance);

        animator = playerInstance.GetComponent<Animator>();
        animator.enabled = true;
        if (animator.GetBool("isActive"))
        {
            animator.SetBool("isActive", false);
        }

        lanes = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            float xPosition = spawnPosition.x + 0.75f - movementX * i;
            lanes[i] = Instantiate(lanePrefab, new Vector3(xPosition, spawnPosition.y, spawnPosition.z + 3.5f), Quaternion.identity);
        }
    }

    void Update()
    {
        if (FlappyCatController.Instance.active == true)
        {
            animator.SetBool("isActive", true);
        }
        else
        {
            animator.SetBool("isActive", false);
        }
        // Read the device tilt along the X-axis
        float tilt = Input.acceleration.x;

        if (tilt < -0.4f && currentPositionX >= Camera.main.transform.position.x)
        {
            //targetPositionX = 1177.046f;
            targetPositionX = Camera.main.transform.position.x - movementX;
        }
        else if (tilt > 0.4f && currentPositionX <= Camera.main.transform.position.x)
        {
            //targetPositionX = 1178.046f;
            targetPositionX = Camera.main.transform.position.x + movementX;
        }
        else if (0.1f > tilt && tilt > -0.1f)
        {
            //targetPositionX = 1177.546f;
            targetPositionX = Camera.main.transform.position.x;
        }

        currentPositionX = Mathf.Lerp(currentPositionX, targetPositionX, Time.deltaTime * 6f);
        //currentPositionX = targetPositionX;

        // Update the player's position
        Vector3 playerPosition = playerInstance.transform.position;
        playerPosition.x = currentPositionX;
        playerInstance.transform.position = playerPosition;
    }
}
