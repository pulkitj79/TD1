using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected BuildingData data;
    
    public BuildingData Data => data;
    public int CurrentLevel { get; protected set; } = 1;
    public Vector2Int GridPosition { get; protected set; }
    
    public virtual void Initialize(BuildingData buildingData, Vector2Int gridPos, int level = 1)
    {
        data = buildingData;
        GridPosition = gridPos;
        CurrentLevel = level;
        
        Debug.Log($"[Building] {data.buildingName} initialized at {gridPos}");
    }
    
    public virtual void Activate()
    {
        enabled = true;
    }
    
    public virtual void Deactivate()
    {
        enabled = false;
    }
    
    public virtual void UpgradeLevel(int newLevel)
    {
        CurrentLevel = Mathf.Clamp(newLevel, 1, 4);
    }
}