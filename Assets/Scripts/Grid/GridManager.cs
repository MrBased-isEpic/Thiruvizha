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

        [SerializeField] private PlaceableSO placeableSO;
        [SerializeField] private NormalSO normalSO;

        private BaseTile[,] mapTiles;

        public static GridManager instance;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            instance = this;
        }

        private void Start()
        {
            InstantiateGrid();
        }

        private void InstantiateGrid()
        {

            mapTiles = new BaseTile[10,10];
            for (int x = 0; x < 10; x++)// Ten times on the x
            {
                for (int y = 0; y < 10; y++)// Ten times on the y
                {  
                    BaseTile tile = Instantiate(BaseTile, grid.CellToWorld(new Vector3Int(x, 0, y)), Quaternion.identity).gameObject.GetComponent<BaseTile>();          
                    mapTiles[x,y] = tile;
                    tile.canBuildingBePlaced = placeableSO.canBuildingBePlacedFlat[x * 10 + y];
                    Debug.Log(placeableSO.canBuildingBePlacedFlat[x * 10 + y] +" : "+ tile.canBuildingBePlaced);
                }
            }
            //BaseBuilding twoSizeBuilding = Instantiate(TwosizeBuilding).GetComponent<BaseBuilding>();
            //PlaceBaseBuilding(twoSizeBuilding, new Vector3Int(5, 0, 5));
            BaseBuilding baseBuilding = Instantiate(BaseBuilding).GetComponent<BaseBuilding>();
            PlaceBaseBuilding(baseBuilding, new Vector3Int(5, 0, 5));
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

            if (targetTile != null)
            {
                Destroy(targetTile.gameObject);
            }
            else
            {

                Debug.Log("This Ran");
            }

            mapTiles[targetPos.x, targetPos.z] = building;
            building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);
            building.transform.position = grid.CellToWorld(targetPos);


            if (building.buildingTilesSO == null) return;// This would mean that the building is single tiled one, so there is no shape.

            foreach (Vector2Int target in building.buildingTilesSO.ShapeTiles)
            {
                targetTile = mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y];
                if (targetTile != null)
                {
                    Destroy(targetTile.gameObject);
                    mapTiles[building.gridPosition.x + target.x, building.gridPosition.y + target.y] = building;
                }
            }
        }

        public bool CheckBuildingPositionisValid(BaseBuilding building)
        {
            Vector3Int targetCenterPos = grid.WorldToCell(building.transform.position);

            if((targetCenterPos.x < 0 || targetCenterPos.x >= 10/*mapTiles.GetLength(0))*/ || (targetCenterPos.z < 0 || targetCenterPos.z >= 10/*mapTiles.GetLength(1)*/))) return false;

            PlacePlane.transform.position = grid.CellToWorld(targetCenterPos);

            if (mapTiles[targetCenterPos.x, targetCenterPos.z] as BaseBuilding != null) return false;// The center position of the baseBuilding is already occupied by another basebuilding.

            if (!mapTiles[targetCenterPos.x, targetCenterPos.z].canBuildingBePlaced)
            {
                Debug.Log("Tile is False");
                return false;
            }

            if(building.buildingTilesSO == null) return true;

            foreach (Vector2Int target in building.buildingTilesSO.ShapeTiles)
            {
                if (!mapTiles[targetCenterPos.x, targetCenterPos.z].canBuildingBePlaced) return false;
                if (mapTiles[targetCenterPos.x + target.x, targetCenterPos.y + target.y] as BaseBuilding != null)// One of the tiles that form the shape is occupied by another basebuilding.
                    return false;
            }
            return true;
        }
    }
}
