using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitData Data { get; protected set; }
    public int CurrentHP { get; protected set; }
    public int MaxHP { get; protected set; }
    
    public virtual void Initialize(UnitData data)
    {
        Data = data;
        UnitStats stats = data.GetStatsForLevel(1);
        MaxHP = stats.hp;
        CurrentHP = MaxHP;
    }
    
    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}