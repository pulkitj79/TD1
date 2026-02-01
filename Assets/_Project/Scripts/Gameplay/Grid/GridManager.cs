using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the grid for portrait mode with smart cell generation and movement
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private int columns = 5;
    [SerializeField] private int rows = 8;
    [SerializeField] private float cellSize = 0.55f;
    [SerializeField] private float cellGap = 0.05f;
    [SerializeField] private Vector2 gridOrigin = new Vector2(-2.25f, -4.4f);
    
    
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    
    [Header("Visual")]
    [SerializeField] private Transform gridContainer;
    
    private GridCell[,] grid;
    private List<GridCell> availableCells = new List<GridCell>();
    
    
    public void Initialize()
    {
        Debug.Log("[GridManager] Initialized");
    }
    
    /// <summary>
    /// Get the grid container transform (for parenting buildings)
    /// </summary>
    public Transform GetGridContainer()
    {
        return gridContainer;
    }

    public void SetupGrid(GridConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("[GridManager] No GridConfig provided, using defaults");
            CreateGrid(18);
            return;
        }
        
        columns = config.gridSize.x;
        rows = config.gridSize.y;
        
        // Fixed range: 16-20 cells
        int targetCells = 18; // Default middle value
        
        CreateGrid(targetCells);
        
        Debug.Log($"[GridManager] Grid created: {columns}x{rows}, {targetCells} cells target");
    }
    
    private void CreateGrid(int targetVisibleCells)
    {
        if (gridContainer == null)
        {
            GameObject containerObj = new GameObject("Grid");
            containerObj.transform.SetParent(transform);
            containerObj.transform.position = new Vector3(0, normalYPosition, 0);
            gridContainer = containerObj.transform;
        }
        
        if (cellPrefab == null)
        {
            Debug.LogError("[GridManager] GridCell prefab not assigned!");
            return;
        }
        
        grid = new GridCell[columns, rows];
        
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, gridContainer);
                cellObj.name = $"Cell_{x}_{y}";
                
                GridCell cell = cellObj.GetComponent<GridCell>();
                cell.Initialize(new Vector2Int(x, y), false, cellSize);
                grid[x, y] = cell;
            }
        }
        
        MakeRandomCellsVisible(targetVisibleCells);
        
        Debug.Log($"[GridManager] Created {columns * rows} total cells, {availableCells.Count} visible");
    }
    
    private void MakeRandomCellsVisible(int targetCount)
    {
        availableCells.Clear();
        
        if (grid == null) return;
        
        targetCount = Mathf.Clamp(targetCount, 16, 20);
        
        int maxRowFromTop = 2; // Top 3 rows only
        
        int startX = Random.Range(1, columns - 1);
        int startY = rows - 1 - Random.Range(0, 3);
        
        Debug.Log($"[GridManager] Starting block at ({startX}, {startY}) - targeting {targetCount} cells");
        
        List<Vector2Int> toProcess = new List<Vector2Int>();
        HashSet<Vector2Int> processed = new HashSet<Vector2Int>();
        
        toProcess.Add(new Vector2Int(startX, startY));
        
        int attempts = 0;
        
        while (toProcess.Count > 0 && availableCells.Count < targetCount && attempts < 1000)
        {
            attempts++;
            
            int index = Random.Range(0, toProcess.Count);
            Vector2Int current = toProcess[index];
            toProcess.RemoveAt(index);
            
            if (processed.Contains(current)) continue;
            processed.Add(current);
            
            if (current.x < 0 || current.x >= columns || current.y < 0 || current.y >= rows)
                continue;
            
            int rowFromTop = rows - 1 - current.y;
            if (rowFromTop > maxRowFromTop)
                continue;
            
            GridCell cell = grid[current.x, current.y];
            cell.SetVisible(true);
            availableCells.Add(cell);
            
            Vector2Int[] neighbors = GetNeighbors(current);
            
            float progress = (float)availableCells.Count / targetCount;
            float expansionChance = 1.0f - (progress * 0.5f);
            
            foreach (var neighbor in neighbors)
            {
                if (!processed.Contains(neighbor) && Random.value < expansionChance)
                {
                    toProcess.Add(neighbor);
                }
            }
        }
        
        Debug.Log($"[GridManager] Created block with {availableCells.Count} cells in top 3 rows");
    }
    
    private Vector2Int[] GetNeighbors(Vector2Int pos)
    {
        return new Vector2Int[]
        {
            new Vector2Int(pos.x + 1, pos.y),
            new Vector2Int(pos.x - 1, pos.y),
            new Vector2Int(pos.x, pos.y + 1),
            new Vector2Int(pos.x, pos.y - 1),
        };
    }
    
    /// <summary>
    /// Get world position for a grid coordinate
    /// </summary>
    public Vector2 GetWorldPosition(int x, int y)
    {
        if (gridContainer == null)
        {
            Debug.LogError("[GridManager] Grid container is null!");
            return Vector2.zero;
        }
        
        float effectiveCellSize = cellSize + cellGap;
        
        // Calculate grid dimensions
        float gridWidth = 8 * effectiveCellSize;
        float gridHeight = 5 * effectiveCellSize;
        
        // Calculate offset to center grid
        float offsetX = -gridWidth / 2f + effectiveCellSize / 2f;
        float offsetY = -gridHeight / 2f + effectiveCellSize / 2f;
        
        // Calculate local position (relative to grid container center)
        Vector2 localPos = new Vector2(
            offsetX + (x * effectiveCellSize),
            offsetY + (y * effectiveCellSize)
        );
        
        // Convert to world position
        Vector3 worldPos = gridContainer.TransformPoint(localPos);
        
        return worldPos;
    }

    /// <summary>
    /// Get grid position from world position (CRITICAL FIX)
    /// </summary>
    public Vector2Int GetGridPosition(Vector2 worldPos)
    {
        if (gridContainer == null)
        {
            Debug.LogError("[GridManager] Grid container is null!");
            return Vector2Int.zero;
        }
        
        // Convert world position to local position (relative to grid container)
        Vector3 localPos = gridContainer.InverseTransformPoint(worldPos);
        
        float effectiveCellSize = cellSize + cellGap;
        
        // Calculate grid dimensions
        float gridWidth = 8 * effectiveCellSize;
        float gridHeight = 5 * effectiveCellSize;
        
        // Calculate offset
        float offsetX = -gridWidth / 2f + effectiveCellSize / 2f;
        float offsetY = -gridHeight / 2f + effectiveCellSize / 2f;
        
        // Calculate grid coordinates
        int x = Mathf.RoundToInt((localPos.x - offsetX) / effectiveCellSize);
        int y = Mathf.RoundToInt((localPos.y - offsetY) / effectiveCellSize);
        
        // Clamp to grid bounds
        x = Mathf.Clamp(x, 0, 7); // 8 columns (0-7)
        y = Mathf.Clamp(y, 0, 4); // 5 rows (0-4)
        
        Debug.Log($"[GridManager] World {worldPos} -> Local {localPos} -> Grid ({x},{y})");
        
        return new Vector2Int(x, y);
    }
    
    public bool CanPlaceBuilding(Vector2Int position, Vector2Int buildingSize)
    {
        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int checkPos = position + new Vector2Int(x, y);
                if (!IsCellAvailable(checkPos))
                    return false;
            }
        }
        return true;
    }
    
    private bool IsCellAvailable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= columns || pos.y < 0 || pos.y >= rows)
            return false;
        
        GridCell cell = grid[pos.x, pos.y];
        return cell.IsVisible && !cell.IsOccupied;
    }
    
    public void PlaceBuilding(Building building, Vector2Int position)
    {
        Vector2Int size = building.Data.size;
        
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPos = position + new Vector2Int(x, y);
                
                // Bounds check
                if (cellPos.x >= 0 && cellPos.x < 8 && cellPos.y >= 0 && cellPos.y < 5)
                {
                    GridCell cell = grid[cellPos.x, cellPos.y];
                    cell.SetOccupied(building);
                    //cell.Hide(); // Hide cell under building
                }
            }
        }
        
        // IMPORTANT: Don't reset building level here!
        // The building's CurrentLevel should be preserved
        
        Debug.Log($"[GridManager] Placed {building.Data.buildingName} L{building.CurrentLevel} at {position}");
    }
    
    public bool PurchaseCellBlock(Vector2Int position, int cost)
    {
        if (!GameManager.Instance.Currency.HasGold(cost))
        {
            Debug.LogWarning("[GridManager] Not enough gold");
            return false;
        }
        
        Vector2Int[] blockCells = new Vector2Int[]
        {
            position,
            new Vector2Int(position.x + 1, position.y)
        };
        
        foreach (var cellPos in blockCells)
        {
            if (cellPos.x < 0 || cellPos.x >= columns || cellPos.y < 0 || cellPos.y >= rows)
                return false;
            
            GridCell cell = grid[cellPos.x, cellPos.y];
            if (cell.IsVisible)
                return false;
        }
        
        if (GameManager.Instance.Currency.SpendGold(cost))
        {
            foreach (var cellPos in blockCells)
            {
                GridCell cell = grid[cellPos.x, cellPos.y];
                cell.SetVisible(true);
                availableCells.Add(cell);
            }
            
            Debug.Log($"[GridManager] Purchased cell block at {position}");
            return true;
        }
        
        return false;
    }
    // ============================================
    // GRID MOVEMENT METHODS
    // ============================================

    [Header("Movement")]
    [SerializeField] private float normalYPosition = -4.4f;
    [SerializeField] private float raisedYPosition = -2.0f;
    [SerializeField] private float moveDuration = 0.3f;

    private bool isRaised = false;
    private Coroutine moveCoroutine;
    
    /// <summary>
    /// Move grid up (for Action Panel)
    /// </summary>
    public void RaiseGrid()
    {
        if (isRaised)
        {
            Debug.Log("[GridManager] Grid already raised");
            return;
        }
        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MoveGridTo(raisedYPosition));
        isRaised = true;
        
        Debug.Log("[GridManager] Raising grid");
    }
    
    /// <summary>
    /// Move grid down (normal position)
    /// </summary>
    public void LowerGrid()
    {
        if (!isRaised)
        {
            Debug.Log("[GridManager] Grid already lowered");
            return;
        }
        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MoveGridTo(normalYPosition));
        isRaised = false;
        
        Debug.Log("[GridManager] Lowering grid");
    }
    
    /// <summary>
    /// Smooth movement coroutine
    /// </summary>
    private System.Collections.IEnumerator MoveGridTo(float targetY)
    {
        if (gridContainer == null)
        {
            Debug.LogError("[GridManager] Grid container is null!");
            yield break;
        }
        
        Vector3 startPos = gridContainer.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);
        
        Debug.Log($"[GridManager] Moving from Y={startPos.y:F2} to Y={targetY:F2}");
        
        float elapsed = 0f;
        
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            
            // Ease-out cubic
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            gridContainer.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        gridContainer.position = targetPos;
        
        Debug.Log($"[GridManager] Movement complete. Y={gridContainer.position.y:F2}");
    }

    /// <summary>
    /// Remove building from grid (frees up cells)
    /// </summary>
    public void RemoveBuildingFromGrid(Building building, Vector2Int position)
    {
        Vector2Int size = building.Data.size;
        
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPos = position + new Vector2Int(x, y);
                if (cellPos.x >= 0 && cellPos.x < 8 && cellPos.y >= 0 && cellPos.y < 5)
                {
                    GridCell cell = grid[cellPos.x, cellPos.y];
                    cell.ClearOccupancy();
                    cell.Show(); // Show cell again when building removed
                }
            }
        }
        
        Debug.Log($"[GridManager] Freed cells at {position}");
    }

    /// <summary>
    /// Get building at a specific grid position
    /// </summary>
    public Building GetBuildingAtPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= 8 || position.y < 0 || position.y >= 5)
            return null;
        
        GridCell cell = grid[position.x, position.y];
        return cell.OccupyingBuilding;
    }        
    // TEST METHODS - Can be called from Inspector or buttons
    [ContextMenu("Test: Raise Grid")]
    public void TestRaiseGrid()
    {
        RaiseGrid();
    }
    
    [ContextMenu("Test: Lower Grid")]
    public void TestLowerGrid()
    {
        LowerGrid();
    }


}