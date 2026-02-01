using UnityEngine;

/// <summary>
/// Debug tool to see what raycasts are hitting
/// </summary>
public class RaycastDebugger : MonoBehaviour
{
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DebugRaycast();
        }
    }
    
    private void DebugRaycast()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        Debug.Log($"=== RAYCAST DEBUG ===");
        Debug.Log($"Mouse Screen: {Input.mousePosition}");
        Debug.Log($"Mouse World: {mouseWorldPos}");
        
        // Try Physics2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            Debug.Log($"✅ HIT: {hit.collider.gameObject.name}");
            Debug.Log($"   Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            Debug.Log($"   Tag: {hit.collider.tag}");
            Debug.Log($"   Position: {hit.collider.transform.position}");
            Debug.Log($"   Has SimpleDrag: {hit.collider.GetComponent<SimpleBuildingDragHandler>() != null}");
        }
        else
        {
            Debug.Log($"❌ NO HIT!");
        }
        
        // Check all hits
        RaycastHit2D[] allHits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        Debug.Log($"Total hits: {allHits.Length}");
        for (int i = 0; i < allHits.Length; i++)
        {
            Debug.Log($"  [{i}] {allHits[i].collider.gameObject.name}");
        }
        
        Debug.Log($"===================");
    }
}