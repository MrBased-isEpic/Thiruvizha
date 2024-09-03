using System.Collections;
using System.Collections.Generic;
using Thiruvizha.Grids;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PlaceableSO : ScriptableObject
{
    public bool[] canBuildingBePlacedFlat = new bool[100];

    public void SetData(Tilemap tilemap)
    {
        canBuildingBePlacedFlat = new bool[100];
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
    }

    public void ViewData(Tilemap tilemap, GroundRuleTile GreenTile, GroundRuleTile RedTile)
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
