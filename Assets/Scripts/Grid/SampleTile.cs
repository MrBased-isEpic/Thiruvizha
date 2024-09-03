using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SampleTile : MonoBehaviour
{
    public Tilemap tilemap;

    public Transform plane;

    public Vector3Int position;
    public Vector3Int position2;

    TileData tile;

    private void Start()
    {
        TileBase greentile = tilemap.GetTile(position);
        TileBase redtile = tilemap.GetTile(position2);

        greentile.GetTileData(position, tilemap, ref tile);

        if (greentile.name == "texture_02")
        { 
            Debug.Log("found");
        }
        else
        {
            Debug.Log("Null");
        }
    }
}
