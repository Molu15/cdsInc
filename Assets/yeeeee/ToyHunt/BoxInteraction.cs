using UnityEngine;
public class BoxInteraction : MonoBehaviour
{
    [SerializeField] private GameObject[] collectiblePrefabs;
    [SerializeField] private GameObject invisiblePlanePrefab;
    private Camera arCamera;
    void Start()
    {
        arCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                Debug.Log("Box touched!");
                SpawnCollectibleAndPlane();
            }
        }
    }
    private void SpawnCollectibleAndPlane()
    {
        if (collectiblePrefabs.Length == 0) return;
        Vector3 boxPosition = transform.position;
        // First spawn the invisible plane exactly at box position
        GameObject plane = Instantiate(invisiblePlanePrefab, new Vector3(boxPosition.x, boxPosition.y - 0.1f, boxPosition.z), Quaternion.identity);
        // Spawn collectible a bit above
        int randomIndex = Random.Range(0, 4);
        GameObject selectedCollectible = collectiblePrefabs[randomIndex];
        Vector3 spawnPosition = boxPosition;
        spawnPosition.y += 0.2f; // Spawn higher so we can see it fall
        GameObject spawnedCollectible = Instantiate(selectedCollectible, spawnPosition, Quaternion.identity);

        Destroy(gameObject);
    }
}