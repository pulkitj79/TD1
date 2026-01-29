# Implementation Guide - Tower Defense Game

## Getting Started

This guide will walk you through setting up your Unity project and implementing the core systems step by step.

---

## Phase 1: Project Setup (Day 1-2)

### Step 1: Create Unity Project
1. **Create new Unity 2022.3 LTS project** (or later)
2. **Project Type**: 2D or 3D (based on your preference)
3. **Recommended settings**:
   - Color Space: Linear (Edit → Project Settings → Player)
   - API Compatibility: .NET Standard 2.1
   - Scripting Backend: IL2CPP (for better performance)

### Step 2: Folder Structure
Create this exact folder structure in your Assets folder:

```
Assets/
├── _Project/
│   ├── Scenes/
│   │   └── MainGame.unity
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Data/
│   │   ├── Gameplay/
│   │   │   ├── Grid/
│   │   │   ├── Buildings/
│   │   │   ├── Units/
│   │   │   ├── Combat/
│   │   │   └── Wave/
│   │   ├── UI/
│   │   ├── Meta/
│   │   ├── Managers/
│   │   └── Utilities/
│   ├── Data/              # ScriptableObject assets
│   │   ├── Buildings/
│   │   ├── Units/
│   │   ├── Waves/
│   │   ├── Chapters/
│   │   └── Upgrades/
│   ├── Prefabs/
│   │   ├── Buildings/
│   │   ├── Units/
│   │   ├── UI/
│   │   └── VFX/
│   └── Art/
│       ├── Sprites/
│       ├── UI/
│       └── Materials/
```

### Step 3: Install Foundation Scripts
1. Copy the provided scripts to their respective folders:
   - `EventSystem.cs` → Scripts/Core/
   - `GameStateMachine.cs` → Scripts/Core/
   - `GameManager.cs` → Scripts/Core/
   - `ScriptableObjectData.cs` → Scripts/Data/
   - `CurrencyManager.cs` → Scripts/Managers/

