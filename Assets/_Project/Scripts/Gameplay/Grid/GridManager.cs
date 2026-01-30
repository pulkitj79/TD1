using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the grid for portrait mode (9:16) with proper margins
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 6;
    [SerializeField] private float cellSize = 0.55f;
    [SerializeField] private float cellGap = 0.05f;
    [SerializeField] private Vector2 gridOrigin = new Vector2(-1.2f, -4.2f);
    
    [Header("Margins")]
    [SerializeField] private float leftMargin = 0.15f;
    [SerializeField] private float rightMargin = 0.15f;
    [SerializeField] private float topMargin = 0.1f;
    [SerializeField] private float bottomMargin = 0.1f;
    
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
    
    public void SetupGrid(GridConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("[GridManager] No GridConfig provided, using defaults");
            CreateGrid(0.5f);
            return;
        }
        
        columns = config.gridSize.x;
        rows = config.gridSize.y;
        
        CreateGrid(config.initialVisiblePercentage);
        
        Debug.Log($"[GridManager] Grid created: {columns}x{rows}, {config.initialVisiblePercentage * 100}% visible");
    }
    
    private void CreateGrid(float visiblePercentage)
    {
        if (gridContainer == null)
        {
            GameObject containerObj = new GameObject("Grid");
            containerObj.transform.SetParent(transform);
            containerObj.transform.position = Vector3.zero;
            gridContainer = containerObj.transform;
        }
        
        if (cellPrefab == null)
        {
            Debug.LogError("[GridManager] GridCell prefab not assigned!");
            return;
        }
        
        grid = new GridCell[columns, rows];
        int totalCells = columns * rows;
        int visibleCount = Mathf.RoundToInt(totalCells * visiblePercentage);
        
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
        
        MakeRandomCellsVisible(visibleCount);
        
        Debug.Log($"[GridManager] Created {totalCells} cells ({columns}x{rows}), {availableCells.Count} visible");
    }
    
    private void MakeRandomCellsVisible(int count)
    {
        availableCells.Clear();
        
        if (count <= 0 || grid == null) return;
        
        int startX = Random.Range(1, columns - 1);
        int startY = Random.Range(1, rows - 1);
        
        List<Vector2Int> toProcess = new List<Vector2Int>();
        HashSet<Vector2Int> processed = new HashSet<Vector2Int>();
        
        toProcess.Add(new Vector2Int(startX, startY));
        
        while (toProcess.Count > 0 && availableCells.Count < count)
        {
            int index = Random.Range(0, toProcess.Count);
            Vector2Int current = toProcess[index];
            toProcess.RemoveAt(index);
            
            if (processed.Contains(current)) continue;
            processed.Add(current);
            
            if (current.x < 0 || current.x >= columns || current.y < 0 || current.y >= rows)
                continue;
            
            GridCell cell = grid[current.x, current.y];
            cell.SetVisible(true);
            availableCells.Add(cell);
            
            Vector2Int[] neighbors = new Vector2Int[]
            {
                new Vector2Int(current.x + 1, current.y),
                new Vector2Int(current.x - 1, current.y),
                new Vector2Int(current.x, current.y + 1),
                new Vector2Int(current.x, current.y - 1),
            };
            
            foreach (var neighbor in neighbors)
            {
                if (!processed.Contains(neighbor) && Random.value > 0.2f)
                {
                    toProcess.Add(neighbor);
                }
            }
        }
        
        Debug.Log($"[GridManager] Created contiguous region with {availableCells.Count} cells");
    }
    
    public Vector2 GetWorldPosition(int x, int y)
    {
        float effectiveCellSize = cellSize + cellGap;
        return gridOrigin + new Vector2(
            leftMargin + (x * effectiveCellSize),
            bottomMargin + (y * effectiveCellSize)
        );
    }
    
    public Vector2Int GetGridPosition(Vector2 worldPos)
    {
        Vector2 localPos = worldPos - gridOrigin;
        localPos -= new Vector2(leftMargin, bottomMargin);
        
        float effectiveCellSize = cellSize + cellGap;
        int x = Mathf.RoundToInt(localPos.x / effectiveCellSize);
        int y = Mathf.RoundToInt(localPos.y / effectiveCellSize);
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
                grid[cellPos.x, cellPos.y].SetOccupied(building);
            }
        }
        
        Debug.Log($"[GridManager] Placed {building.Data.buildingName} at {position}");
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // Screen boundaries (camera size 4.8, aspect 9:16)
        float screenHeight = 9.6f;
        float screenWidth = 2.7f;
        
        // Draw screen boundary in cyan
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(-screenWidth / 2, -screenHeight / 2, 0);
        Vector3 topLeft = new Vector3(-screenWidth / 2, screenHeight / 2, 0);
        Vector3 topRight = new Vector3(screenWidth / 2, screenHeight / 2, 0);
        Vector3 bottomRight = new Vector3(screenWidth / 2, -screenHeight / 2, 0);
        
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        
        // Draw grid bounds in green
        if (grid != null && grid.Length > 0)
        {
            Gizmos.color = Color.green;
            Vector2 gridBL = GetWorldPosition(0, 0);
            Vector2 gridTR = GetWorldPosition(columns - 1, rows - 1);
            
            float effectiveCellSize = cellSize + cellGap;
            gridTR += new Vector2(effectiveCellSize, effectiveCellSize);
            
            Vector3 bl = new Vector3(gridBL.x - leftMargin, gridBL.y - bottomMargin, 0);
            Vector3 tl = new Vector3(gridBL.x - leftMargin, gridTR.y + topMargin, 0);
            Vector3 tr = new Vector3(gridTR.x + rightMargin, gridTR.y + topMargin, 0);
            Vector3 br = new Vector3(gridTR.x + rightMargin, gridBL.y - bottomMargin, 0);
            
            Gizmos.DrawLine(bl, tl);
            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);
        }
    }
}