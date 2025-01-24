using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARInteractionManager : MonoBehaviour
{
    // Reference to AR components
    private ARRaycastManager raycastManager;
    private Camera arCamera;

    void Start()
    {
        // Get required components
        raycastManager = GetComponent<ARRaycastManager>();
        arCamera = GetComponentInChildren<Camera>();

        if (raycastManager == null || arCamera == null)
        {
            Debug.LogError("Missing required AR components!");
        }
    }

    public bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }
}
