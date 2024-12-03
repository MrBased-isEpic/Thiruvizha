using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Thiruvizha.Grids;


    [CreateAssetMenu]
    [System.Serializable]
    public class PlaceableSO : ScriptableObject
    {
        public bool[] canBuildingBePlacedFlat;
        public List<Transform> buildingTransforms;
        public List<Vector3> positions;
        public List<float> yRotations;

    public void SetData(Tilemap tilemap) // Called by the editor tool to save the tilemap
        {
            canBuildingBePlacedFlat = new bool[GridManager.worldLength * GridManager.worldLength];
            buildingTransforms = new List<Transform>();
            positions = new List<Vector3>();

            for (int x = 0; x < GridManager.worldLength; x++)
            {
                for (int y = 0; y < GridManager.worldLength; y++)
                {
                    GroundRuleTile tile = tilemap.GetTile<GroundRuleTile>(new Vector3Int(x, y, 0));
                    if (tile != null)
                    {
                        canBuildingBePlacedFlat[x * GridManager.worldLength + y] = tile.canBuildingBePlaced;
                    }
                }
            }


            foreach (Transform building in tilemap.transform)
            {
                BuildingTilesSO buildingSO = building.GetComponent<BaseBuilding>().buildingTilesSO;
                buildingTransforms.Add(buildingSO.prefab);
                positions.Add(building.position);
                yRotations.Add(building.rotation.eulerAngles.y);
            }

            EditorUtility.SetDirty(this);
        }
        public void LoadData(Tilemap tilemap, GroundRuleTile GreenTile, GroundRuleTile RedTile) // Called by the editor tool to load the array to the tilemap
        {
            for (int x = 0; x < GridManager.worldLength; x++)
            {
                for (int y = 0; y < GridManager.worldLength; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), canBuildingBePlacedFlat[x * 10 + y] ? GreenTile : RedTile);
                }
            }
        }
    }


