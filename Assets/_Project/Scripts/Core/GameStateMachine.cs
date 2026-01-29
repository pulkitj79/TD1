using UnityEngine;

/// <summary>
/// Controls the four-phase gameplay loop: Preparation → Deployment → Battle → Resolution
/// Uses the State pattern for clean state management
/// </summary>
public class GameStateMachine : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    
    private IGameState _currentStateHandler;
    
    // State handlers
    private PreparationState _preparationState;
    private DeploymentState _deploymentState;
    private BattleState _battleState;
    private ResolutionState _resolutionState;

    private void Awake()
    {
        // Initialize all state handlers
        _preparationState = new PreparationState(this);
        _deploymentState = new DeploymentState(this);
        _battleState = new BattleState(this);
        _resolutionState = new ResolutionState(this);
    }

    public void Initialize()
    {
        // Start with Preparation state
        ChangeState(GameState.Preparation);
    }

    /// <summary>
    /// Change to a new game state
    /// </summary>
    public void ChangeState(GameState newState)
    {
        // Exit current state
        _currentStateHandler?.Exit();
        
        GameState previousState = CurrentState;
        CurrentState = newState;
        
        // Enter new state
        _currentStateHandler = GetStateHandler(newState);
        _currentStateHandler?.Enter();
        
        // Notify listeners
        EventSystem.Instance.Trigger(new GameStateChangedEvent(previousState, newState));
        
        Debug.Log($"[GameStateMachine] State changed: {previousState} → {newState}");
    }

    private IGameState GetStateHandler(GameState state)
    {
        switch (state)
        {
            case GameState.Preparation: return _preparationState;
            case GameState.Deployment: return _deploymentState;
            case GameState.Battle: return _battleState;
            case GameState.Resolution: return _resolutionState;
            default: return null;
        }
    }

    private void Update()
    {
        _currentStateHandler?.Update();
    }
}

/// <summary>
/// Interface for all game states
/// </summary>
public interface IGameState
{
    void Enter();
    void Update();
    void Exit();
}

// ============================================
// STATE IMPLEMENTATIONS
// ============================================

/// <summary>
/// PREPARATION STATE (The Logic Phase)
/// - Action Panel slides up
/// - Receive 2 free random assets
/// - Roll for more assets or buy grid cells
/// - Merge identical assets
/// </summary>
public class PreparationState : IGameState
{
    private GameStateMachine _stateMachine;

    public PreparationState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("[PreparationState] Entering Preparation Phase");
        
        // TODO: Slide up Action Panel
        // TODO: Draw 2 random assets
        // TODO: Enable roll and buy buttons
        
        // For now, auto-transition after a delay (replace with actual logic)
        // In real implementation, player clicks "Ready" or "Deploy" button
    }

    public void Update()
    {
        // Listen for player input to transition to Deployment
        // This will be handled by UI buttons in actual implementation
        
        // Example: If player presses Deploy button
        // _stateMachine.ChangeState(GameState.Deployment);
    }

    public void Exit()
    {
        Debug.Log("[PreparationState] Exiting Preparation Phase");
        // TODO: Hide Action Panel (or keep visible but disabled)
    }
}

/// <summary>
/// DEPLOYMENT STATE (The Strategy Phase)
/// - Place assets on the 5x10 grid
/// - Validate placements
/// - Balance strategy
/// </summary>
public class DeploymentState : IGameState
{
    private GameStateMachine _stateMachine;

    public DeploymentState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("[DeploymentState] Entering Deployment Phase");
        
        // TODO: Enable grid for placement
        // TODO: Show "Battle" button
        // TODO: Allow drag-and-drop from Action Panel to Grid
    }

    public void Update()
    {
        // Listen for "Start Battle" button
        // Example: If player clicks Battle button
        // _stateMachine.ChangeState(GameState.Battle);
    }

    public void Exit()
    {
        Debug.Log("[DeploymentState] Exiting Deployment Phase");
        // TODO: Lock grid placements
        // TODO: Hide Action Panel
    }
}

/// <summary>
/// BATTLE STATE (The Battle Phase)
/// - Enemies spawn from top
/// - Soldiers exit gates to engage
/// - Automatic combat
/// - XP system and buffs
/// - Hero control (Wave 3+)
/// </summary>
public class BattleState : IGameState
{
    private GameStateMachine _stateMachine;

    public BattleState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("[BattleState] Entering Battle Phase");
        
        // TODO: Hide Action Panel
        // TODO: Reset grid/battlezone visual state
        // TODO: Start enemy spawning
        // TODO: Activate all buildings
        
        // Subscribe to wave completion
        EventSystem.Instance.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
    }

    public void Update()
    {
        // Combat happens automatically
        // This state ends when wave is completed or wall is destroyed
    }

    public void Exit()
    {
        Debug.Log("[BattleState] Exiting Battle Phase");
        
        // TODO: Stop all combat
        // TODO: Deactivate buildings
        
        // Unsubscribe from events
        EventSystem.Instance.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
    }

    private void OnWaveCompleted(WaveCompletedEvent evt)
    {
        // Transition to Resolution state
        _stateMachine.ChangeState(GameState.Resolution);
    }
}

/// <summary>
/// RESOLUTION STATE (The Reward Phase)
/// - Display wave completion
/// - Award Gold, Silver, Sacred Essence
/// - Show rewards UI
/// - Cycle repeats or chapter ends
/// </summary>
public class ResolutionState : IGameState
{
    private GameStateMachine _stateMachine;
    private float _resolutionTimer = 0f;
    private const float RESOLUTION_DURATION = 3f; // Show rewards for 3 seconds

    public ResolutionState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("[ResolutionState] Entering Resolution Phase");
        
        _resolutionTimer = 0f;
        
        // TODO: Show victory UI
        // TODO: Display earned rewards
        // TODO: Update currency displays
        // TODO: Check if chapter is complete
    }

    public void Update()
    {
        _resolutionTimer += Time.deltaTime;
        
        // Auto-transition back to Preparation after showing rewards
        if (_resolutionTimer >= RESOLUTION_DURATION)
        {
            // Check if chapter complete
            bool chapterComplete = false; // TODO: Get from WaveManager
            
            if (chapterComplete)
            {
                // TODO: Go to chapter complete screen
                Debug.Log("[ResolutionState] Chapter Complete!");
            }
            else
            {
                // Next wave - back to Preparation
                _stateMachine.ChangeState(GameState.Preparation);
            }
        }
    }

    public void Exit()
    {
        Debug.Log("[ResolutionState] Exiting Resolution Phase");
        // TODO: Hide rewards UI
    }
}

/// <summary>
/// Game state enumeration
/// </summary>
