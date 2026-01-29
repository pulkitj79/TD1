# Quick Start Checklist

Your first 2 hours with this project structure.

---

## Hour 1: Setup Unity Project

### ☐ Create New Unity Project
- [ ] Unity 2022.3 LTS or newer
- [ ] Project name: "TowerDefenseGame"
- [ ] Template: 2D or 3D (your choice)

### ☐ Create Folder Structure
```
Copy the folder structure from IMPLEMENTATION_GUIDE.md into your Assets folder
```

### ☐ Import Foundation Scripts
- [ ] Create `Assets/_Project/Scripts/Core/`
- [ ] Add: `EventSystem.cs`
- [ ] Add: `GameStateMachine.cs`
- [ ] Add: `GameManager.cs`
- [ ] Create `Assets/_Project/Scripts/Data/`
- [ ] Add: `ScriptableObjectData.cs`
- [ ] Create `Assets/_Project/Scripts/Managers/`
- [ ] Add: `CurrencyManager.cs`
- [ ] Add: `ManagerScripts.cs` (split into individual files)

### ☐ Fix Compilation Errors
After adding scripts, Unity will show errors because some classes reference each other. This is normal!

**Quick fix strategy:**
1. Open each script
2. Comment out any lines with red underlines
3. Add `// TODO: Implement` above them
4. This allows Unity to compile

Example:
```csharp
// Before (ERROR):
GameManager.Instance.Grid.SetupGrid(chapter.gridConfig);

// After (COMPILES):
// TODO: Implement grid setup
// GameManager.Instance.Grid.SetupGrid(chapter.gridConfig);
```

---

## Hour 2: Create Basic Scene

### ☐ Setup Main Scene
- [ ] Create new scene: `MainGame.unity`
- [ ] Save in `Assets/_Project/Scenes/`

### ☐ Create GameManager GameObject
- [ ] In Hierarchy: Right-click → Create Empty
- [ ] Name: "GameManager"
- [ ] Add Component: `GameManager`
- [ ] Add Component: `GameStateMachine`
- [ ] Add Component: `CurrencyManager`

### ☐ Create Manager Children
Create these as children of GameManager:

```
GameManager
├── GridManager (empty GameObject + GridManager script)
├── WaveManager (empty + WaveManager script)
├── UIManager (empty + UIManager script)
├── BuildingManager (empty + BuildingManager script)
├── CombatManager (empty + CombatManager script)
└── MetaProgressionManager (empty + MetaProgressionManager script)
```

### ☐ Link Manager References
- [ ] Select GameManager
- [ ] In Inspector, find the GameManager component
- [ ] Drag each child GameObject into its corresponding field:
  - Grid Manager → GridManager field
  - Wave Manager → WaveManager field
  - UI Manager → UIManager field
  - Building Manager → BuildingManager field
  - Combat Manager → CombatManager field
  - Meta Progression Manager → MetaProgressionManager field

### ☐ Create First Test Data

**Step 1: Create a Basic Unit**
- [ ] In Project: `Assets/_Project/Data/Units/`
- [ ] Right-click → Create → Game Data → Unit
- [ ] Name: "BasicSoldier"
- [ ] In Inspector, set:
  - Unit Name: "Basic Soldier"
  - Unit ID: "soldier_basic"
  - Base HP: 100
  - Base Damage: 10
  - Attack Speed: 1.5
  - Move Speed: 3

**Step 2: Create a Basic Building**
- [ ] In Project: `Assets/_Project/Data/Buildings/`
- [ ] Right-click → Create → Game Data → Building
- [ ] Name: "BasicBarracks"
- [ ] In Inspector, set:
  - Building Name: "Basic Barracks"
  - Building ID: "barracks_basic"
  - Building Type: Barracks
  - Size: X=1, Y=1
  - Spawned Unit: (drag BasicSoldier here)
  - Max Active Units: 3
  - Spawn Time: 5

**Step 3: Create a Test Chapter**
- [ ] In Project: `Assets/_Project/Data/Chapters/`
- [ ] Right-click → Create → Game Data → Chapter
- [ ] Name: "Chapter1"
- [ ] In Inspector, set:
  - Chapter Name: "Tutorial"
  - Starting Gold: 100
  - Starting Silver: 50
  - Grid Config → Initial Visible Percentage: 0.45

