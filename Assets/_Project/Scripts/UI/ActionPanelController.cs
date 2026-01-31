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
    [SerializeField] private float hiddenY = -400f; // Off-screen
    [SerializeField] private float visibleY = 0f;   // On-screen
    
    private bool isVisible = false;
    private Coroutine moveCoroutine;
    
    private void Awake()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }
        
        // Start hidden
        SetPanelPosition(hiddenY);
    }
    
    /// <summary>
    /// Show the Action Panel and raise grid
    /// </summary>
    public void Show()
    {
        if (isVisible) return;
        
        Debug.Log("[ActionPanel] Showing panel");
        
        // Raise grid first
        GameManager.Instance.Grid.RaiseGrid();
        
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
        if (!isVisible) return;
        
        Debug.Log("[ActionPanel] Hiding panel");
        
        // Hide panel first
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MovePanelTo(hiddenY));
        isVisible = false;
        
        // Lower grid after panel is hidden
        GameManager.Instance.Grid.LowerGrid();
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
        Vector2 startPos = panelRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        
        float elapsed = 0f;
        
        while (elapsed < showDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / showDuration;
            
            // Ease-out
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        
        panelRect.anchoredPosition = targetPos;
    }
    
    private void SetPanelPosition(float y)
    {
        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, y);
        }
    }
}