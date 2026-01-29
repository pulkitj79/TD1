using UnityEngine;

/// <summary>
/// Manages all in-game currencies:
/// - Gold/Silver: Wave-to-wave currency (resets each chapter)
/// - Sacred Essence: Meta currency (permanent across runs)
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    [Header("Current Session Currency")]
    [SerializeField] private int currentGold = 0;
    [SerializeField] private int currentSilver = 0;
    
    [Header("Meta Currency (Persistent)")]
    [SerializeField] private int sacredEssence = 0;
    
    // Properties
    public int Gold => currentGold;
    public int Silver => currentSilver;
    public int SacredEssence => sacredEssence;

    public void Initialize()
    {
        // Load Sacred Essence from save data
        LoadSacredEssence();
        Debug.Log("[CurrencyManager] Initialized");
    }

    // ============================================
    // GOLD MANAGEMENT
    // ============================================
    
    /// <summary>
    /// Add gold and trigger event
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        
        int previousAmount = currentGold;
        currentGold += amount;
        
        EventSystem.Instance.Trigger(new GoldChangedEvent(currentGold, amount));
        Debug.Log($"[Currency] Gold +{amount} (Total: {currentGold})");
    }

    /// <summary>
    /// Remove gold if enough available
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[Currency] Cannot spend zero or negative gold");
            return false;
        }
        
        if (currentGold < amount)
        {
            Debug.LogWarning($"[Currency] Not enough gold! Have: {currentGold}, Need: {amount}");
            return false;
        }
        
        currentGold -= amount;
        EventSystem.Instance.Trigger(new GoldChangedEvent(currentGold, -amount));
        Debug.Log($"[Currency] Gold -{amount} (Total: {currentGold})");
        return true;
    }

    /// <summary>
    /// Check if player has enough gold
    /// </summary>
    public bool HasGold(int amount)
    {
        return currentGold >= amount;
    }

    /// <summary>
    /// Set gold to specific amount (used at chapter start)
    /// </summary>
    public void SetGold(int amount)
    {
        int delta = amount - currentGold;
        currentGold = amount;
        EventSystem.Instance.Trigger(new GoldChangedEvent(currentGold, delta));
        Debug.Log($"[Currency] Gold set to {currentGold}");
    }

    // ============================================
    // SILVER MANAGEMENT
    // ============================================
    
    /// <summary>
    /// Add silver and trigger event
    /// </summary>
    public void AddSilver(int amount)
    {
        if (amount <= 0) return;
        
        int previousAmount = currentSilver;
        currentSilver += amount;
        
        EventSystem.Instance.Trigger(new SilverChangedEvent(currentSilver, amount));
        Debug.Log($"[Currency] Silver +{amount} (Total: {currentSilver})");
    }

    /// <summary>
    /// Remove silver if enough available
    /// </summary>
    public bool SpendSilver(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[Currency] Cannot spend zero or negative silver");
            return false;
        }
        
        if (currentSilver < amount)
        {
            Debug.LogWarning($"[Currency] Not enough silver! Have: {currentSilver}, Need: {amount}");
            return false;
        }
        
        currentSilver -= amount;
        EventSystem.Instance.Trigger(new SilverChangedEvent(currentSilver, -amount));
        Debug.Log($"[Currency] Silver -{amount} (Total: {currentSilver})");
        return true;
    }

    /// <summary>
    /// Check if player has enough silver
    /// </summary>
    public bool HasSilver(int amount)
    {
        return currentSilver >= amount;
    }

    /// <summary>
    /// Set silver to specific amount (used at chapter start)
    /// </summary>
    public void SetSilver(int amount)
    {
        int delta = amount - currentSilver;
        currentSilver = amount;
        EventSystem.Instance.Trigger(new SilverChangedEvent(currentSilver, delta));
        Debug.Log($"[Currency] Silver set to {currentSilver}");
    }

    // ============================================
    // SACRED ESSENCE MANAGEMENT (Meta Currency)
    // ============================================
    
    /// <summary>
    /// Add Sacred Essence (permanent meta currency)
    /// </summary>
    public void AddSacredEssence(int amount)
    {
        if (amount <= 0) return;
        
        sacredEssence += amount;
        EventSystem.Instance.Trigger(new SacredEssenceChangedEvent(sacredEssence, amount));
        Debug.Log($"[Currency] Sacred Essence +{amount} (Total: {sacredEssence})");
        
        // Auto-save when meta currency changes
        SaveSacredEssence();
    }

    /// <summary>
    /// Spend Sacred Essence on meta upgrades
    /// </summary>
    public bool SpendSacredEssence(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[Currency] Cannot spend zero or negative essence");
            return false;
        }
        
        if (sacredEssence < amount)
        {
            Debug.LogWarning($"[Currency] Not enough Sacred Essence! Have: {sacredEssence}, Need: {amount}");
            return false;
        }
        
        sacredEssence -= amount;
        EventSystem.Instance.Trigger(new SacredEssenceChangedEvent(sacredEssence, -amount));
        Debug.Log($"[Currency] Sacred Essence -{amount} (Total: {sacredEssence})");
        
        // Auto-save when meta currency changes
        SaveSacredEssence();
        return true;
    }

    /// <summary>
    /// Check if player has enough Sacred Essence
    /// </summary>
    public bool HasSacredEssence(int amount)
    {
        return sacredEssence >= amount;
    }

    // ============================================
    // WAVE REWARDS
    // ============================================
    
    /// <summary>
    /// Award all currencies at once (typically at wave end)
    /// </summary>
    public void AwardWaveRewards(int gold, int silver, int essence)
    {
        if (gold > 0) AddGold(gold);
        if (silver > 0) AddSilver(silver);
        if (essence > 0) AddSacredEssence(essence);
        
        Debug.Log($"[Currency] Wave rewards: {gold}G, {silver}S, {essence}E");
    }

    // ============================================
    // SAVE/LOAD
    // ============================================
    
    private void LoadSacredEssence()
    {
        sacredEssence = PlayerPrefs.GetInt("SacredEssence", 0);
        Debug.Log($"[Currency] Loaded Sacred Essence: {sacredEssence}");
    }

    private void SaveSacredEssence()
    {
        PlayerPrefs.SetInt("SacredEssence", sacredEssence);
        PlayerPrefs.Save();
    }

    // ============================================
    // UTILITY
    // ============================================
    
    /// <summary>
    /// Reset session currencies (called at chapter start)
    /// Sacred Essence is preserved
    /// </summary>
    public void ResetSessionCurrencies()
    {
        SetGold(0);
        SetSilver(0);
        Debug.Log("[Currency] Session currencies reset");
    }

    /// <summary>
    /// Debug method to add currencies for testing
    /// </summary>
    [ContextMenu("Debug: Add 100 Gold")]
    private void DebugAddGold()
    {
        AddGold(100);
    }

    [ContextMenu("Debug: Add 50 Silver")]
    private void DebugAddSilver()
    {
        AddSilver(50);
    }

    [ContextMenu("Debug: Add 1000 Sacred Essence")]
    private void DebugAddEssence()
    {
        AddSacredEssence(1000);
    }
}
