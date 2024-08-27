using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Thiruvizha.Grids
{
    public class GridManager : MonoBehaviour
    {
        private Grid grid;
        public Transform PlacePlane;
        public Transform BaseTile;
        public Transform TwosizeBuilding;
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
            for (int i = 0; i < 10; i++)// Ten times on the x
            {
                for (int j = 0; j < 10; j++)// Ten times on the y
                {  
                    BaseTile tile = Instantiate(BaseTile, grid.CellToWorld(new Vector3Int(i, 0, j)), Quaternion.identity).gameObject.GetComponent<BaseTile>();          
                    mapTiles[i,j] = tile;
                }
            }
            BaseBuilding twoSizeBuilding = Instantiate(TwosizeBuilding).GetComponent<BaseBuilding>();
            PlaceBaseBuilding(twoSizeBuilding, new Vector3Int(5, 0, 5));
            BaseBuilding baseBuilding = Instantiate(BaseBuilding).GetComponent<BaseBuilding>();
            PlaceBaseBuilding(baseBuilding, new Vector3Int(5, 0, 7));
        }

        public void PlaceBaseBuilding(BaseBuilding building)
        {

            Vector3Int targetPos =  grid.WorldToCell(building.transform.position);
            BaseTile targetTile = mapTiles[targetPos.x, targetPos.z];

            if (building.gridPosition != Vector2Int.zero)// Check if it was assigned
            {
                mapTiles[building.gridPosition.x, building.gridPosition.y] = targetTile;
                targetTile.transform.position = grid.CellToWorld(new Vector3Int(building.gridPosition.x, 0, building.gridPosition.y));
            }
            else// If it wasn't assigned yet..
            {
                Destroy(targetTile.gameObject);
            }

            mapTiles[targetPos.x, targetPos.z] = building;
            building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);
            building.transform.position = grid.CellToWorld(targetPos);

            if (building.buildingTilesSO == null) return;// This would mean that the building is single tiled one, so there is no shape.

            foreach (Vector2Int target in building.buildingTilesSO.ShapeTiles)
            {
                targetTile = mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y];
                mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y] = building;
                Destroy(targetTile.gameObject);
            }

        }

        public void PlaceBaseBuilding(BaseBuilding building, Vector3Int targetPos)
        {
            BaseTile targetTile = mapTiles[targetPos.x, targetPos.z];

            Destroy(targetTile.gameObject);

            mapTiles[targetPos.x, targetPos.z] = building;
            building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);
            building.transform.position = grid.CellToWorld(targetPos);


            if (building.buildingTilesSO == null) return;// This would mean that the building is single tiled one, so there is no shape.

            foreach (Vector2Int target in building.buildingTilesSO.ShapeTiles)
            {
                targetTile = mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y];
                mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y] = building;
                Destroy(targetTile.gameObject);
            }
        }

        public bool CheckBuildingPositionisValid(BaseBuilding building)
        {
            Vector3Int targetCenterPos = grid.WorldToCell(building.transform.position);
            PlacePlane.transform.position = grid.CellToWorld(targetCenterPos);
            if (mapTiles[targetCenterPos.x, targetCenterPos.z] as BaseBuilding != null) return false;// The center position of the baseBuilding is already occupied by another basebuilding.

            if(building.buildingTilesSO == null) return true;

            foreach (Vector2Int target in building.buildingTilesSO.ShapeTiles)
            {
                if (mapTiles[targetCenterPos.x + target.x, targetCenterPos.y + target.y] as BaseBuilding != null)// One of the tiles that form the shape is occupied by another basebuilding.
                    return false;
            }
            return true;
        }
    }
}
