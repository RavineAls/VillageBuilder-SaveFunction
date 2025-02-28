using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] housesPrefabs, specialPrefabs, bigStructuresPrefabs;
    public PlacementManager placementManager;
    public ResourceManager resourceManager;
    public RoadManager roadManager;
    public RoadFixer roadFixer;
    public UIController uiController;

    private float[] houseWeights, specialWeights, bigStructureWeights;

    private void Start()
    {
        houseWeights = housesPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        bigStructureWeights = bigStructuresPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
    }

    public void PlaceHouse(Vector3Int position/*, int index*/)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(houseWeights);
            if(CheckforResource(housesPrefabs[randomIndex].price))
            {
                placementManager.PlaceObjectOnTheMap(position, housesPrefabs[randomIndex].prefab, CellType.Structure, prefabIndex:randomIndex);
                AudioPlayer.instance.PlayPlacementSound();
            }
        }
        
    }

    internal void PlaceBigStructure(Vector3Int position)
    {
        int width = 2;
        int height = 2;
        if(CheckBigStructure(position, width , height))
        {
            int randomIndex = GetRandomWeightedIndex(bigStructureWeights);
            if(CheckforResource(bigStructuresPrefabs[randomIndex].price))
            {
                placementManager.PlaceObjectOnTheMap(position, bigStructuresPrefabs[randomIndex].prefab, CellType.BigStructure, width, height, randomIndex);
                AudioPlayer.instance.PlayPlacementSound();
            }
        }
    }

    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        bool nearRoad = false;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                
                if (DefaultCheck(newPosition)==false)
                {
                    return false;
                }
                if (nearRoad == false)
                {
                    nearRoad = RoadCheck(newPosition);
                }
            }
        }
        if(nearRoad == false)
        {
            uiController.ShowError("Bangunan harus dibangun didekat jalanan");
        }
        return nearRoad;
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            if(CheckforResource(specialPrefabs[randomIndex].price))
            {
                
                placementManager.PlaceObjectOnTheMap(position, specialPrefabs[randomIndex].prefab, CellType.SpecialStructure, prefabIndex:randomIndex);
                AudioPlayer.instance.PlayPlacementSound();
            }
        }
    }

    int skipBig = 0;
    public void PlaceLoadedStructure(Vector3Int position, int prefabIndex, CellType buildingType)
    {
        switch (buildingType)
        {
            case CellType.Structure:
                placementManager.PlaceObjectOnTheMap(position, housesPrefabs[prefabIndex].prefab, CellType.Structure, prefabIndex:prefabIndex);
                break;
            case CellType.BigStructure:
                if(skipBig>0)
                {
                    skipBig--;
                    break;
                }
                placementManager.PlaceObjectOnTheMap(position, bigStructuresPrefabs[prefabIndex].prefab, CellType.BigStructure, 2, 2, prefabIndex);
                skipBig = 3;
                break;
            case CellType.SpecialStructure:
                placementManager.PlaceObjectOnTheMap(position, specialPrefabs[prefabIndex].prefab, CellType.SpecialStructure, prefabIndex:prefabIndex);
                break;
            default:
                break;
        }
    }

    public void DestroyStructure(Vector3Int position)
    {
        if(placementManager.GetCellType(position) == CellType.Road)
        {
            placementManager.DestroyStructureAt(position);
            AudioPlayer.instance.PlayDestroySound();

            List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();
            var neighbours = placementManager.GetNeighboursOfTypeFor(position, CellType.Road);
            foreach (var roadposition in neighbours)
            {
                if (roadPositionsToRecheck.Contains(roadposition)==false)
                {
                    roadPositionsToRecheck.Add(roadposition);
                }
            }
            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, positionToFix);
            }
            return;
        }

        if(placementManager.GetCellType(position) == CellType.Structure)
        {
            resourceManager.mGold += housesPrefabs[0].price;
            resourceManager.UpdateResourceUI();
            AudioPlayer.instance.PlayDestroySound();
        }
        if(placementManager.GetCellType(position) == CellType.BigStructure)
        {
            resourceManager.mGold += bigStructuresPrefabs[0].price;
            resourceManager.UpdateResourceUI();
            AudioPlayer.instance.PlayDestroySound();
        }
        if(placementManager.GetCellType(position) == CellType.SpecialStructure)
        {
            resourceManager.mGold += specialPrefabs[0].price;
            resourceManager.UpdateResourceUI();
            AudioPlayer.instance.PlayDestroySound();
        }
        placementManager.DestroyStructureAt(position);
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            //0->weight[0] weight[0]->weight[1]
            if(randomValue >= tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckforResource(int required)
    {
        if(resourceManager.mGold >= required)
        {
            resourceManager.mGold -= required;
            resourceManager.UpdateResourceUI();
            return true;
        }

        else
        {
            uiController.ShowError("mGold tidak mencukupi");
            return false;
        }
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if (DefaultCheck(position) == false)
        {
            return false;
        }

        if (RoadCheck(position) == false)
        {
            uiController.ShowError("Bangunan harus dibangun didekat jalanan"); 
            return false;
        }
        return true;
    }

    private bool RoadCheck(Vector3Int position)
    {
        if (placementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count <= 0)
        {
            return false;
        }
        return true;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            uiController.ShowError("Posisi ini ada di luar area pembangunan");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            uiController.ShowError("Posisi ini tidak kosong");
            return false;
        }
        return true;
    }

    public Dictionary<Vector3Int, StructureModel> GetAllStructure()
    {
        return placementManager.GetAllStructure();
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject prefab;
    [Range(0,1)]
    public float weight;
    public int price;
}
