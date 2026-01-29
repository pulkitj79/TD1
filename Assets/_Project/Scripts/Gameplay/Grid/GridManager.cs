using UnityEngine;

public class GridManager : MonoBehaviour
{
    public void Initialize()
    {
        Debug.Log("[GridManager] Initialized");
    }
    
    public void SetupGrid(GridConfig config)
    {
        Debug.Log("[GridManager] Grid setup (not yet implemented)");
    }
    
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x, y);
    }
}