using UnityEngine;

/// <summary>
/// Grid configuration for a chapter
/// </summary>
[System.Serializable]
public class GridConfig
{
    [Range(0.3f, 0.7f)]
    [Tooltip("Percentage of cells visible at start (0.4 = 40%)")]
    public float initialVisiblePercentage = 0.45f; // 40-50% cells visible at start
    
    [Tooltip("Grid dimensions (columns x rows)")]
    public Vector2Int gridSize = new Vector2Int(10, 5); // 10 columns x 5 rows
    
    [Tooltip("Cost in gold to buy a 2-cell block")]
    public int cellBlockCost = 20;
}