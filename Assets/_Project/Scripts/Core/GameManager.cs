using UnityEngine;

/// <summary>
/// Central game manager that coordinates all game systems
/// Singleton pattern for global access
/// Initialize all managers and maintain game state
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private MetaProgressionManager metaProgressionManager;

    [Header("Game Configuration")]
    [SerializeField] private ChapterData currentChapter;
    
    // State Machine
    private GameStateMachine _stateMachine;
    
    // Properties
    public GridManager Grid => gridManager;
    public WaveManager Wave => waveManager;
    public CurrencyManager Currency => currencyManager;
    public UIManager UI => uiManager;
    public BuildingManager Buildings => buildingManager;
    public CombatManager Combat => combatManager;
    public MetaProgressionManager MetaProgression => metaProgressionManager;
    public GameStateMachine StateMachine => _stateMachine;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get or add GameStateMachine
        _stateMachine = GetComponent<GameStateMachine>();
        if (_stateMachine == null)
        {
            _stateMachine = gameObject.AddComponent<GameStateMachine>();
        }

        InitializeManagers();
    }

    private void Start()
    {
        if (currentChapter != null)
        {
            StartChapter(currentChapter);
        }
        else
        {
            Debug.LogWarning("[GameManager] No chapter assigned! Create a ChapterData asset and assign it.");
        }
    }

    /// <summary>
    /// Initialize all game managers in correct order
    /// </summary>
    private void InitializeManagers()
    {
        Debug.Log("[GameManager] Initializing all managers...");

        // Initialize managers that don't depend on others first
        if (metaProgressionManager != null)
            metaProgressionManager.Initialize();
        
        if (currencyManager != null)
            currencyManager.Initialize();
        
        // Initialize gameplay managers
        if (gridManager != null)
            gridManager.Initialize();
        
        if (buildingManager != null)
            buildingManager.Initialize();
        
        if (combatManager != null)
            combatManager.Initialize();
        
        if (waveManager != null)
            waveManager.Initialize();
        
        // Initialize UI last (depends on other managers)
        if (uiManager != null)
            uiManager.Initialize();

        Debug.Log("[GameManager] All managers initialized successfully");
    }

    /// <summary>
    /// Start a new chapter
    /// </summary>
    public void StartChapter(ChapterData chapter)
    {
        if (chapter == null)
        {
            Debug.LogError("[GameManager] Cannot start chapter - ChapterData is null!");
            return;
        }

        Debug.Log($"[GameManager] Starting Chapter: {chapter.chapterName}");
        
        currentChapter = chapter;
        
        // Apply meta upgrades to this run
        if (metaProgressionManager != null)
            metaProgressionManager.ApplyStaticUpgrades();
        
        // Set starting currency for the chapter
        int startingGold = chapter.startingGold;
        int startingSilver = chapter.startingSilver;
        
        if (metaProgressionManager != null)
            startingGold += metaProgressionManager.GetLogisticsBonus();
        
        if (currencyManager != null)
        {
            currencyManager.SetGold(startingGold);
            currencyManager.SetSilver(startingSilver);
        }
        
        // Initialize grid with chapter configuration
        if (gridManager != null)
            gridManager.SetupGrid(chapter.gridConfig);
        
        // Load chapter waves
        if (waveManager != null)
            waveManager.LoadChapter(chapter);
        
        // Start the state machine
        if (_stateMachine != null)
            _stateMachine.Initialize();
    }

    /// <summary>
    /// Called when wall is destroyed - game over
    /// </summary>
    public void OnWallDestroyed()
    {
        Debug.Log("[GameManager] Wall Destroyed - Game Over");
        
        // TODO: Show defeat screen
        // TODO: Award participation rewards
        // TODO: Return to menu
    }

    /// <summary>
    /// Called when chapter is completed successfully
    /// </summary>
    public void OnChapterComplete()
    {
        Debug.Log($"[GameManager] Chapter Complete: {currentChapter.chapterName}");
        
        // TODO: Calculate final rewards
        // TODO: Show victory screen
        // TODO: Save progress
        // TODO: Unlock next chapter
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("[GameManager] Game Paused");
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameManager] Game Resumed");
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMenu()
    {
        // TODO: Save progress
        // TODO: Load menu scene
        Debug.Log("[GameManager] Returning to Menu");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}