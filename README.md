# Tower Defense Game

A modular, data-driven tower defense game built in Unity with a focus on clean architecture and maintainability.

![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Development Status](https://img.shields.io/badge/status-in%20development-yellow)

## 🎮 Game Overview

A strategic tower defense game featuring:
- **Four-Phase Gameplay Loop**: Preparation → Deployment → Battle → Resolution
- **Dynamic Grid System**: 5x10 grid with expandable cells
- **Asset Merging**: Combine identical buildings to level them up (max level 4)
- **Multiple Building Types**: Barracks (spawn soldiers), Towers (ranged attacks), Support (buffs)
- **Meta Progression**: Permanent upgrades, tech trees, and collection systems
- **Wave-Based Combat**: Increasing difficulty with boss waves

## 🏗️ Architecture

Built using professional game development patterns:
- **MVC Pattern**: Clean separation of data, logic, and presentation
- **Event System**: Decoupled communication between systems
- **ScriptableObjects**: Data-driven design for easy balancing
- **State Machine**: Manages game flow through distinct phases
- **Object Pooling**: Efficient spawning and despawning

### Core Systems
- **Grid System**: Dynamic 5x10 placement grid
- **Building System**: Barracks, Towers, Support buildings with level progression
- **Unit System**: Soldiers and enemies with automatic combat AI
- **Wave System**: Enemy spawning with configurable patterns
- **Currency System**: Gold/Silver (session) + Sacred Essence (permanent)
- **Combat System**: Damage calculation with buffs and modifiers
- **Meta Progression**: Static upgrades, tech tree, collections

## 📁 Project Structure

```
Assets/_Project/
├── Scenes/              # Game scenes
├── Scripts/
│   ├── Core/           # Core systems (GameManager, EventSystem, StateMachine)
│   ├── Data/           # ScriptableObject definitions
│   ├── Gameplay/       # Game logic (Grid, Buildings, Units, Combat, Wave)
│   ├── UI/             # UI controllers
│   ├── Meta/           # Progression systems
│   ├── Managers/       # Individual managers
│   └── Utilities/      # Helper classes
├── Data/               # ScriptableObject assets
│   ├── Buildings/
│   ├── Units/
│   ├── Waves/
│   ├── Chapters/
│   └── Upgrades/
├── Prefabs/            # Game prefabs
└── Art/                # Sprites, materials, animations
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Basic C# knowledge
- Git for version control

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/YOUR_USERNAME/tower-defense-game.git
cd tower-defense-game
```

2. **Open in Unity**
- Launch Unity Hub
- Click "Add" and select the project folder
- Open the project

3. **Open the main scene**
- Navigate to `Assets/_Project/Scenes/MainGame.unity`
- Press Play to test

### First-Time Setup

Follow the **QUICK_START_CHECKLIST.md** for a step-by-step guide to:
1. Set up the Unity project
2. Create the scene hierarchy
3. Create your first test data
4. Verify everything works

## 📚 Documentation

- **[QUICK_START_CHECKLIST.md](Documentation/QUICK_START_CHECKLIST.md)** - Your first 2 hours with the project
- **[PROJECT_ARCHITECTURE.md](Documentation/PROJECT_ARCHITECTURE.md)** - Complete system design and patterns
- **[IMPLEMENTATION_GUIDE.md](Documentation/IMPLEMENTATION_GUIDE.md)** - Detailed implementation tutorials

## 🎯 Development Roadmap

### Phase 1: Foundation ✅
- [x] Core manager architecture
- [x] Event system
- [x] State machine
- [x] Currency system
- [x] ScriptableObject data pipeline

### Phase 2: Core Gameplay (In Progress)
- [ ] Grid system with visualization
- [ ] Building placement mechanics
- [ ] Basic unit spawning
- [ ] Combat system
- [ ] Wave spawning

### Phase 3: Game Loop
- [ ] Action Panel UI
- [ ] Asset rolling and merging
- [ ] XP and buff system
- [ ] Hero mechanics
- [ ] Wave progression

### Phase 4: Meta Progression
- [ ] Static upgrades (The Forge)
- [ ] Tech tree (The Great Library)
- [ ] Collection systems (Codex, Relics)
- [ ] Save/load system

### Phase 5: Polish
- [ ] Visual effects
- [ ] Audio and music
- [ ] UI animations
- [ ] Balancing
- [ ] Performance optimization

## 🛠️ Key Scripts

### Core Systems
- `GameManager.cs` - Central game coordinator
- `EventSystem.cs` - Type-safe event bus
- `GameStateMachine.cs` - Four-phase state management
- `CurrencyManager.cs` - Gold, Silver, Sacred Essence

### Data Definitions
- `UnitData.cs` - Soldier and enemy stats
- `BuildingData.cs` - Barracks, Tower, Support data
- `WaveData.cs` - Enemy spawn configurations
- `ChapterData.cs` - Chapter settings and progression

### Managers
- `GridManager.cs` - 5x10 grid management
- `WaveManager.cs` - Wave spawning and progression
- `BuildingManager.cs` - Building creation and tracking
- `CombatManager.cs` - Damage calculations
- `MetaProgressionManager.cs` - Permanent upgrades

## 🎨 Creating Game Content

### Create a New Unit
1. Right-click in `Assets/_Project/Data/Units/`
2. Create → Game Data → Unit
3. Configure stats (HP, Damage, Speed, etc.)
4. Assign prefab and animations

### Create a New Building
1. Right-click in `Assets/_Project/Data/Buildings/`
2. Create → Game Data → Building
3. Set building type (Barracks/Tower/Support)
4. Configure stats and link units

### Create a New Wave
1. Right-click in `Assets/_Project/Data/Waves/`
2. Create → Game Data → Wave
3. Add spawn groups with enemy types
4. Set rewards

## 🧪 Testing

### Debug Commands
The GameManager and CurrencyManager have context menu commands for testing:
- Right-click on component → "Debug: Add 100 Gold"
- Right-click on component → "Debug: Add 50 Silver"
- Right-click on component → "Debug: Add 1000 Sacred Essence"

### Console Logging
All major systems log their activities. Watch the Console for:
```
[GameManager] Initializing all managers...
[EventSystem] Triggered: WaveStartedEvent
[CurrencyManager] Gold +50 (Total: 150)
```

## 🤝 Contributing

This is a solo development project, but suggestions and feedback are welcome!

### Development Guidelines
- Follow Unity C# naming conventions
- Keep methods under 50 lines
- Use XML documentation comments
- Test each feature in isolation
- Commit working code frequently

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Architecture inspired by Unity design patterns
- ScriptableObject architecture based on Ryan Hipple's Unite talk
- Event system pattern from game programming best practices

## 📧 Contact

- **Developer**: Pulkit Jain
- **Email**: developer.pulkit@gmail.com
- **Project Link**: https://github.com/pulkitj79/TD1

---

**Happy Coding!** 🎮✨

Remember: Build iteratively, test frequently, commit often!
