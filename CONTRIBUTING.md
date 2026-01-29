# Contributing to Tower Defense Game

Thank you for your interest in this project! This is primarily a solo development project, but I welcome feedback, suggestions, and bug reports.

## Development Status

This project is in **active development** as a part-time solo project. Progress may be sporadic but steady.

## How to Contribute

### Reporting Bugs
If you find a bug, please open an issue with:
- Description of the bug
- Steps to reproduce
- Expected behavior
- Actual behavior
- Unity version
- Screenshots (if applicable)

### Suggesting Features
Feature suggestions are welcome! Please open an issue with:
- Clear description of the feature
- Why it would be valuable
- How it fits with the current design

### Code Contributions
At this stage, I'm not accepting pull requests as I'm learning and building the architecture myself. However, feel free to:
- Fork the repository
- Experiment with your own features
- Share your discoveries in discussions

## Development Guidelines

If you're forking this project, here are the guidelines I follow:

### Code Style
- Follow Unity C# naming conventions
- Use XML documentation comments for public methods
- Keep methods under 50 lines when possible
- One class per file

### Naming Conventions
```csharp
// Classes and structs: PascalCase
public class GameManager { }

// Interfaces: IPascalCase
public interface IGameState { }

// Methods and Properties: PascalCase
public void StartGame() { }
public int Health { get; set; }

// Private fields: _camelCase
private int _currentHealth;

// Parameters and local variables: camelCase
public void TakeDamage(int damageAmount) { }

// Constants: UPPER_CASE
private const int MAX_LEVEL = 4;
```

### Documentation
```csharp
/// <summary>
/// Brief description of what this method does
/// </summary>
/// <param name="paramName">What this parameter represents</param>
/// <returns>What this method returns</returns>
public int CalculateDamage(int baseDamage)
{
    // Implementation
}
```

### Git Commit Messages
- Use present tense ("Add feature" not "Added feature")
- Keep first line under 50 characters
- Provide detailed description if needed

Examples:
```
Add grid cell purchasing system
Implement spawn queue for barracks
Fix currency not updating after wave complete
Refactor combat damage calculation
```

### Branch Strategy
- `main` - Stable, working builds only
- `develop` - Active development
- `feature/feature-name` - Individual features

### Testing Before Commit
- [ ] Code compiles without errors
- [ ] No null reference exceptions
- [ ] Feature works as intended
- [ ] Doesn't break existing features
- [ ] Console has no error messages

## Project Structure

When adding new features, follow the existing structure:

```
Assets/_Project/
├── Scripts/
│   ├── Core/           # Core systems only
│   ├── Data/           # ScriptableObject definitions
│   ├── Gameplay/       # Game logic
│   ├── UI/             # UI controllers
│   ├── Meta/           # Progression
│   └── Managers/       # Specific managers
```

## Communication

- Open an **Issue** for bugs or feature requests
- Start a **Discussion** for general questions
- Check existing issues before creating new ones

## Learning Resources

If you're learning Unity like me, here are helpful resources:
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting Reference: https://docs.unity3d.com/ScriptReference/
- Game Programming Patterns: http://gameprogrammingpatterns.com/
- Brackeys YouTube Channel
- CodeMonkey YouTube Channel

## Questions?

Feel free to open a discussion or issue. I'll respond when I can!

---

Thank you for your interest in this project! 🎮✨
