using UnityEngine;

/// <summary>
/// Represents a single cell in the grid
/// Handles visual state and occupancy
/// </summary>
public class GridCell : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
    [SerializeField] private Color occupiedColor = new Color(0.8f, 0.3f, 0.3f, 1f); // Red
    [SerializeField] private Color hoverColor = new Color(0.3f, 0.8f, 0.3f, 1f); // Green
    
    // Properties
    public Vector2Int GridPosition { get; private set; }
    public bool IsVisible { get; private set; }
    public bool IsOccupied { get; private set; }
    public Building OccupyingBuilding { get; private set; }
    
    /// <summary>
    /// Initialize this grid cell
    /// </summary>
    public void Initialize(Vector2Int gridPosition, bool visible, float visualSize = 0.55f)
    {
        GridPosition = gridPosition;
        IsVisible = visible;
        IsOccupied = false;
        
        // Get or add SpriteRenderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }
        
        // Set sprite if not assigned
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = SpriteHelper.GetWhiteSquare();
        }
        
        // Set visual size for gaps
        SetVisualSize(visualSize);
        
        // Set sorting order to render behind buildings
        spriteRenderer.sortingOrder = -1;
        
        UpdateVisual();
    }  
    
    /// <summary>
    /// Make this cell visible/available
    /// </summary>
    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        UpdateVisual();
    }
    
    /// <summary>
    /// Mark this cell as occupied by a building
    /// </summary>
    public void SetOccupied(Building building)
    {
        IsOccupied = true;
        OccupyingBuilding = building;
        UpdateVisual();
    }
    
    /// <summary>
    /// Clear occupancy
    /// </summary>
    public void ClearOccupancy()
    {
        IsOccupied = false;
        OccupyingBuilding = null;
        UpdateVisual();
    }
    
    /// <summary>
    /// Show placement preview (valid or invalid)
    /// </summary>
    public void ShowPlacementPreview(bool valid)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = valid ? hoverColor : occupiedColor;
        }
    }
    
    /// <summary>
    /// Update visual based on current state
    /// </summary>
    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;
        
        if (!IsVisible)
        {
            // Hidden cells are completely invisible
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            
            if (IsOccupied)
            {
                spriteRenderer.color = occupiedColor;
            }
            else
            {
                spriteRenderer.color = availableColor;
            }
        }
    }

    /// <summary>
    /// Set the visual size of this cell (smaller than cell size for gaps)
    /// </summary>
    public void SetVisualSize(float size)
    {
        if (spriteRenderer != null)
        {
            transform.localScale = new Vector3(size, size, 1f);
        }
    }
    
    /// <summary>
    /// Handle mouse hover (for future placement preview)
    /// </summary>
    private void OnMouseEnter()
    {
        // TODO: Show placement preview when dragging a building
    }
    
    private void OnMouseExit()
    {
        // TODO: Clear placement preview
        if (!IsOccupied && IsVisible)
        {
            UpdateVisual();
        }
    }
    /// <summary>
    /// Show cell again (after building removed)
    /// </summary>
    public void Show()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            UpdateVisual();
        }
    }
}