### ☐ Link Chapter to GameManager
- [ ] Select GameManager in Hierarchy
- [ ] Find "Current Chapter" field
- [ ] Drag Chapter1 into this field

---

## Testing Your Setup

### ☐ First Test: Does It Compile?
- [ ] Press Play
- [ ] Check Console for errors
- [ ] Expected: Some warnings, but no red errors

### ☐ Second Test: Check Initialization
- [ ] Press Play
- [ ] Open Console
- [ ] Look for initialization messages:
  ```
  [GameManager] Initializing all managers...
  [CurrencyManager] Initialized
  [GridManager] Initialized
  ... (etc)
  [GameManager] All managers initialized successfully
  ```

### ☐ Third Test: State Machine
- [ ] Press Play
- [ ] Check Console for:
  ```
  [GameStateMachine] State changed: ... → Preparation
  [PreparationState] Entering Preparation Phase
  ```

### ☐ Fourth Test: Currency
- [ ] With game playing, select GameManager
- [ ] In Inspector, find CurrencyManager component
- [ ] Right-click on the component header
- [ ] Select "Debug: Add 100 Gold"
- [ ] Check Console for: `[Currency] Gold +100 (Total: 100)`

---

## Common First-Time Issues

### ❌ "NullReferenceException on GameManager.Instance"
**Fix:** Make sure GameManager GameObject exists in the scene before Play is pressed.

### ❌ "GridManager is null"
**Fix:** 
1. Select GameManager in Hierarchy
2. Make sure GridManager child exists
3. Drag GridManager into the "Grid Manager" field in GameManager inspector

### ❌ "Can't create ScriptableObject menu items"
**Fix:**
1. Make sure `ScriptableObjectData.cs` is in your project
2. Wait for Unity to recompile (watch bottom-right corner)
3. Try right-clicking again in Project window

### ❌ "EventSystem.Instance is null"
**Fix:** EventSystem creates itself automatically, but if there's an error:
1. Create empty GameObject named "EventSystem"
2. Add EventSystem component
3. Should fix itself

---

## What You've Accomplished

After these 2 hours, you now have:
- ✅ A properly structured Unity project
- ✅ Core manager architecture set up
- ✅ Event system working
- ✅ State machine functioning
- ✅ Currency system operational
- ✅ ScriptableObject data pipeline
- ✅ First test data created

---

## Next Session: Grid Visualization

**Goal:** Make the grid visible in the scene

**Time needed:** 1-2 hours

**Tasks:**
1. Create a simple grid cell prefab (white square sprite)
2. Implement GridManager.CreateGrid()
3. See your 5x10 grid appear in the scene!

**Preparation for next time:**
- [ ] Create a white square sprite (32x32 pixels)
- [ ] Import into Unity: `Assets/_Project/Art/Sprites/`
- [ ] Create Prefab: `Assets/_Project/Prefabs/GridCell.prefab`

---

## Getting Help

### Where to Look
1. **PROJECT_ARCHITECTURE.md** - Overall design
2. **IMPLEMENTATION_GUIDE.md** - Detailed tutorials
3. **Unity Console** - Error messages
4. **Unity Documentation** - https://docs.unity3d.com

### Debugging Tips
```csharp
// Add this to any script to see when methods run:
Debug.Log($"[ClassName] MethodName called");

// Add this to check values:
Debug.Log($"[ClassName] variableName = {variableName}");

// Add this for warnings:
Debug.LogWarning($"[ClassName] Something unexpected happened");

// Add this for errors:
Debug.LogError($"[ClassName] Critical error!");
```

### Common Debug Checks
- Is the GameObject in the scene?
- Is the script attached to the GameObject?
- Are all references assigned in the Inspector?
- Did Unity finish compiling? (check bottom-right corner)
- Is there an error in the Console?

---

## Celebrate! 🎉

You've just built the foundation of a complex, data-driven game system!

This is genuinely impressive work for a novice programmer. The architecture you've set up follows professional game development patterns and will scale well as you add more features.

**Keep going - you've got this!**

---

## Version Control (Optional but Recommended)

### ☐ Initialize Git Repository
```bash
cd /path/to/TowerDefenseGame
git init
```

### ☐ Create .gitignore
Download Unity's .gitignore: https://github.com/github/gitignore/blob/main/Unity.gitignore

### ☐ First Commit
```bash
git add .
git commit -m "Initial project setup with core managers and architecture"
```

This protects your work and lets you experiment without fear!
