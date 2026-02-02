using UnityEngine;

/// <summary>
/// Tower building - ranged attacks
/// </summary>
public class Tower : Building
{
    [Header("Tower Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackRate = 1f;
    
    public override void Activate()
    {
        base.Activate();
        Debug.Log($"[Tower] Activated at {GridPosition}");
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
        Debug.Log($"[Tower] Deactivated");
    }
}