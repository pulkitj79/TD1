using UnityEngine;

/// <summary>
/// Barracks building - spawns soldiers
/// </summary>
public class Barracks : Building
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    
    private int activeUnits = 0;
    private float spawnTimer = 0f;
   /* 
    protected override void ApplyLevelStats()
    {
        // TODO: Apply level-specific stats
        Debug.Log($"[Barracks] Applied level {CurrentLevel} stats");
    }
    */    
    public override void Activate()
    {
        base.Activate();
        Debug.Log($"[Barracks] Activated at {GridPosition}");
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
        Debug.Log($"[Barracks] Deactivated");
    }
    
    private void Update()
    {
        if (!enabled) return;
        
        // TODO: Implement soldier spawning
        // For now, just log that we're active
    }
}