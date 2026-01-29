# Tower Defense Game - Project Architecture

## Overview
This document outlines the architecture for a modular, data-driven tower defense game built in Unity. The design prioritizes maintainability, scalability, and clean separation of concerns.

## Core Design Principles

### 1. Data-Driven Design
- **ScriptableObjects** for all game data (units, buildings, waves, balancing)
- **JSON configs** for save data and progression
- Clear separation between data and logic

### 2. Modular Architecture
- Each system is self-contained with clear interfaces
- Systems communicate through events/messages
- Easy to test, modify, and extend

### 3. Design Patterns Used
- **MVC Pattern**: Separating data (Model), logic (Controller), and presentation (View)
- **Observer Pattern**: Event system for decoupled communication
- **Object Pooling**: For spawning units and projectiles efficiently
- **State Machine**: For game states and unit behaviors
- **Factory Pattern**: For creating units and buildings
- **Command Pattern**: For player actions and undo functionality

---

## Project Structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/                  # Core game systems
│   │   │   ├── GameManager.cs
│   │   │   ├── GameStateMachine.cs
│   │   │   └── EventSystem.cs
│   │   ├── Data/                  # ScriptableObject definitions
│   │   │   ├── BuildingData.cs
│   │   │   ├── UnitData.cs
│   │   │   ├── WaveData.cs
│   │   │   └── ChapterData.cs
│   │   ├── Gameplay/
│   │   │   ├── Grid/              # Grid system
│   │   │   ├── Buildings/         # Building logic
│   │   │   ├── Units/             # Unit/Enemy logic
│   │   │   ├── Combat/            # Combat system
│   │   │   └── Wave/              # Wave spawning
│   │   ├── UI/                    # All UI controllers
│   │   ├── Meta/                  # Progression systems
│   │   ├── Utilities/             # Helper classes
│   │   └── Managers/              # Individual managers
│   ├── Data/                      # ScriptableObject assets
│   │   ├── Buildings/
│   │   ├── Units/
│   │   ├── Waves/
│   │   └── Upgrades/
│   ├── Prefabs/
│   │   ├── Buildings/
│   │   ├── Units/
│   │   └── UI/
│   ├── Scenes/
│   └── Resources/
└── Plugins/                       # Third-party assets
```

---

## System Architecture

### 1. Core Systems Layer

#### GameManager (Singleton)
- Central coordination point
- Initializes all managers
- Handles scene transitions
- Maintains game state

#### EventSystem
- Centralized event bus
- Type-safe events
- Decouples systems
```csharp
// Example usage
EventSystem.OnWaveComplete += HandleWaveComplete;
EventSystem.Trigger(new EnemyDeathEvent(enemy, gold, silver));
```

#### GameStateMachine
- Manages four core states:
  - PreparationState (Action Panel phase)
  - DeploymentState (Grid placement phase)
  - BattleState (Active combat)
  - ResolutionState (Rewards)

---

### 2. Gameplay Systems

#### Grid System
**Key Classes:**
- `GridManager`: Manages the 5x10 grid
- `GridCell`: Individual cell logic
- `CellData`: ScriptableObject for cell properties

**Features:**
- Dynamic cell unlocking (40-50% start, expand with currency)
- Cell blocking for 2x1 purchased blocks
- Placement validation
- Visual feedback (valid/invalid placement)

#### Building System
**Key Classes:**
- `BuildingData` (ScriptableObject): Stats, prefab, cost
- `Building` (Base class): Common building logic
- `Barracks`: Spawns soldiers with queue system
- `Tower`: Attacks in range
- `Support`: Provides buffs in radius

**Spawn Queue Logic:**
```
Capacity = 3 soldiers max on field
When soldier dies → Start spawn timer
Queue additional deaths → Process sequentially
T_total = T_current + (N_queue × T_base_spawn)
```

#### Unit System
**Key Classes:**
- `UnitData` (ScriptableObject): Stats, prefab, behavior
- `Unit` (Base class): HP, damage, movement
- `Soldier`: Player units
- `Enemy`: Enemy units
- `Hero`: Special controlled unit

**Features:**
- Pathfinding to gates/enemies
- Automatic combat
- State machine (Idle, Moving, Attacking, Dead)

#### Combat System
**Key Classes:**
- `CombatManager`: Handles all combat calculations
- `Projectile`: Pooled projectile objects
- `DamageCalculator`: Damage formulas with buffs

**Damage Formula:**
```
FinalDamage = BaseDamage × (1 + BuffMultiplier) × CritMultiplier
```

#### Wave System
**Key Classes:**
- `WaveManager`: Controls wave spawning
- `WaveData` (ScriptableObject): Enemy composition, timing
- `SpawnController`: Handles enemy spawning rhythm

---

### 3. Economy & Progression

#### Currency System
**Types:**
- **Gold/Silver**: Wave-to-wave currency (resets each chapter)
- **Sacred Essence**: Meta currency (permanent)

**Key Classes:**
- `CurrencyManager`: Tracks all currencies
- `ShopController`: Handles purchases (rolls, grid cells)

#### Action Panel System
**Key Features:**
- Random asset draw (2 free per wave)
- Roll mechanism (cost increases: 3→5→7→10 gold)
- Merge system (drag & drop identical assets)
- Buy grid cells (2-cell blocks)

#### Merge System
**Rules:**
- Level 1 + Level 1 → Level 2
- Level 2 + Level 2 → Level 3
- Level 3 + Level 3 → Level 4
- Max level is 4

**Key Classes:**
- `MergeController`: Handles merge logic
- `AssetInventory`: Stores available assets

#### Meta Progression
**Static Upgrades (The Forge):**
- Bought with Sacred Essence
- Permanent across all runs
- Examples: +5% Wall HP, +10% starting Gold

**Tech Tree (Great Library):**
- Branching unlock path
- Unlocks new unit types, abilities
- Saved in player profile

**Collections:**
- Soldier Codex: Shard collection → stat bonuses
- Relic Room: Boss drops → global bonuses

**Key Classes:**
- `MetaProgressionManager`: Saves/loads progression
- `UpgradeData` (ScriptableObject): Upgrade definitions
- `PlayerProfile`: Persistent player data

---

### 4. UI Systems

#### UI Architecture
```
Canvas (Screen Space - Overlay)
├── TopHUD (HP, Currency, Wave)
├── ActionPanel (Bottom, slides up)
│   ├── AssetSlots
│   ├── MergeArea
│   └── ShopButtons
├── GridUI (Placement feedback)
└── BattleUI (XP, buffs, hero controls)
```

**Key Classes:**
- `UIManager`: Coordinates all UI
- `ActionPanelController`: Slides, shows assets
- `HUDController`: Updates stats in real-time

---

## Data Flow Examples

### Example 1: Wave Start
```
1. GameStateMachine → Enter PreparationState
2. ActionPanelController.Show()
3. AssetDrawer.DrawAssets(2, random=true)
4. CurrencyManager.Load(previousWaveEarnings)
5. Wait for player actions...
```

### Example 2: Building Placement
```
1. Player drags building from ActionPanel
2. GridManager.ValidatePlacement(position)
3. If valid → GridCell.PlaceBuilding(building)
4. Building.Initialize(cellPosition)
5. ActionPanel.RemoveAsset(building)
```

### Example 3: Enemy Death
```
1. CombatSystem.ApplyDamage(enemy, damage)
2. If HP ≤ 0 → Enemy.Die()
3. EventSystem.Trigger(EnemyDeathEvent)
4. WaveManager.OnEnemyDeath() → Check wave complete
5. CurrencyManager.AddGold(enemy.goldReward)
6. XPManager.AddXP(enemy.xpReward)
```

### Example 4: Spawn Queue
```
1. Soldier A dies in combat
2. Barracks.OnSoldierDeath(A)
3. If currentCount < capacity → StartSpawnTimer()
4. Soldier B dies (timer at 50%)
5. Barracks.AddToQueue(B)
6. Timer completes → SpawnSoldier(A)
7. Start new timer for B immediately
```

---

## Performance Considerations

### Object Pooling
- **Pool ALL units and projectiles**
- Pre-warm pools at scene start
- Return objects instead of destroying

### Update Loops
- Use managers with centralized Update loops
- Avoid Update() in individual units
- Consider Tick-based updates (every 0.1s) for non-critical logic

### Memory Management
- ScriptableObjects are loaded once, shared
- Unload unused assets between chapters
- Profile regularly with Unity Profiler

---

## Save System

### Save Data Structure
```json
{
  "playerProfile": {
    "sacredEssence": 5000,
    "staticUpgrades": {
      "engineering": 3,
      "logistics": 2
    },
    "techTree": ["heavy_infantry", "chain_lightning"],
    "collections": {
      "soldierCodex": {...},
      "relicRoom": [...]
    }
  },
  "currentChapter": {
    "chapterId": "chapter_2",
    "waveNumber": 3,
    "gold": 150,
    "silver": 75
  }
}
```

### Key Classes
- `SaveManager`: Handles save/load
- `PlayerProfile`: Serializable data class
- Uses Unity's JsonUtility or external library (Newtonsoft.Json)

---

## Testing Strategy

### Unit Tests
- Test damage calculations
- Test merge logic
- Test spawn queue timing

### Integration Tests
- Test full gameplay loop
- Test save/load
- Test currency flow

### Playtest Checklist
- Balance testing (difficulty curve)
- UI/UX flow
- Performance (60 FPS target)

---

## Development Roadmap

### Phase 1: Foundation (Weeks 1-3)
- [ ] Core managers (Game, Event, State)
- [ ] Grid system
- [ ] Basic unit and building structure
- [ ] Data architecture (ScriptableObjects)

### Phase 2: Core Loop (Weeks 4-6)
- [ ] Full gameplay state machine
- [ ] Action Panel with roll/merge
- [ ] Combat system
- [ ] Wave spawning

### Phase 3: Progression (Weeks 7-8)
- [ ] Currency and economy
- [ ] Meta progression (Forge, Library)
- [ ] Save/Load system

### Phase 4: Polish (Weeks 9-10)
- [ ] UI animations
- [ ] VFX and audio
- [ ] Balancing
- [ ] Bug fixing

---

## Resources & Learning

### Unity Patterns
- Unity Design Patterns: https://github.com/Habrador/Unity-Programming-Patterns
- ScriptableObject Architecture: Ryan Hipple's Unite Talk

### Recommended Assets
- DOTween: Smooth animations
- Odin Inspector: Better inspector (optional)
- TextMeshPro: Better text rendering

### Code Style
- Follow Unity C# naming conventions
- Use XML documentation comments
- Keep methods under 50 lines
- One class per file

---

## Next Steps
1. Set up Unity project with folder structure
2. Create base ScriptableObject classes
3. Implement GameManager and EventSystem
4. Build Grid system with basic placement
5. Create first working prototype (Prep → Deploy → Battle loop)

