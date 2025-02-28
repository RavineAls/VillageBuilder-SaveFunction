using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModel : MonoBehaviour
{
    float yHeight = 0;
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    [field: SerializeField]
    public CellType StructureType { get; private set; }

    [field: SerializeField]
    public int PrefabIndex { get; private set; }

    public void CreateModel(GameObject model, int prefabIndex, CellType structureType, int width = 1, int height = 1)
    {
        Width = width;
        Height = height;

        var structure = Instantiate(model, transform);
        yHeight = structure.transform.position.y;
        StructureType = structureType;
        PrefabIndex = prefabIndex;
    }

    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var structure = Instantiate(model, transform);
        structure.transform.localPosition = new Vector3(0, yHeight, 0);
        structure.transform.localRotation = rotation;
    }
}
