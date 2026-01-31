using UnityEngine;

/// <summary>
/// Controls Action Panel visibility and grid movement
/// </summary>
public class ActionPanelController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float showDuration = 0.3f;
    
    [Header("Positions")]
    [SerializeField] private float hiddenY = -500f; // Off-screen (adjust if needed)
    [SerializeField] private float visibleY = 0f;   // On-screen
    
    private bool isVisible = false;
    private Coroutine moveCoroutine;
    
    private void Awake()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }
    }
    
    private void Start()
    {
        // Start hidden
        SetPanelPosition(hiddenY);
        isVisible = false;
        
        // Show panel automatically when game starts (Preparation phase)
        Invoke("Show", 0.5f); // Small delay to ensure everything is initialized
    }
    
    /// <summary>
    /// Show the Action Panel and raise grid
    /// </summary>
    public void Show()
    {
        if (isVisible)
        {
            Debug.Log("[ActionPanel] Already visible");
            return;
        }
        
        Debug.Log("[ActionPanel] Showing panel");
        
        // Raise grid FIRST
        if (GameManager.Instance != null && GameManager.Instance.Grid != null)
        {
            GameManager.Instance.Grid.RaiseGrid();
        }
        
        // Then show panel
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MovePanelTo(visibleY));
        isVisible = true;
    }
    
    /// <summary>
    /// Hide the Action Panel and lower grid
    /// </summary>
    public void Hide()
    {
        if (!isVisible)
        {
            Debug.Log("[ActionPanel] Already hidden");
            return;
        }
        
        Debug.Log("[ActionPanel] Hiding panel");
        
        // Hide panel FIRST
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MovePanelTo(hiddenY));
        isVisible = false;
        
        // Then lower grid (after a small delay to let panel start moving)
        if (GameManager.Instance != null && GameManager.Instance.Grid != null)
        {
            // Delay grid lowering slightly
            StartCoroutine(DelayedLowerGrid());
        }
    }
    
    private System.Collections.IEnumerator DelayedLowerGrid()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (GameManager.Instance != null && GameManager.Instance.Grid != null)
        {
            GameManager.Instance.Grid.LowerGrid();
        }
    }
    
    /// <summary>
    /// Toggle panel visibility
    /// </summary>
    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }
    
    private System.Collections.IEnumerator MovePanelTo(float targetY)
    {
        if (panelRect == null)
        {
            Debug.LogError("[ActionPanel] Panel rect is null!");
            yield break;
        }
        
        Vector2 startPos = panelRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        
        Debug.Log($"[ActionPanel] Moving from Y={startPos.y} to Y={targetY}");
        
        float elapsed = 0f;
        
        while (elapsed < showDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / showDuration;
            
            // Ease-out cubic
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        panelRect.anchoredPosition = targetPos;
        
        Debug.Log($"[ActionPanel] Movement complete. Final Y={panelRect.anchoredPosition.y}");
    }
    
    private void SetPanelPosition(float y)
    {
        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, y);
        }
    }
    
    // Debug method - can be called from Inspector context menu
    [ContextMenu("Test: Show Panel")]
    public void TestShow()
    {
        Show();
    }
    
    [ContextMenu("Test: Hide Panel")]
    public void TestHide()
    {
        Hide();
    }
}