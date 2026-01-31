using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages building creation, placement, and tracking
/// </summary>
public class BuildingManager : MonoBehaviour
{
    [Header("Buildings")]
    [SerializeField] private Transform buildingsContainer;
    
    private List<Building> activeBuildings = new List<Building>();
    
    public void Initialize()
    {
        Debug.Log("[BuildingManager] Initialized");
    }
    
public Building CreateBuilding(BuildingData data, Vector2Int gridPosition, int level = 1)
{
    if (data.prefab == null)
    {
        Debug.LogError($"[BuildingManager] No prefab for {data.buildingName}");
        return null;
    }
    
    // Get world position from GridManager
    Vector2 worldPos = GameManager.Instance.Grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    
    // Get grid container reference
    Transform gridContainer = GameManager.Instance.Grid.GetGridContainer();
    
    // CRITICAL: Parent to grid container, not buildingsContainer!
    GameObject buildingObj = Instantiate(data.prefab, gridContainer);
    buildingObj.name = $"{data.buildingName}_{gridPosition.x}_{gridPosition.y}";
    
    // Set LOCAL position (relative to grid container)
    buildingObj.transform.position = worldPos;
    
    Building building = buildingObj.GetComponent<Building>();
    if (building == null)
    {
        Debug.LogError($"[BuildingManager] Prefab missing Building component");
        Destroy(buildingObj);
        return null;
    }
    
    building.Initialize(data, gridPosition, level);
    activeBuildings.Add(building);
    
    EventSystem.Instance.Trigger(new BuildingPlacedEvent(building, gridPosition));
    
    Debug.Log($"[BuildingManager] Placed {data.buildingName} at grid {gridPosition}, world {worldPos}, parent: {buildingObj.transform.parent.name}");
    
    return building;
}    
    public void RemoveBuilding(Building building)
    {
        if (activeBuildings.Contains(building))
        {
            activeBuildings.Remove(building);
            EventSystem.Instance.Trigger(new BuildingRemovedEvent(building, building.GridPosition));
            Destroy(building.gameObject);
        }
    }
    
    public void ActivateAllBuildings()
    {
        foreach (var building in activeBuildings)
        {
            building.Activate();
        }
    }
    
    public void DeactivateAllBuildings()
    {
        foreach (var building in activeBuildings)
        {
            building.Deactivate();
        }
    }
}