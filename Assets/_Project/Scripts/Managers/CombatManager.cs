using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages combat calculations and damage
/// </summary>
public class CombatManager : MonoBehaviour
{
    public void Initialize()
    {
        Debug.Log("[CombatManager] Initialized");
    }
    
    public int CalculateDamage(int baseDamage, List<BuffData> buffs)
    {
        float totalMultiplier = 1f;
        int totalFlat = 0;
        
        foreach (var buff in buffs)
        {
            if (buff.buffType == BuffType.DamagePercent)
                totalMultiplier += buff.modifier;
            else if (buff.buffType == BuffType.DamageFlat)
                totalFlat += buff.flatBonus;
        }
        
        return Mathf.RoundToInt(baseDamage * totalMultiplier) + totalFlat;
    }
}