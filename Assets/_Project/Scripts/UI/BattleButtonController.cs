using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Battle button - transitions from Preparation to Battle state
/// </summary>
public class BattleButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button battleButton;
    [SerializeField] private ActionPanelController actionPanel;
    
    private void Start()
    {
        // Find references if not assigned
        if (battleButton == null)
        {
            battleButton = GetComponent<Button>();
        }
        
        if (actionPanel == null)
        {
            actionPanel = FindObjectOfType<ActionPanelController>();
        }
        
        // Add click listener
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(OnBattleButtonClicked);
        }
        
        // Subscribe to state changes to show/hide button
        EventSystem.Instance.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        
        // Update button visibility based on current state
        UpdateButtonVisibility();
    }
    
    private void OnDestroy()
    {
        if (battleButton != null)
        {
            battleButton.onClick.RemoveListener(OnBattleButtonClicked);
        }
        
        if (EventSystem.Instance != null)
        {
            EventSystem.Instance.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }
    }
    
    /// <summary>
    /// Called when Battle button is clicked
    /// </summary>
    private void OnBattleButtonClicked()
    {
        Debug.Log("[BattleButton] Clicked! Starting battle...");
        
        // Hide Action Panel
        if (actionPanel != null)
        {
            actionPanel.Hide();
        }
        
        // Change game state to Battle
        if (GameManager.Instance != null && GameManager.Instance.StateMachine != null)
        {
            GameManager.Instance.StateMachine.ChangeState(GameState.Battle);
        }
    }
    
    /// <summary>
    /// Handle game state changes
    /// </summary>
    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        UpdateButtonVisibility();
    }
    
    /// <summary>
    /// Show button only in Preparation and Deployment states
    /// </summary>
    private void UpdateButtonVisibility()
    {
        if (battleButton == null) return;
        
        GameState currentState = GameManager.Instance.StateMachine.CurrentState;
        
        // Show button in Preparation and Deployment, hide in Battle and Resolution
        bool shouldShow = (currentState == GameState.Preparation || 
                          currentState == GameState.Deployment);
        
        battleButton.gameObject.SetActive(shouldShow);
        
        Debug.Log($"[BattleButton] Visibility: {shouldShow} (State: {currentState})");
    }
}