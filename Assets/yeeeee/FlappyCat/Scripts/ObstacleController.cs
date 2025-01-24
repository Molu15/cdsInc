using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObstacleController : MonoBehaviour
{
    public float speed = 1f; // Speed of obstacle movement towards the player
    private float cameraZPosition = 0f; // Z-position where the camera is fixed
    private bool alreadyPassedPlayer = false;


    private void Update()
    {
        if (FlappyCatController.Instance.active)
        {
            // Move the obstacle towards the camera
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
            if (!alreadyPassedPlayer && transform.position.z < FlappyCatController.Instance.getPlayer().transform.position.z)
            {
                FlappyCatController.Instance.UpdateCounter();
                alreadyPassedPlayer = true;
            }
            
            if (transform.position.z < Camera.main.transform.position.z)
            {
                Destroy(gameObject);

            }
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the obstacle collides with the player (camera)
        if (other.CompareTag("Player"))
        {
            TriggerGameOver();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over! An obstacle hit the player.");
        // Call Game Over logic here
        FlappyCatController.Instance.GameOver(); // Assuming you have a GameManager instance
        Destroy(gameObject); // Optionally destroy the obstacle after collision
    }
}