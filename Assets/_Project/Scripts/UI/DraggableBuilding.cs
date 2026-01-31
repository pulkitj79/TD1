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
        
        // Check if we're over a valid grid cell
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int gridPos = GameManager.Instance.Grid.GetGridPosition(worldPos);
        
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
        Debug.Log($"[DraggableBuilding] Placing {buildingData.buildingName} at {gridPosition}");
        
        // Create building through BuildingManager
        Building building = GameManager.Instance.Buildings.CreateBuilding(buildingData, gridPosition, 1);
        
        if (building != null)
        {
            // Mark grid cells as occupied
            GameManager.Instance.Grid.PlaceBuilding(building, gridPosition);
            
            Debug.Log($"[DraggableBuilding] Successfully placed {buildingData.buildingName}!");
            
            // HIDE this slot after placing (one-time use)
            gameObject.SetActive(false);
        }
    }
}