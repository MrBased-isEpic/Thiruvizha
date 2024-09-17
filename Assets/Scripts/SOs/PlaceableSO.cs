using System.Collections;
using System.Collections.Generic;
using Thiruvizha.Grids;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[System.Serializable]
public class PlaceableSO : ScriptableObject
{
    public bool[] canBuildingBePlacedFlat = new bool[100];
    public List<Transform> buildingTransforms = new List<Transform>();

    public void SetData(Tilemap tilemap) // Called by the editor tool to save the tilemap
    {
        canBuildingBePlacedFlat = new bool[100];
        buildingTransforms = new List<Transform>();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GroundRuleTile tile = tilemap.GetTile<GroundRuleTile>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    canBuildingBePlacedFlat[x * 10 + y] = tile.canBuildingBePlaced;
                }
            }
        }


        foreach (Transform building in tilemap.transform)
        {
            buildingTransforms.Add(building.transform);
            //buildingTilesSOs.Add(building.GetComponent<BaseBuilding>().buildingTilesSO);
            //positions.Add(new Vector3Int((int)building.transform.position.x, 0, (int)building.transform.position.z));
        }

        EditorUtility.SetDirty(this);
    }
    public void LoadData(Tilemap tilemap, GroundRuleTile GreenTile, GroundRuleTile RedTile) // Called by the editor tool to load the array to the tilemap
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                tilemap.SetTile(new Vector3Int(x,y,0), canBuildingBePlacedFlat[x * 10 + y] ? GreenTile : RedTile);
            }
        }
    }
}

