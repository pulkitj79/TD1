using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Makes placed buildings draggable back to Action Panel or to other grid cells
/// </summary>
public class BuildingDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("References")]
    private Building building;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    private Vector2Int originalGridPosition;
    
    [Header("Drag State")]
    private bool isDragging = false;
    private GameObject dragPreview;
    private Camera mainCamera;

    private void Start()
    {
        Debug.Log($"[BuildingDrag] BuildingDragHandler initialized on {gameObject.name}");
        
        if (building == null)
        {
            Debug.LogError($"[BuildingDrag] Building component is NULL on {gameObject.name}!");
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"[BuildingDrag] SpriteRenderer is NULL on {gameObject.name}");
        }
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError($"[BuildingDrag] No Box Collider 2D on {gameObject.name}! Cannot receive drag events!");
        }
        else
        {
            Debug.Log($"[BuildingDrag] Collider found: {collider.size}, IsTrigger: {collider.isTrigger}");
        }
    }

    private void Awake()
    {
        building = GetComponent<Building>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[BuildingDrag] Clicked {building.Data.buildingName}");
        // Future: Show building info, upgrade options, etc.
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (building == null) return;
        
        Debug.Log($"[BuildingDrag] Started dragging {building.Data.buildingName}");
        
        isDragging = true;
        originalPosition = transform.position;
        originalGridPosition = building.GridPosition;
        
        // Create drag preview
        dragPreview = new GameObject("DragPreview");
        SpriteRenderer previewRenderer = dragPreview.AddComponent<SpriteRenderer>();
        previewRenderer.sprite = spriteRenderer.sprite;
        previewRenderer.color = new Color(1, 1, 1, 0.6f); // Semi-transparent
        previewRenderer.sortingOrder = 100; // Above everything
        
        // Make original semi-transparent while dragging
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);
        
        // Free up grid cells
        GameManager.Instance.Grid.RemoveBuildingFromGrid(building, originalGridPosition);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || dragPreview == null) return;
        
        // Move preview to mouse position
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        dragPreview.transform.position = worldPos;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        Debug.Log("[BuildingDrag] Ended drag");
        
        isDragging = false;
        
        // Destroy preview
        if (dragPreview != null)
        {
            Destroy(dragPreview);
        }
        
        // Check where we dropped
        Vector2 dropWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        
        // Is it over the Action Panel?
        if (IsOverActionPanel(eventData.position))
        {
            ReturnToActionPanel();
        }
        // Is it over a valid grid cell?
        else
        {
            TryPlaceOnGrid(dropWorldPos);
        }
    }
    
    private bool IsOverActionPanel(Vector2 screenPos)
    {
        // Check if screen position is in bottom 25% of screen (where Action Panel is)
        float screenHeight = Screen.height;
        return screenPos.y < (screenHeight * 0.25f);
    }
    
    private void ReturnToActionPanel()
    {
        Debug.Log("[BuildingDrag] Returning to Action Panel");
        
        // For now, just destroy the building
        // TODO: Later we'll add it back to panel as a draggable asset
        GameManager.Instance.Buildings.RemoveBuilding(building);
        Destroy(gameObject);
        
        Debug.Log("[BuildingDrag] Building removed from grid (Action Panel return not yet implemented)");
    }
    
    private void TryPlaceOnGrid(Vector2 worldPos)
    {
        Vector2Int newGridPos = GameManager.Instance.Grid.GetGridPosition(worldPos);
        
        // Check if there's already a building at this position
        Building existingBuilding = GameManager.Instance.Grid.GetBuildingAtPosition(newGridPos);
        
        if (existingBuilding != null && existingBuilding != building)
        {
            // Check if we can merge
            if (CanMergeWith(existingBuilding))
            {
                MergeBuildings(existingBuilding);
                return;
            }
            else
            {
                Debug.Log("[BuildingDrag] Can't merge - different building or level");
                ReturnToOriginalPosition();
                return;
            }
        }
        
        // Normal placement logic
        bool canPlace = GameManager.Instance.Grid.CanPlaceBuilding(newGridPos, building.Data.size);
        
        if (canPlace)
        {
            // Place at new position
            Debug.Log($"[BuildingDrag] Moving to new position {newGridPos}");
            
            building.GridPosition = newGridPos; // Now this works!
            transform.position = GameManager.Instance.Grid.GetWorldPosition(newGridPos.x, newGridPos.y);
            
            GameManager.Instance.Grid.PlaceBuilding(building, newGridPos);
            
            // Restore opacity
            spriteRenderer.color = Color.white;
        }
        else
        {
            // Can't place - return to original position
            Debug.Log("[BuildingDrag] Invalid placement, returning to original position");
            ReturnToOriginalPosition();
        }
    }
    
    private bool CanMergeWith(Building other)
    {
        // Must be same building type and same level
        return building.Data.buildingID == other.Data.buildingID &&
               building.CurrentLevel == other.CurrentLevel &&
               building.CurrentLevel < 4; // Max level is 4
    }
    
    private void MergeBuildings(Building target)
    {
        Debug.Log($"[BuildingDrag] Merging {building.Data.buildingName} into {target.Data.buildingName}");
        
        // Upgrade target building
        target.UpgradeLevel(target.CurrentLevel + 1);
        
        // Change sprite color to show upgrade (temporary visual)
        SpriteRenderer targetRenderer = target.GetComponent<SpriteRenderer>();
        if (targetRenderer != null)
        {
            // Darker color for higher levels
            float colorFactor = 1f - (target.CurrentLevel * 0.15f);
            targetRenderer.color = new Color(colorFactor, colorFactor, 1f); // Darker blue
        }
        
        // Destroy this building
        GameManager.Instance.Buildings.RemoveBuilding(building);
        Destroy(gameObject);
        
        Debug.Log($"[BuildingDrag] Merge complete! {target.Data.buildingName} now level {target.CurrentLevel}");
    }
    
    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
        building.GridPosition = originalGridPosition;
        GameManager.Instance.Grid.PlaceBuilding(building, originalGridPosition);
        spriteRenderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        Debug.Log($"[BuildingDrag] OnMouseDown detected on {gameObject.name}!");
    }

    private void OnMouseOver()
    {
        // Uncomment to test (will spam console)
        // Debug.Log($"[BuildingDrag] Mouse over {gameObject.name}");
    }

}