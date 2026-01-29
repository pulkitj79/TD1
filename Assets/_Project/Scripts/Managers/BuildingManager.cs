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
        
        // TODO: Get world position from GridManager
        // Vector2 worldPos = GameManager.Instance.Grid.GetWorldPosition(gridPosition.x, gridPosition.y);
        Vector2 worldPos = Vector2.zero; // Temporary
        
        GameObject buildingObj = Instantiate(data.prefab, worldPos, Quaternion.identity, buildingsContainer);
        
        Building building = buildingObj.GetComponent<Building>();
        if (building == null)
        {
            Debug.LogError($"[BuildingManager] Prefab missing Building component");
            return null;
        }
        
        building.Initialize(data, gridPosition, level);
        activeBuildings.Add(building);
        
        EventSystem.Instance.Trigger(new BuildingPlacedEvent(building, gridPosition));
        
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