using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized event system for decoupled communication between game systems.
/// Uses a type-safe generic event pattern.
/// </summary>
public class EventSystem : MonoBehaviour
{
    private static EventSystem _instance;
    public static EventSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EventSystem");
                _instance = go.AddComponent<EventSystem>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Dictionary to store event handlers for each event type
    private Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();

    /// <summary>
    /// Subscribe to an event type
    /// </summary>
    public void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = Delegate.Combine(_eventHandlers[eventType], handler);
        }
        else
        {
            _eventHandlers[eventType] = handler;
        }
    }

    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = Delegate.Remove(_eventHandlers[eventType], handler);
            
            if (_eventHandlers[eventType] == null)
            {
                _eventHandlers.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// Trigger an event immediately
    /// </summary>
    public void Trigger<T>(T gameEvent) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            (_eventHandlers[eventType] as Action<T>)?.Invoke(gameEvent);
        }
    }

    /// <summary>
    /// Clear all event handlers (useful for scene transitions)
    /// </summary>
    public void ClearAllEvents()
    {
        _eventHandlers.Clear();
    }

    private void OnDestroy()
    {
        ClearAllEvents();
    }
}

/// <summary>
/// Base interface for all game events
/// </summary>
public interface IGameEvent { }

// ============================================
// GAME EVENT DEFINITIONS
// ============================================

#region Currency Events

public struct GoldChangedEvent : IGameEvent
{
    public int NewAmount;
    public int Delta;

    public GoldChangedEvent(int newAmount, int delta)
    {
        NewAmount = newAmount;
        Delta = delta;
    }
}

public struct SilverChangedEvent : IGameEvent
{
    public int NewAmount;
    public int Delta;

    public SilverChangedEvent(int newAmount, int delta)
    {
        NewAmount = newAmount;
        Delta = delta;
    }
}

public struct SacredEssenceChangedEvent : IGameEvent
{
    public int NewAmount;
    public int Delta;

    public SacredEssenceChangedEvent(int newAmount, int delta)
    {
        NewAmount = newAmount;
        Delta = delta;
    }
}

#endregion

#region Wave Events

public struct WaveStartedEvent : IGameEvent
{
    public int WaveNumber;

    public WaveStartedEvent(int waveNumber)
    {
        WaveNumber = waveNumber;
    }
}

public struct WaveCompletedEvent : IGameEvent
{
    public int WaveNumber;
    public int GoldEarned;
    public int SilverEarned;
    public int SacredEssenceEarned;

    public WaveCompletedEvent(int waveNumber, int gold, int silver, int essence)
    {
        WaveNumber = waveNumber;
        GoldEarned = gold;
        SilverEarned = silver;
        SacredEssenceEarned = essence;
    }
}

#endregion

#region Combat Events

public struct EnemySpawnedEvent : IGameEvent
{
    public Enemy Enemy;

    public EnemySpawnedEvent(Enemy enemy)
    {
        Enemy = enemy;
    }
}

public struct EnemyDeathEvent : IGameEvent
{
    public Enemy Enemy;
    public int GoldReward;
    public int SilverReward;
    public int XPReward;

    public EnemyDeathEvent(Enemy enemy, int gold, int silver, int xp)
    {
        Enemy = enemy;
        GoldReward = gold;
        SilverReward = silver;
        XPReward = xp;
    }
}

public struct SoldierSpawnedEvent : IGameEvent
{
    public Soldier Soldier;
    public Building SourceBuilding;

    public SoldierSpawnedEvent(Soldier soldier, Building source)
    {
        Soldier = soldier;
        SourceBuilding = source;
    }
}

public struct SoldierDeathEvent : IGameEvent
{
    public Soldier Soldier;

    public SoldierDeathEvent(Soldier soldier)
    {
        Soldier = soldier;
    }
}

public struct WallDamagedEvent : IGameEvent
{
    public int CurrentHP;
    public int MaxHP;
    public int DamageTaken;

    public WallDamagedEvent(int currentHP, int maxHP, int damage)
    {
        CurrentHP = currentHP;
        MaxHP = maxHP;
        DamageTaken = damage;
    }
}

#endregion

#region Building Events

public struct BuildingPlacedEvent : IGameEvent
{
    public Building Building;
    public Vector2Int GridPosition;

    public BuildingPlacedEvent(Building building, Vector2Int position)
    {
        Building = building;
        GridPosition = position;
    }
}

public struct BuildingRemovedEvent : IGameEvent
{
    public Building Building;
    public Vector2Int GridPosition;

    public BuildingRemovedEvent(Building building, Vector2Int position)
    {
        Building = building;
        GridPosition = position;
    }
}

#endregion

#region XP Events

public struct XPGainedEvent : IGameEvent
{
    public int Amount;
    public int NewTotal;

    public XPGainedEvent(int amount, int newTotal)
    {
        Amount = amount;
        NewTotal = newTotal;
    }
}

public struct LevelUpEvent : IGameEvent
{
    public int NewLevel;
    public BuffData[] AvailableBuffs; // 3 options to choose from

    public LevelUpEvent(int level, BuffData[] buffs)
    {
        NewLevel = level;
        AvailableBuffs = buffs;
    }
}

#endregion

#region State Events

public struct GameStateChangedEvent : IGameEvent
{
    public GameState PreviousState;
    public GameState NewState;

    public GameStateChangedEvent(GameState previous, GameState newState)
    {
        PreviousState = previous;
        NewState = newState;
    }
}

#endregion

// Placeholder classes - these will be defined in their own files
//public class Enemy { }
//public class Soldier { }
//public class Building { }
//public class BuffData { }
//public enum GameState { Preparation, Deployment, Battle, Resolution }
