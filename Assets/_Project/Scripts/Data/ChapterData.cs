using UnityEngine;

/// <summary>
/// ScriptableObject that defines a chapter's configuration
/// </summary>
[CreateAssetMenu(fileName = "New Chapter", menuName = "Game Data/Chapter")]
public class ChapterData : ScriptableObject
{
    [Header("Chapter Info")]
    public string chapterName = "Chapter 1";
    public string chapterDescription = "The beginning of your defense";
    
    [Header("Starting Resources")]
    public int startingGold = 100;
    public int startingSilver = 50;
    
    [Header("Grid Configuration")]
    public GridConfig gridConfig;
    
    [Header("Waves")]
    public WaveData[] waves;
    
    [Header("Rewards")]
    public int sacredEssenceReward = 100;
    public ChapterData nextChapter;
}