using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class PlaneDetectionManager : MonoBehaviour
{
    public GameObject boxPrefab;
    public AudioClip spawnSoundClip;  // Changed to AudioClip
    private ARPlaneManager arPlaneManager;
    private Camera arCamera;
    private float minDistanceBetweenPlanes = 0.5f;
    private float minDistanceFromPlayer = 0.5f;  // Minimum spawn distance
    private float maxDistanceFromPlayer = 2.0f;  // Maximum spawn distance
    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arCamera = GetComponentInChildren<Camera>();
        if (arPlaneManager == null)
        {
            Debug.LogError("AR Plane Manager not found!");
            return;
        }
        arPlaneManager.planesChanged += PlanesChanged;
        Debug.Log("PlaneDetectionManager started successfully");
    }
    void OnDestroy()
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.planesChanged -= PlanesChanged;
        }
    }
    private bool IsInPlayerRange(Vector3 position)
    {
        float distanceFromCamera = Vector3.Distance(position, arCamera.transform.position);
        return distanceFromCamera >= minDistanceFromPlayer && distanceFromCamera <= maxDistanceFromPlayer;
    }
    private void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            // Check if plane is too close to existing planes
            bool tooClose = false;
            foreach (ARPlane existingPlane in arPlaneManager.trackables)
            {
                if (plane != existingPlane &&
                    Vector3.Distance(plane.center, existingPlane.center) < minDistanceBetweenPlanes)
                {
                    tooClose = true;
                    break;
                }
            }
            if (!tooClose && plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 spawnPosition = plane.center;
                spawnPosition.y += 0.1f;
                if (IsInPlayerRange(spawnPosition))
                {
                    Debug.Log("Spawning box on new plane");
                    SpawnBox(spawnPosition);
                }
                else
                {
                    Debug.Log("Position out of player range");
                    plane.gameObject.SetActive(false);
                }
            }
            else
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
    private void SpawnBox(Vector3 position)
    {
        if (boxPrefab == null) return;
        float distance = Vector3.Distance(position, arCamera.transform.position);
        float scale = Mathf.Lerp(0.3f, 0.2f, (distance - minDistanceFromPlayer) / (maxDistanceFromPlayer - minDistanceFromPlayer));
        GameObject box = Instantiate(boxPrefab, position, Quaternion.identity);
        box.transform.localScale = new Vector3(scale, scale, scale);
        if (spawnSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(spawnSoundClip, position);  // Play sound at box position
        }
    }
}