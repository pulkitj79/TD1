using UnityEngine;

/// <summary>
/// Base class for all unit data (Soldiers and Enemies)
/// ScriptableObject for data-driven design
/// </summary>
[CreateAssetMenu(fileName = "New Unit", menuName = "Game Data/Unit")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string unitName = "Soldier";
    public string unitID = "soldier_basic"; // Unique identifier
    public Sprite icon;
    public GameObject prefab;
    
    [Header("Stats")]
    [Tooltip("Base health points")]
    public int baseHP = 100;
    
    [Tooltip("Base damage per attack")]
    public int baseDamage = 10;
    
    [Tooltip("Time between attacks in seconds")]
    public float attackSpeed = 1.5f;
    
    [Tooltip("Movement speed in units/second")]
    public float moveSpeed = 3f;
    
    [Tooltip("Attack range in units")]
    public float attackRange = 1.5f;
    
    [Header("Leveling")]
    [Tooltip("Stat bonuses per level")]
    public StatBonuses levelUpBonuses;
    
    [Header("Rewards (Enemies only)")]
    public int goldReward = 5;
    public int silverReward = 2;
    public int xpReward = 10;
    
    [Header("Visual")]
    public RuntimeAnimatorController animatorController;
    
    /// <summary>
    /// Calculate stats for a specific level
    /// </summary>
    public UnitStats GetStatsForLevel(int level)
    {
        return new UnitStats
        {
            hp = baseHP + (levelUpBonuses.hpPerLevel * (level - 1)),
            damage = baseDamage + (levelUpBonuses.damagePerLevel * (level - 1)),
            attackSpeed = attackSpeed,
            moveSpeed = moveSpeed,
            attackRange = attackRange
        };
    }
}

[System.Serializable]
public struct StatBonuses
{
    public int hpPerLevel;
    public int damagePerLevel;
}

[System.Serializable]
public struct UnitStats
{
    public int hp;
    public int damage;
    public float attackSpeed;
    public float moveSpeed;
    public float attackRange;
}

/// <summary>
/// Building data ScriptableObject
/// Defines stats for Barracks, Towers, and Support buildings
/// </summary>
[CreateAssetMenu(fileName = "New Building", menuName = "Game Data/Building")]
public class BuildingData : ScriptableObject
{
    [Header("Identity")]
    public string buildingName = "Barracks";
    public string buildingID = "barracks_basic";
    public BuildingType buildingType;
    public Sprite icon;
    public GameObject prefab;
    
    [Header("Grid Placement")]
    [Tooltip("Size in grid cells (width x height)")]
    public Vector2Int size = new Vector2Int(1, 1);
    
    [Header("Barracks Settings")]
    [Tooltip("What unit this barracks spawns")]
    public UnitData spawnedUnit;
    
    [Tooltip("Maximum units on field simultaneously")]
    public int maxActiveUnits = 3;
    
    [Tooltip("Time to spawn one unit in seconds")]
    public float spawnTime = 5f;
    
    [Header("Tower Settings")]
    [Tooltip("Damage per projectile")]
    public int towerDamage = 15;
    
    [Tooltip("Attack range in units")]
    public float towerRange = 5f;
    
    [Tooltip("Time between shots")]
    public float towerAttackSpeed = 2f;
    
    [Tooltip("Projectile prefab")]
    public GameObject projectilePrefab;
    
    [Header("Support Settings")]
    [Tooltip("Buff radius in grid cells")]
    public int supportRadius = 2;
    
    [Tooltip("Buffs provided to units/buildings in range")]
    public BuffData[] providedBuffs;
    
    [Header("Level Progression")]
    public BuildingLevelData[] levelData = new BuildingLevelData[4];
    
    /// <summary>
    /// Get stats for a specific level (1-4)
    /// </summary>
    public BuildingLevelData GetLevelData(int level)
    {
        level = Mathf.Clamp(level, 1, 4);
        return levelData[level - 1];
    }
}

public enum BuildingType
{
    Barracks,  // Spawns soldiers
    Tower,     // Attacks enemies from range
    Support    // Provides buffs
}

