using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Thiruvizha.Grids;


    [CreateAssetMenu]
    [System.Serializable]
    public class PlaceableSO : ScriptableObject
    {
        public bool[] canBuildingBePlacedFlat = new bool[100];
        public List<Transform> buildingTransforms = new List<Transform>();
        public List<Vector3> positions = new List<Vector3>();

        public void SetData(Tilemap tilemap) // Called by the editor tool to save the tilemap
        {
            canBuildingBePlacedFlat = new bool[100];
            buildingTransforms = new List<Transform>();
            positions = new List<Vector3>();

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
                BuildingTilesSO buildingSO = building.GetComponent<BaseBuilding>().buildingTilesSO;
                buildingTransforms.Add(buildingSO.prefab);
                positions.Add(building.position);
            }

            EditorUtility.SetDirty(this);
        }
        public void LoadData(Tilemap tilemap, GroundRuleTile GreenTile, GroundRuleTile RedTile) // Called by the editor tool to load the array to the tilemap
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), canBuildingBePlacedFlat[x * 10 + y] ? GreenTile : RedTile);
                }
            }
        }
    }


