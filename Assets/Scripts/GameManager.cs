using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    None,
    RoadPlacement,
    HousePlacement,
    SpecialPlacement,
    BigStructurePlacement,
    Destroy
}

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public UIController uiController;
    public StructureManager structureManager;
    public SaveSystem saveSystem;

    private ActionType currentAction = ActionType.None;

    private void Start()
    {
        setMainAsActive();
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacementHandler;
        uiController.OnDestroySelection += DestroyHandler;
        uiController.OnCloseMenu += ClearInputActions;
        inputManager.OnRightMouseHold += cameraMovement.DragCamera;
        uiController.onMainInactive += setMainAsInactive;
        uiController.onMainActive += setMainAsActive;
        
        LoadGame();
    }

    private void setMainAsInactive()
    {
        inputManager.isMainActive = false;
        ClearInputActions();
    }

    private void setMainAsActive()
    {
        inputManager.isMainActive = true;
        ClearInputActions();
    }

    private void HousePlacementHandler()
    {
        if (ToggleButton(ActionType.HousePlacement))
        {
            inputManager.OnMouseClick +=/* (position) => */structureManager.PlaceHouse/*(position, 2)*/;
        }
    }

    private void BigStructurePlacementHandler()
    {
        if (ToggleButton(ActionType.BigStructurePlacement))
        {
            inputManager.OnMouseClick += structureManager.PlaceBigStructure;
        }
    }

    private void SpecialPlacementHandler()
    {
        if (ToggleButton(ActionType.SpecialPlacement))
        {
            inputManager.OnMouseClick += structureManager.PlaceSpecial;
        }
    }

    private void RoadPlacementHandler()
    {
        if (ToggleButton(ActionType.RoadPlacement))
        {
            inputManager.OnMouseClick += roadManager.PlaceRoad;
            inputManager.OnMouseHold += roadManager.PlaceRoad;
            inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
        }
    }

    private void DestroyHandler()
    {
        if (ToggleButton(ActionType.Destroy))
        {
            inputManager.OnMouseClick += structureManager.DestroyStructure;
            inputManager.OnMouseHold += structureManager.DestroyStructure;
        }
    }

    private void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
        currentAction = ActionType.None;
    }

    private bool ToggleButton(ActionType action)
    {
        if (currentAction == action)
        {
            ClearInputActions();
            uiController.ResetButtonColor();
            return false;
        }
        ClearInputActions();
        currentAction = action;
        return true;
    }

    public void SaveGame()
    {
        SaveDataSerialization saveData = new SaveDataSerialization();
        saveData.mGold = structureManager.resourceManager.mGold;
        saveData.rupiah = structureManager.resourceManager.rupiah;
        int structureSaved = 0;
        foreach (var structuresData in structureManager.GetAllStructure())
        {
            saveData.AddStructureData(structuresData.Key, structuresData.Value.PrefabIndex, structuresData.Value.StructureType);
            structureSaved++;
        }
        var convertToJson = JsonUtility.ToJson(saveData);
        Debug.Log(convertToJson);
        Debug.Log(structureSaved);
        saveSystem.SaveData(convertToJson);
    }

    public void LoadGame()
    {
        var jsonData = saveSystem.LoadData();
        if(String.IsNullOrEmpty(jsonData))
        {
            Debug.Log("No data found, making new save");
            structureManager.resourceManager.mGold = 1000000;
            structureManager.resourceManager.rupiah = 100000000;
            structureManager.resourceManager.UpdateResourceUI();

            SaveGame();
            return;
        }
        SaveDataSerialization saveData = JsonUtility.FromJson<SaveDataSerialization>(jsonData);
        int structureLoaded = 0;
        structureManager.placementManager.ClearGrid();
        foreach (var structuresData in saveData.structuresData)
        {
            Vector3Int position = Vector3Int.RoundToInt(structuresData.position.GetValue());
            if (structuresData.buildingType == CellType.Road)
            {
                roadManager.PlaceRoad(position);
                roadManager.FinishPlacingRoad();
            }else
            {
                structureManager.PlaceLoadedStructure(position, structuresData.prefabIndex, structuresData.buildingType);
            }
            
            structureLoaded++;
        }
        structureManager.resourceManager.mGold = saveData.mGold;
        structureManager.resourceManager.rupiah = saveData.rupiah;
        structureManager.resourceManager.UpdateResourceUI();
        Debug.Log("Structures Loaded = "+ structureLoaded);
        Debug.Log("mgold:" + saveData.mGold + " rupiah:" + saveData.rupiah);

    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x,0, inputManager.CameraMovementVector.y));
    }

    private void OnDestroy()
    {
        inputManager.OnRightMouseHold -= cameraMovement.DragCamera;
    }
}
