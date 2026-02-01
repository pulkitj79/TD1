using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected BuildingData data;
    
    public BuildingData Data => data;
    public int CurrentLevel { get; protected set; } = 1;
    public Vector2Int GridPosition { get; set; }
    
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
        UpdateLevelVisual();
    }
    

    /// <summary>
    /// Update building visual based on current level
    /// </summary>
    public void UpdateLevelVisual()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Calculate color based on level
            float colorFactor = 1f - (CurrentLevel * 0.15f);
            Color levelColor = new Color(colorFactor, colorFactor, 1f);
            
            renderer.color = levelColor;
            
            Debug.Log($"[Building] Updated visual for level {CurrentLevel}, color factor: {colorFactor}");
        }
    }

    /// <summary>
    /// Override UpgradeLevel to include visual update
    /// </summary>
    /*public override void UpgradeLevel(int newLevel)
    {
        base.UpgradeLevel(newLevel);
        UpdateLevelVisual();
    }
    */

}