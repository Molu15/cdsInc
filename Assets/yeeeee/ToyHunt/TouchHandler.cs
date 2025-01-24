using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TouchHandler : MonoBehaviour
{
    private ToyHuntController toyHuntController;
    private static TextMeshProUGUI messageText;
    private static float messageTimer = 0f;
    private float messageDuration = 4f;  // Increased duration for better readability
    private bool initialized = false;

    private float hideCounterDelay = 0.1f;


    void Start()
    {
        // Only try to find the message text if we haven't already
        if (messageText == null)
        {
            GameObject messageObj = GameObject.Find("Message");
            if (messageObj != null)
            {
                messageText = messageObj.GetComponent<TextMeshProUGUI>();
                if (messageText != null)
                {
                    messageText.gameObject.SetActive(false);
                }
            }
        }

        // Find ToyHuntController
        GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XR Origin");
        }

        if (xrOrigin != null)
        {
            toyHuntController = xrOrigin.GetComponent<ToyHuntController>();
            if (toyHuntController == null)
            {
                Debug.LogError($"ToyHuntController not found on {xrOrigin.name}!");
            }
            else
            {
                Debug.Log("ToyHuntController found successfully");
            }
        }
        else
        {
            Debug.LogError("Could not find XR Origin!");
        }

        initialized = true;
    }


    void Update()
    {
        if (!initialized) return;

        // Handle message timer
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
        }
        if (toyHuntController != null)
        {
            Invoke("TouchControl", hideCounterDelay);
        }

    }

    private void TouchControl()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag ("Toy"))
                {
                    string toyName = hit.collider.gameObject.GetComponent<ToyItem>().toyName;
                    bool isRequired = toyHuntController.IsToyRequired(toyName);


                    if (isRequired)
                    {
                        if (toyHuntController.IsToyAlreadyFound(toyName))
                        {
                            // Show message for already collected toys
                            if (messageText != null)
                            {
                                messageText.text = "We already have this one!";
                                messageText.gameObject.SetActive(true);
                                messageTimer = messageDuration;
                            }
                        }
                        else
                        {
                            // Collect new required toy
                            toyHuntController.ToyFound(toyName);
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        // Show message for wrong toys
                        if (messageText != null)
                        {
                            messageText.text = "It's not what we are looking for";
                            messageText.gameObject.SetActive(true);
                            messageTimer = messageDuration;
                        }
                    }
                }
            }
        }
    }
}