2. **Create empty manager scripts** (we'll implement these later):
   ```
   Scripts/Managers/
   ├── GridManager.cs
   ├── WaveManager.cs
   ├── UIManager.cs
   ├── BuildingManager.cs
   ├── CombatManager.cs
   └── MetaProgressionManager.cs
   ```

### Step 4: Setup Main Scene
1. Open `MainGame.unity`
2. Create a GameObject called "GameManager"
3. Attach the following components:
   - `GameManager.cs`
   - `GameStateMachine.cs`
   - `CurrencyManager.cs`
   
4. Create child GameObjects for other managers:
   ```
   GameManager
   ├── GridManager (add GridManager.cs)
   ├── WaveManager (add WaveManager.cs)
   ├── UIManager (add UIManager.cs)
   ├── BuildingManager (add BuildingManager.cs)
   ├── CombatManager (add CombatManager.cs)
   └── MetaProgressionManager (add MetaProgressionManager.cs)
   ```

5. In GameManager inspector, drag and drop the references to each manager

---

## Phase 2: Core Systems Implementation (Week 1)

### A. GridManager Implementation

**Purpose**: Manage the 5x10 grid where buildings are placed

**GridManager.cs skeleton:**
```csharp
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private int columns = 10;
    [SerializeField] private int rows = 5;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    
    private GridCell[,] grid;
    private List<GridCell> availableCells = new List<GridCell>();
    
    public void Initialize()
    {
        CreateGrid();
        Debug.Log("[GridManager] Initialized");
    }
    
    public void SetupGrid(GridConfig config)
    {
        // Set initial visible cells based on config
        int totalCells = columns * rows;
        int visibleCount = Mathf.RoundToInt(totalCells * config.initialVisiblePercentage);
        
        // TODO: Randomly select which cells to make visible
        // TODO: Ensure they're somewhat clustered/strategic
    }
    
    private void CreateGrid()
    {
        grid = new GridCell[columns, rows];
        
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";
                
                GridCell cell = cellObj.GetComponent<GridCell>();
                cell.Initialize(new Vector2Int(x, y), false); // Start hidden
                grid[x, y] = cell;
            }
        }
    }
    
    public Vector2 GetWorldPosition(int x, int y)
    {
        return gridOrigin + new Vector2(x * cellSize, y * cellSize);
    }
    
    public Vector2Int GetGridPosition(Vector2 worldPos)
    {
        Vector2 localPos = worldPos - gridOrigin;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }
    
    public bool CanPlaceBuilding(Vector2Int position, Vector2Int buildingSize)
    {
        // Check if all required cells are available
        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int checkPos = position + new Vector2Int(x, y);
                if (!IsCellAvailable(checkPos)) return false;
            }
        }
        return true;
    }
    
    private bool IsCellAvailable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= columns || pos.y < 0 || pos.y >= rows)
            return false;
        
        GridCell cell = grid[pos.x, pos.y];
        return cell.IsVisible && !cell.IsOccupied;
    }
    
    public void PlaceBuilding(Building building, Vector2Int position)
    {
        // Mark cells as occupied
        Vector2Int size = building.Data.size;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPos = position + new Vector2Int(x, y);
                grid[cellPos.x, cellPos.y].SetOccupied(building);
            }
        }
    }
    
    public bool PurchaseCellBlock(Vector2Int position)
    {
        // Unlock a 2x1 cell block
        // TODO: Check cost with CurrencyManager
        // TODO: Mark cells as visible
        return false;
    }
}
```

**GridCell.cs:**
```csharp
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color visibleColor = Color.white;
    [SerializeField] private Color hiddenColor = Color.gray;
    [SerializeField] private Color occupiedColor = Color.red;
    [SerializeField] private Color validPlacementColor = Color.green;
    
    public Vector2Int GridPosition { get; private set; }
    public bool IsVisible { get; private set; }
    public bool IsOccupied { get; private set; }
    public Building OccupyingBuilding { get; private set; }
    
    public void Initialize(Vector2Int gridPos, bool visible)
    {
        GridPosition = gridPos;
        IsVisible = visible;
        IsOccupied = false;
        UpdateVisual();
    }
    
    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        UpdateVisual();
    }
    
    public void SetOccupied(Building building)
    {
        IsOccupied = true;
        OccupyingBuilding = building;
        UpdateVisual();
    }
    
    public void ClearOccupancy()
    {
        IsOccupied = false;
        OccupyingBuilding = null;
        UpdateVisual();
    }
    
    public void ShowPlacementPreview(bool valid)
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = valid ? validPlacementColor : Color.red;
        }
    }
    
    private void UpdateVisual()
    {
        if (!spriteRenderer) return;
        
        if (!IsVisible)
            spriteRenderer.color = hiddenColor;
        else if (IsOccupied)
            spriteRenderer.color = occupiedColor;
        else
            spriteRenderer.color = visibleColor;
    }
}
```

### B. Building System Implementation

**Base Building Class:**
```csharp
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected BuildingData data;
    
    public BuildingData Data => data;
    public int CurrentLevel { get; protected set; } = 1;
    public Vector2Int GridPosition { get; protected set; }
    
    public virtual void Initialize(BuildingData buildingData, Vector2Int gridPos, int level = 1)
    {
        data = buildingData;
        GridPosition = gridPos;
        CurrentLevel = level;
        
        Debug.Log($"[Building] {data.buildingName} initialized at {gridPos}");
    }
    
    public virtual void Activate()
    {
        // Called when battle phase starts
        enabled = true;
    }
    
    public virtual void Deactivate()
    {
        // Called when battle phase ends
        enabled = false;
    }
    
    public virtual void UpgradeLevel(int newLevel)
    {
        CurrentLevel = Mathf.Clamp(newLevel, 1, 4);
        ApplyLevelStats();
    }
    
    protected virtual void ApplyLevelStats()
    {
        // Override in derived classes
    }
}
```

**Barracks Implementation:**
```csharp
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Building
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    
    private int activeUnits = 0;
    private float spawnTimer = 0f;
    private Queue<Soldier> spawnQueue = new Queue<Soldier>();
    private bool isSpawning = false;
    
    private int maxCapacity;
    private float currentSpawnTime;
    
    protected override void ApplyLevelStats()
    {
        BuildingLevelData levelData = data.GetLevelData(CurrentLevel);
        
        maxCapacity = data.maxActiveUnits + levelData.capacityBonus;
        currentSpawnTime = data.spawnTime * levelData.spawnSpeedMultiplier;
    }
    
    public override void Initialize(BuildingData buildingData, Vector2Int gridPos, int level = 1)
    {
        base.Initialize(buildingData, gridPos, level);
        ApplyLevelStats();
        
        // Subscribe to soldier death events
        EventSystem.Instance.Subscribe<SoldierDeathEvent>(OnSoldierDeath);
    }
    
    private void Update()
    {
        if (!enabled) return;
        
        ProcessSpawnQueue();
    }
    
    private void ProcessSpawnQueue()
    {
        if (isSpawning)
        {
            spawnTimer -= Time.deltaTime;
            
            if (spawnTimer <= 0f)
            {
                SpawnSoldier();
                isSpawning = false;
            }
        }
        else if (activeUnits < maxCapacity && spawnQueue.Count > 0)
        {
            StartSpawning();
        }
    }
    
    private void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = currentSpawnTime;
    }
    
    private void SpawnSoldier()
    {
        // TODO: Get soldier from object pool
        // TODO: Initialize soldier with data.spawnedUnit
        // TODO: Place at spawnPoint
        
        activeUnits++;
        
        EventSystem.Instance.Trigger(new SoldierSpawnedEvent(null, this));
        
        Debug.Log($"[Barracks] Spawned soldier. Active: {activeUnits}/{maxCapacity}");
    }
    
    private void OnSoldierDeath(SoldierDeathEvent evt)
    {
        // Only process if this soldier belongs to this barracks
        // TODO: Track which soldiers belong to which barracks
        
        activeUnits--;
        spawnQueue.Enqueue(evt.Soldier);
        
        if (!isSpawning)
        {
            StartSpawning();
        }
    }
    
    private void OnDestroy()
    {
        EventSystem.Instance.Unsubscribe<SoldierDeathEvent>(OnSoldierDeath);
    }
}
```

---

## Phase 3: Testing the Foundation (Week 1)

### Create Test Data

1. **Create a test Unit**:
   - Right-click in Project → Create → Game Data → Unit
   - Name: "BasicSoldier"
   - Set stats: HP=100, Damage=10, etc.

2. **Create a test Building**:
   - Right-click in Project → Create → Game Data → Building
   - Name: "BasicBarracks"
   - Type: Barracks
   - Link BasicSoldier as spawnedUnit

3. **Create a test Chapter**:
   - Right-click in Project → Create → Game Data → Chapter
   - Name: "Chapter1"
   - Set starting resources

### Test Checklist

- [ ] GameManager initializes without errors
- [ ] Grid creates properly (5x10)
- [ ] Currency can be added/spent
- [ ] State machine transitions work
- [ ] Events trigger correctly (use Debug.Log to verify)

---

## Best Practices for Part-Time Development

### 1. Version Control
- Use Git/GitHub for version control
- Commit after each working feature
- Branch structure:
  - `main` - stable builds
  - `develop` - active development
  - `feature/grid-system` - individual features

### 2. Time Management
- **30-60 min sessions**: Focus on ONE small task
- **Daily commits**: Even if incomplete, commit progress
- **Weekly goals**: Set achievable weekly milestones

### 3. Documentation
- **Comment your code**: Future you will thank you
- **Keep a dev journal**: Quick notes on what you did
- **TODO comments**: Mark incomplete sections

Example:
```csharp
// TODO: Implement object pooling for soldiers
// FIXME: Spawn timer doesn't account for queue properly
// NOTE: This formula matches the design doc section 4
```

### 4. Testing Strategy
- Test each system in isolation first
- Use Unity's Context Menu for quick tests:
```csharp
[ContextMenu("Test Spawn Soldier")]
private void TestSpawn()
{
    SpawnSoldier();
}
```

### 5. Avoid Scope Creep
- Stick to the design document
- Mark "nice to have" features for later
- Finish core loop before adding polish

---

## Common Pitfalls to Avoid

### 1. ❌ Don't: Update() in Every Object
```csharp
// BAD - 100 soldiers = 100 Update calls per frame
public class Soldier : MonoBehaviour
{
    void Update()
    {
        CheckEnemies();
        Move();
    }
}
```

### 2. ✅ Do: Manager-Based Updates
```csharp
// GOOD - 1 Update call manages all soldiers
public class CombatManager : MonoBehaviour
{
    List<Soldier> activeSoldiers = new List<Soldier>();
    
    void Update()
    {
        foreach (var soldier in activeSoldiers)
        {
            soldier.Tick(Time.deltaTime);
        }
    }
}
```

### 3. ❌ Don't: Hardcode Values
```csharp
// BAD
int soldierHP = 100;
```

### 4. ✅ Do: Use ScriptableObjects
```csharp
// GOOD
int soldierHP = unitData.baseHP;
```

### 5. ❌ Don't: Use Find/GetComponent Every Frame
```csharp
// BAD
void Update()
{
    GameObject enemy = GameObject.Find("Enemy");
}
```

### 6. ✅ Do: Cache References
```csharp
// GOOD
private Enemy targetEnemy;

void SetTarget(Enemy enemy)
{
    targetEnemy = enemy;
}
```

---

## Next Steps After Foundation

Once you have the foundation working:

1. **Week 2**: Implement ActionPanel UI and merge system
2. **Week 3**: Build combat system with basic AI
3. **Week 4**: Wave spawning and progression
4. **Week 5**: Meta progression (Forge, Library)
5. **Week 6**: Polish, VFX, and balancing

---

## Resources

### Unity-Specific
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting Reference: https://docs.unity3d.com/ScriptReference/

### Design Patterns
- Game Programming Patterns: http://gameprogrammingpatterns.com/
- Unity Design Patterns: https://github.com/Habrador/Unity-Programming-Patterns

### Learning
- Brackeys YouTube (Unity basics)
- CodeMonkey YouTube (Unity patterns)
- Unity Learn (official tutorials)

---

## Questions & Debugging

### "How do I debug events?"
Add logging to EventSystem:
```csharp
public void Trigger<T>(T gameEvent) where T : IGameEvent
{
    Debug.Log($"[Event] Triggered: {typeof(T).Name}");
    // ... rest of method
}
```

### "Objects aren't showing in scene"
- Check Z position (2D) or Camera frustum (3D)
- Verify SpriteRenderer/MeshRenderer is enabled
- Check sorting layer/order

### "Performance is slow"
- Use Unity Profiler (Window → Analysis → Profiler)
- Look for expensive Update() calls
- Check for memory allocations in loops

---

## Support

If you get stuck:
1. Check Unity console for errors
2. Review the PROJECT_ARCHITECTURE.md
3. Test systems in isolation
4. Use Debug.Log extensively

Remember: **Build iteratively, test frequently, commit often!**