[System.Serializable]
public struct BuildingLevelData
{
    [Tooltip("Visual tier/model to use")]
    public GameObject prefabOverride;
    
    [Header("Stat Multipliers")]
    public float damageMultiplier;      // For towers
    public float spawnSpeedMultiplier;  // For barracks (lower = faster)
    public int capacityBonus;           // For barracks
    public float rangeMultiplier;       // For towers and support
}

/// <summary>
/// Buff data - temporary or permanent stat modifiers
/// </summary>
[CreateAssetMenu(fileName = "New Buff", menuName = "Game Data/Buff")]
public class BuffData : ScriptableObject
{
    [Header("Identity")]
    public string buffName = "Damage Boost";
    public string buffID = "buff_damage";
    public Sprite icon;
    public string description = "+10% Damage";
    
    [Header("Effect")]
    public BuffType buffType;
    
    [Tooltip("Percentage modifier (e.g., 0.1 = +10%)")]
    public float modifier = 0.1f;
    
    [Tooltip("Flat bonus (alternative to percentage)")]
    public int flatBonus = 0;
    
    [Header("Duration")]
    public bool isPermanent = false;
    public float duration = 10f; // Seconds (if not permanent)
    
    [Header("Stacking")]
    public bool canStack = false;
    public int maxStacks = 1;
}

public enum BuffType
{
    DamagePercent,
    DamageFlat,
    AttackSpeedPercent,
    HPPercent,
    HPFlat,
    MoveSpeedPercent,
    RangePercent,
    GoldBonus,
    XPBonus
}

/// <summary>
/// Wave data - defines enemy composition and timing
/// </summary>
[CreateAssetMenu(fileName = "New Wave", menuName = "Game Data/Wave")]
public class WaveData : ScriptableObject
{
    [Header("Wave Info")]
    public int waveNumber = 1;
    
    [Header("Enemy Spawns")]
    public EnemySpawnGroup[] spawnGroups;
    
    [Header("Rewards")]
    public int goldReward = 50;
    public int silverReward = 25;
    public int sacredEssenceReward = 10;
    
    [Header("Special")]
    public bool isBossWave = false;
    public UnitData bossUnit;
}

[System.Serializable]
public struct EnemySpawnGroup
{
    [Tooltip("What enemy to spawn")]
    public UnitData enemyType;
    
    [Tooltip("How many to spawn")]
    public int count;
    
    [Tooltip("Delay between each spawn in this group")]
    public float spawnInterval;
    
    [Tooltip("Delay before this group starts")]
    public float delayBeforeGroup;
}

/// <summary>
/// Static upgrade data for meta progression
/// </summary>
[CreateAssetMenu(fileName = "New Static Upgrade", menuName = "Game Data/Static Upgrade")]
public class StaticUpgradeData : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName = "Engineering";
    public string upgradeID = "upgrade_engineering";
    public Sprite icon;
    public string description = "Permanent +5% HP to the Wall per rank";
    
    [Header("Progression")]
    public int maxRank = 10;
    public int[] costs; // Cost in Sacred Essence for each rank
    
    [Header("Effect")]
    public UpgradeEffectType effectType;
    public float effectPerRank = 0.05f; // 5% per rank
}

public enum UpgradeEffectType
{
    WallHP,
    StartingGold,
    StartingSilver,
    SupportRadius,
    SoldierDamage,
    TowerDamage,
    SpawnSpeed
}

/// <summary>
/// Collection item data (for Soldier Codex, Relic Room)
/// </summary>
[CreateAssetMenu(fileName = "New Collection Item", menuName = "Game Data/Collection Item")]
public class CollectionItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName = "Heavy Infantry Shard";
    public string itemID = "shard_heavy_infantry";
    public Sprite icon;
    public string description = "Collect 10 to unlock permanent bonuses";
    
    [Header("Collection")]
    public CollectionType collectionType;
    public int shardsRequired = 10;
    
    [Header("Unlocked Bonus")]
    public string bonusDescription = "+5% HP to all Heavy Infantry";
    public BuffData unlockedBuff;
}

public enum CollectionType
{
    SoldierCodex,
    RelicRoom
}
