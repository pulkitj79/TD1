using UnityEngine;

/// <summary>
/// Manages wave spawning and progression
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Current Wave")]
    [SerializeField] private int currentWaveIndex = 0;
    
    private ChapterData currentChapter;
    private WaveData currentWave;
    private int enemiesRemaining = 0;
    
    public int CurrentWaveNumber => currentWaveIndex + 1;
    
    public void Initialize()
    {
        EventSystem.Instance.Subscribe<EnemyDeathEvent>(OnEnemyDeath);
        Debug.Log("[WaveManager] Initialized");
    }
    
    public void LoadChapter(ChapterData chapter)
    {
        currentChapter = chapter;
        currentWaveIndex = 0;
        Debug.Log($"[WaveManager] Loaded chapter: {chapter.chapterName}");
    }
    
    public void StartWave()
    {
        if (currentWaveIndex >= currentChapter.waves.Length)
        {
            Debug.Log("[WaveManager] Chapter complete!");
            GameManager.Instance.OnChapterComplete();
            return;
        }
        
        currentWave = currentChapter.waves[currentWaveIndex];
        enemiesRemaining = CalculateTotalEnemies(currentWave);
        
        EventSystem.Instance.Trigger(new WaveStartedEvent(CurrentWaveNumber));
        
        Debug.Log($"[WaveManager] Started wave {CurrentWaveNumber}");
    }
    
    private int CalculateTotalEnemies(WaveData wave)
    {
        int total = 0;
        foreach (var group in wave.spawnGroups)
        {
            total += group.count;
        }
        return total;
    }
    
    private void OnEnemyDeath(EnemyDeathEvent evt)
    {
        enemiesRemaining--;
        
        if (enemiesRemaining <= 0)
        {
            OnWaveComplete();
        }
    }
    
    private void OnWaveComplete()
    {
        Debug.Log($"[WaveManager] Wave {CurrentWaveNumber} complete!");
        
        EventSystem.Instance.Trigger(new WaveCompletedEvent(
            CurrentWaveNumber,
            currentWave.goldReward,
            currentWave.silverReward,
            currentWave.sacredEssenceReward
        ));
        
        GameManager.Instance.Currency.AwardWaveRewards(
            currentWave.goldReward,
            currentWave.silverReward,
            currentWave.sacredEssenceReward
        );
        
        currentWaveIndex++;
    }
    
    private void OnDestroy()
    {
        EventSystem.Instance.Unsubscribe<EnemyDeathEvent>(OnEnemyDeath);
    }
}