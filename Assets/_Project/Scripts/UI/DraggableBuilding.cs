using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Makes a UI building draggable from Action Panel to Grid
/// </summary>
public class DraggableBuilding : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Building Data")]
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visual")]
    [SerializeField] private Canvas canvas;
    
    private GameObject draggedObject;
    private RectTransform draggedRectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        
        // Add CanvasGroup to control raycast blocking
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (buildingData == null)
        {
            Debug.LogWarning("[DraggableBuilding] No building data assigned!");
            return;
        }
        
        Debug.Log($"[DraggableBuilding] Started dragging {buildingData.buildingName}");
        
        // Create a dragged copy
        draggedObject = new GameObject("DraggedBuilding");
        draggedObject.transform.SetParent(canvas.transform, false);
        
        // Add image
        var image = draggedObject.AddComponent<UnityEngine.UI.Image>();
        image.sprite = buildingData.icon;
        image.raycastTarget = false;
        
        // Position and size
        draggedRectTransform = draggedObject.GetComponent<RectTransform>();
        draggedRectTransform.sizeDelta = new Vector2(150, 150);
        draggedRectTransform.position = eventData.position;
        
        // Make original semi-transparent while dragging
        canvasGroup.alpha = 0.5f;
        
        startPosition = transform.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            draggedRectTransform.position = eventData.position;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("[DraggableBuilding] Ended drag");
        
        // Restore original transparency
        canvasGroup.alpha = 1f;
        
        if (draggedObject == null) return;
        
        // Get world position
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int gridPos = GameManager.Instance.Grid.GetGridPosition(worldPos);
        
        Debug.Log($"[DraggableBuilding] Dropped at grid {gridPos}");
        
        // IMPORTANT: Check for MERGE first, BEFORE checking CanPlaceBuilding
        // This allows merging even on occupied cells
        Building existingBuilding = GameManager.Instance.Grid.GetBuildingAtPosition(gridPos);
        
        if (existingBuilding != null)
        {
            Debug.Log($"[DraggableBuilding] Found existing building: {existingBuilding.Data.buildingName} L{existingBuilding.CurrentLevel}");
            
            // Try to merge with existing building
            PlaceBuilding(gridPos); // This will handle merge logic
            
            // Destroy UI drag object
            Destroy(draggedObject);
            draggedObject = null;
            return;
        }
        
        // No existing building - check if we can place normally
        bool canPlace = GameManager.Instance.Grid.CanPlaceBuilding(gridPos, buildingData.size);
        
        if (canPlace)
        {
            // Place the building!
            PlaceBuilding(gridPos);
        }
        else
        {
            Debug.Log("[DraggableBuilding] Cannot place building here");
        }
        
        // Destroy dragged object
        Destroy(draggedObject);
        draggedObject = null;
    }    
    private void PlaceBuilding(Vector2Int gridPosition)
    {
        Debug.Log($"[DraggableBuilding] Attempting to place {buildingData.buildingName} at {gridPosition}");
        
        // Check if there's already a building at this position
        Building existingBuilding = GameManager.Instance.Grid.GetBuildingAtPosition(gridPosition);
        
        if (existingBuilding != null)
        {
            Debug.Log($"[DraggableBuilding] Found existing building: {existingBuilding.Data.buildingName} L{existingBuilding.CurrentLevel}");
            
            // Check if we can merge
            bool sameType = buildingData.buildingID == existingBuilding.Data.buildingID;
            bool sameLevel = existingBuilding.CurrentLevel == 1; // Panel buildings are always L1, target must also be L1
            bool notMaxLevel = existingBuilding.CurrentLevel < 4;
            
            Debug.Log($"[DraggableBuilding] Merge check: SameType={sameType}, SameLevel={sameLevel}, NotMax={notMaxLevel}");
            
            bool canMerge = sameType && sameLevel && notMaxLevel;
            
            if (canMerge)
            {
                Debug.Log($"[DraggableBuilding] 🎉 MERGING panel building into grid building!");
                
                // Upgrade the existing building
                existingBuilding.UpgradeLevel(existingBuilding.CurrentLevel + 1);
                
                // Update visual (if method exists)
                if (existingBuilding.GetComponent<Building>() != null)
                {
                    SpriteRenderer renderer = existingBuilding.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        float factor = 1f - (existingBuilding.CurrentLevel * 0.15f);
                        renderer.color = new Color(factor, factor, 1f);
                        Debug.Log($"[DraggableBuilding] Updated color for level {existingBuilding.CurrentLevel}");
                    }
                }
                
                // Reset UI slot for next placement
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
                
                Debug.Log($"[DraggableBuilding] ✅ Merge complete! Building now level {existingBuilding.CurrentLevel}");
                
                // Trigger merge event
                EventSystem.Instance.Trigger(new BuildingMergedEvent(existingBuilding, existingBuilding.CurrentLevel));
                
                return; // Important: Don't create new building
            }
            else
            {
                Debug.Log($"[DraggableBuilding] ❌ Can't merge - different building type or level mismatch");
                
                // Cell is occupied, can't place
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
                return;
            }
        }
        
        // No building at target - place new building
        Debug.Log($"[DraggableBuilding] No existing building, creating new one");
        
        Building building = GameManager.Instance.Buildings.CreateBuilding(buildingData, gridPosition, 1);
        
        if (building != null)
        {
            GameManager.Instance.Grid.PlaceBuilding(building, gridPosition);
            
            Debug.Log($"[DraggableBuilding] ✅ Successfully placed {buildingData.buildingName}!");
            
            // Reset for unlimited placement (testing mode)
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}