using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages meta progression (Static Upgrades, Tech Tree, Collections)
/// </summary>
public class MetaProgressionManager : MonoBehaviour
{
    [Header("Static Upgrades")]
    private Dictionary<string, int> upgradeRanks = new Dictionary<string, int>();
    
    public void Initialize()
    {
        LoadProgress();
        Debug.Log("[MetaProgressionManager] Initialized");
    }
    
    public void ApplyStaticUpgrades()
    {
        Debug.Log("[MetaProgression] Applying static upgrades to run");
    }
    
    public int GetLogisticsBonus()
    {
        int rank = GetUpgradeRank("logistics");
        return rank * 10;
    }
    
    public int GetUpgradeRank(string upgradeID)
    {
        return upgradeRanks.ContainsKey(upgradeID) ? upgradeRanks[upgradeID] : 0;
    }
    
    public bool PurchaseUpgrade(StaticUpgradeData upgrade)
    {
        int currentRank = GetUpgradeRank(upgrade.upgradeID);
        
        if (currentRank >= upgrade.maxRank)
        {
            Debug.LogWarning($"[MetaProgression] {upgrade.upgradeName} already max rank");
            return false;
        }
        
        int cost = upgrade.costs[currentRank];
        
        if (GameManager.Instance.Currency.SpendSacredEssence(cost))
        {
            upgradeRanks[upgrade.upgradeID] = currentRank + 1;
            SaveProgress();
            return true;
        }
        
        return false;
    }
    
    private void LoadProgress()
    {
        string json = PlayerPrefs.GetString("MetaProgress", "{}");
    }
    
    private void SaveProgress()
    {
        PlayerPrefs.Save();
    }
}