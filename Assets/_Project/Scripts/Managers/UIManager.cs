using UnityEngine;

/// <summary>
/// Manages UI elements and transitions
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject battleUI;
    
    public void Initialize()
    {
        // Subscribe to state changes
        EventSystem.Instance.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        
        Debug.Log("[UIManager] Initialized");
    }
    
    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        switch (evt.NewState)
        {
            case GameState.Preparation:
                ShowActionPanel();
                break;
            case GameState.Battle:
                HideActionPanel();
                ShowBattleUI();
                break;
            case GameState.Resolution:
                ShowRewardsUI();
                break;
        }
    }
    
    private void ShowActionPanel()
    {
        // TODO: Slide up animation
        if (actionPanel) actionPanel.SetActive(true);
    }
    
    private void HideActionPanel()
    {
        // TODO: Slide down animation
        if (actionPanel) actionPanel.SetActive(false);
    }
    
    private void ShowBattleUI()
    {
        if (battleUI) battleUI.SetActive(true);
    }
    
    private void ShowRewardsUI()
    {
        // TODO: Show rewards screen
    }
    
    private void OnDestroy()
    {
        EventSystem.Instance.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }
}