using UnityEngine;

public class SimpleBuildingDragHandler : MonoBehaviour
{
    private Building building;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    
    private bool isDragging = false;
    private bool isDragAllowed = true; // NEW: Track if dragging is allowed
    private Vector3 originalPosition;
    private Vector2Int originalGridPosition;
    private Vector3 dragOffset;
    
    private void Start()
    {
        building = GetComponent<Building>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (building == null)
        {
            Debug.LogError($"[Drag] No Building component!");
            enabled = false;
            return;
        }
        
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(0.9f, 0.9f);
        }
        
        boxCollider.isTrigger = false;
        
        // Subscribe to game state changes
        EventSystem.Instance.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        
        Debug.Log($"[Drag] Ready on {building.Data.buildingName} L{building.CurrentLevel}");
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (EventSystem.Instance != null)
        {
            EventSystem.Instance.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }
    }
    
    /// <summary>
    /// Handle game state changes - enable/disable dragging
    /// </summary>
    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        // Only allow dragging in Preparation and Deployment states
        isDragAllowed = (evt.NewState == GameState.Preparation || 
                        evt.NewState == GameState.Deployment);
        
        if (!isDragAllowed && isDragging)
        {
            // If battle starts while dragging, cancel the drag
            CancelDrag();
        }
    }
    
    private void OnMouseDown()
    {
        // Check if dragging is allowed
        if (!isDragAllowed)
        {
            Debug.Log($"[Drag] ❌ Dragging not allowed in current game state");
            return; // Battle mode - no dragging!
        }
        
        Debug.Log($"[Drag] ⭐ Clicked {building.Data.buildingName} L{building.CurrentLevel} at {building.GridPosition}");
        
        originalPosition = transform.position;
        originalGridPosition = building.GridPosition;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragOffset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
        
        // Free grid cells
        GameManager.Instance.Grid.RemoveBuildingFromGrid(building, originalGridPosition);
        
        // Visual feedback
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            spriteRenderer.sortingOrder = 100;
        }
        
        isDragging = true;
    }
    
    private void OnMouseDrag()
    {
        if (!isDragging || !isDragAllowed) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(
            mousePos.x + dragOffset.x,
            mousePos.y + dragOffset.y,
            0f
        );
    }
    
    private void OnMouseUp()
    {
        if (!isDragging) return;
        
        Debug.Log($"[Drag] Released at {transform.position}");
        
        isDragging = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = Color.white;
        }
        
        // If drag is no longer allowed (state changed), return to original
        if (!isDragAllowed)
        {
            ReturnToOriginalPosition();
            return;
        }
        
        // Check if over Action Panel
        if (IsOverActionPanel())
        {
            Debug.Log($"[Drag] ✅ Over Action Panel");
            GameManager.Instance.Buildings.RemoveBuilding(building);
            Destroy(gameObject);
            return;
        }
        
        // Get grid position
        Vector2Int newGridPos = GameManager.Instance.Grid.GetGridPosition(transform.position);
        Debug.Log($"[Drag] Grid position: {newGridPos}");
        
        // Bounds check
        if (newGridPos.x < 0 || newGridPos.x >= 8 || newGridPos.y < 0 || newGridPos.y >= 5)
        {
            Debug.Log($"[Drag] ❌ Out of bounds");
            ReturnToOriginalPosition();
            return;
        }
        
        // Check for existing building
        Building targetBuilding = GameManager.Instance.Grid.GetBuildingAtPosition(newGridPos);
        
        if (targetBuilding != null && targetBuilding != building)
        {
            if (CanMergeWith(targetBuilding))
            {
                PerformMerge(targetBuilding);
                return;
            }
            else
            {
                Debug.Log($"[Drag] ❌ Can't merge");
                ReturnToOriginalPosition();
                return;
            }
        }
        
        // Try to place
        bool canPlace = GameManager.Instance.Grid.CanPlaceBuilding(newGridPos, building.Data.size);
        
        if (canPlace)
        {
            PlaceAtNewPosition(newGridPos);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }
    
    /// <summary>
    /// Cancel drag operation (called when state changes)
    /// </summary>
    private void CancelDrag()
    {
        isDragging = false;
        ReturnToOriginalPosition();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = Color.white;
        }
        
        Debug.Log($"[Drag] ⚠️ Drag cancelled due to state change");
    }
    
    private bool IsOverActionPanel()
    {
        ActionPanelController panel = FindObjectOfType<ActionPanelController>();
        if (panel == null) return false;
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null) return false;
        
        Vector2 localMousePos;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            Input.mousePosition,
            null,
            out localMousePos
        );
        
        if (!success) return false;
        
        return panelRect.rect.Contains(localMousePos);
    }
    
    private bool CanMergeWith(Building target)
    {
        if (target == null) return false;
        
        bool sameType = building.Data.buildingID == target.Data.buildingID;
        bool sameLevel = building.CurrentLevel == target.CurrentLevel;
        bool notMaxLevel = target.CurrentLevel < 4;
        
        return sameType && sameLevel && notMaxLevel;
    }
    
    private void PerformMerge(Building target)
    {
        int newLevel = target.CurrentLevel + 1;
        
        Debug.Log($"[Drag] 🎉 MERGING to level {newLevel}");
        
        // Upgrade target
        target.UpgradeLevel(newLevel);
        
        // Update visual
        target.UpdateLevelVisual();
        
        // IMPORTANT: Ensure target has drag handler
        SimpleBuildingDragHandler targetDragHandler = target.GetComponent<SimpleBuildingDragHandler>();
        if (targetDragHandler == null)
        {
            Debug.LogWarning($"[Drag] Target missing drag handler! Adding it...");
            target.gameObject.AddComponent<SimpleBuildingDragHandler>();
        }
        
        // Remove this building
        GameManager.Instance.Buildings.RemoveBuilding(building);
        Destroy(gameObject);
        
        EventSystem.Instance.Trigger(new BuildingMergedEvent(target, newLevel));
        
        Debug.Log($"[Drag] ✅ Merge complete!");
    }
    
    private void PlaceAtNewPosition(Vector2Int newPos)
    {
        building.GridPosition = newPos;
        
        Vector2 worldPos = GameManager.Instance.Grid.GetWorldPosition(newPos.x, newPos.y);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        
        GameManager.Instance.Grid.PlaceBuilding(building, newPos);
        
        // IMPORTANT: Restore level-based visual
        building.UpdateLevelVisual();

        Debug.Log($"[Drag] ✅ Placed at {newPos}");
    }
    
    private void ReturnToOriginalPosition()
    {
        transform.position = new Vector3(originalPosition.x, originalPosition.y, 0f);
        building.GridPosition = originalGridPosition;
        GameManager.Instance.Grid.PlaceBuilding(building, originalGridPosition);
        
        // Restore level-based visual
        building.UpdateLevelVisual();

        Debug.Log($"[Drag] ↩️ Returned to {originalGridPosition}");
    }
}