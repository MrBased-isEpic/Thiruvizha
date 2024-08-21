using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Thiruvizha.Grids
{
    public class GridManager : MonoBehaviour
    {
        private Grid grid;
        public Transform BaseTile;
        public Transform BaseBuilding;


        private BaseTile[,] mapTiles;

        public static GridManager instance;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            instance = this;
            InstantiateGrid();
        }

        private void InstantiateGrid()
        {
            mapTiles = new BaseTile[10,10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {  
                    BaseTile tile = Instantiate(BaseTile, grid.GetCellCenterWorld(new Vector3Int(i, 0, j)), Quaternion.identity).gameObject.GetComponent<BaseTile>();          
                    mapTiles[i,j] = tile;
                }
            }
            BaseBuilding baseBuilding = Instantiate(BaseBuilding).GetComponent<BaseBuilding>();
            PlaceBaseBuilding(baseBuilding, new Vector3Int(5, 0, 5));
        }

        public void PlaceBaseBuilding(BaseBuilding building)
        {
            Vector3Int targetPos =  grid.WorldToCell(building.transform.position);
            BaseTile targetTile = mapTiles[targetPos.x, targetPos.z];

            if (building.gridPosition != Vector2Int.zero)
            {
                mapTiles[building.gridPosition.x, building.gridPosition.y] = targetTile;
                targetTile.transform.position = grid.GetCellCenterWorld(new Vector3Int(building.gridPosition.x, 0, building.gridPosition.y));
            }
            else
            {
                Destroy(targetTile.gameObject);
            }

            mapTiles[targetPos.x, targetPos.z] = building;
            building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);
            building.transform.position = grid.GetCellCenterWorld(targetPos);

        }

        public void PlaceBaseBuilding(BaseBuilding building, Vector3Int targetPos)
        {
            BaseTile targetTile = mapTiles[targetPos.x, targetPos.z];

            Destroy(targetTile.gameObject);

            mapTiles[targetPos.x, targetPos.z] = building;
            building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);
            building.transform.position = targetPos;
        }
    }
}
