using Thiruvizha.Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace Thiruvizha.Grids
{
    public class GridManager : MonoBehaviour
    {
        private Grid grid;
        public Transform PlacePlane;
        public Transform BaseTile;
        public Transform TwosizeBuilding;
        public Transform BaseBuilding;
        

        public NPC.NPCSpawner spawner;
        private PlayerStateContext player;

        [SerializeField] private PlaceableSO placeableSO;

        private BaseTile[,] mapTiles;

        public static int worldLength = 15;
        public static GridManager instance;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            instance = this;

            player = GameManager.instance.player;
        }

        private void Start()
        {
            player.OnMoveStateEntered += TurnOnPlacableVisuals;
            player.OnMoveStateExited += TurnOffPlacableVisuals;
            InstantiateGrid();
        }

        private void InstantiateGrid()
        {

            mapTiles = new BaseTile[worldLength,worldLength];
            for (int x = 0; x < worldLength; x++)// Ten times on the x
            {
                for (int y = 0; y < worldLength; y++)// Ten times on the y
                {  
                    BaseTile tile = Instantiate(BaseTile, grid.CellToWorld(new Vector3Int(x, 0, y)), Quaternion.identity,this.transform).gameObject.GetComponent<BaseTile>();          
                    mapTiles[x,y] = tile;
                    tile.canBuildingBePlaced = placeableSO.canBuildingBePlacedFlat[x * worldLength + y];
                    //Debug.Log(placeableSO.canBuildingBePlacedFlat[x * 10 + y] +" : "+ tile.canBuildingBePlaced);
                }
            }

            Debug.Log(placeableSO.positions.Count);
            for(int i = 0; i < placeableSO.positions.Count; i++)
            {
                BaseBuilding baseBuilding = Instantiate(placeableSO.buildingTransforms[i]).GetComponent<BaseBuilding>();
                baseBuilding.transform.rotation = Quaternion.Euler(0, placeableSO.yRotations[i],0);
                PlaceBaseBuilding(baseBuilding, grid.WorldToCell(placeableSO.positions[i]));
            }
        }

        public void PlaceBaseBuilding(BaseBuilding building)
        {
            Vector3Int centerPos = grid.WorldToCell(building.transform.position);
            building.transform.position = grid.CellToWorld(centerPos);

            PlaceBaseTile(building);

            RemoveBaseTiles(building);

            building.gridPosition = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z);

            building.gridOrientation = building.transform.rotation.eulerAngles.y;

            building.OnBuildingPlaced?.Invoke();
        }
        public void PlaceBaseBuilding(BaseBuilding building, Vector3Int targetPos)
        {
            BaseTile targetTile = mapTiles[targetPos.x, targetPos.z];
            bool previouslyPlaced = false;

            if (building.gridPosition != new Vector2Int(targetPos.x, targetPos.z))// Check if building was not already there.
            {
                mapTiles[targetPos.x, targetPos.z] = building;
                building.gridPosition = new Vector2Int(targetPos.x, targetPos.z);

                if (targetTile != null)
                {
                    Destroy(targetTile.gameObject);
                }
            }
            else previouslyPlaced = true;

            building.transform.position = grid.CellToWorld(targetPos);


            //if (building.buildingTilesSO == null) return;// This would mean that the building is single tiled one, so there is no shape.

            if(previouslyPlaced) return;

            foreach (Transform target in building.ShapeTransforms)
            {
                targetTile = mapTiles[(int)target.position.x, (int)target.position.z];
                if (targetTile != null)
                {
                    Destroy(targetTile.gameObject);
                    mapTiles[(int)target.position.x, (int)target.position.y] = building;
                }
            }
        }
        public bool CheckBuildingPositionisValid(BaseBuilding building)
        {
            Vector3Int targetCenterPos = grid.WorldToCell(building.transform.position);

            if((targetCenterPos.x < 0 || targetCenterPos.x >= worldLength || (targetCenterPos.z < 0 || targetCenterPos.z >= worldLength))) return false;

            PlacePlane.transform.position = grid.CellToWorld(targetCenterPos);


            BaseTile centerTile = mapTiles[targetCenterPos.x, targetCenterPos.z];
            if (centerTile != null)
            {

                BaseBuilding BuildingAtPos = centerTile as BaseBuilding;
                if (BuildingAtPos != null) // The center position of the baseBuilding is already occupied by another basebuilding.
                {
                    if (BuildingAtPos != building) // Checking if it is the same building lol.
                    {
                        Debug.Log("Building at CenterPosition"); return false;
                    }   
                }
                else
                {
                    if (!centerTile.canBuildingBePlaced)
                    {
                        Debug.Log("BaseTile Rejected"); return false;
                    }
                }
            }


            foreach (Transform target in building.ShapeTransforms)
            {
                if ((target.position.x < 0 || target.position.x >= worldLength || (target.position.z < 0 || target.position.z >= worldLength))) return false;


                Vector3Int CenterPos = grid.WorldToCell(target.position); 
                centerTile = mapTiles[CenterPos.x, CenterPos.z];
                if (centerTile != null)
                {
                    BaseBuilding BuildingAtPos = centerTile as BaseBuilding;
                    if (BuildingAtPos != null) // The center position of the baseBuilding is already occupied by another basebuilding.
                    {
                        if (BuildingAtPos != building) // Checking if it is the same building lol.
                        {
                            Debug.Log("Building at TargetPosition"); return false;
                        }
                    }
                    else
                    {
                        if (!centerTile.canBuildingBePlaced)
                        {
                            Debug.Log("BaseTile Rejected at: "+ centerTile.transform.position); return false;
                        }
                    }
                }
            }
            Debug.Log("Checked ok");
            return true;
        }
        private void PlaceBaseTile(BaseBuilding building)
        {
            Vector3 OGPos = building.transform.position;
            Vector3 OGRot = building.transform.rotation.eulerAngles;
            Vector3Int centerPos = new Vector3Int(building.gridPosition.x,0,building.gridPosition.y);
            Vector3Int currentPos = grid.WorldToCell(building.transform.position);

            BaseBuilding placedBuilding = mapTiles[centerPos.x,centerPos.z] as BaseBuilding;
            BaseTile tile;
            if (centerPos != currentPos)
            {
                tile = Instantiate(BaseTile, centerPos, Quaternion.identity,this.transform).GetComponent<BaseTile>();
                mapTiles[centerPos.x, centerPos.z] = tile;
                tile.canBuildingBePlaced = placeableSO.canBuildingBePlacedFlat[centerPos.x * worldLength + centerPos.z];
            }

            building.transform.position = centerPos;
            building.transform.rotation = Quaternion.Euler(building.transform.rotation.eulerAngles.x, building.gridOrientation, building.transform.rotation.eulerAngles.z);

            foreach (Transform target in building.ShapeTransforms)
            {
                Vector3Int spawnPos = grid.WorldToCell(target.position);

                placedBuilding = mapTiles[spawnPos.x, spawnPos.z] as BaseBuilding;

                tile = Instantiate(BaseTile, spawnPos, Quaternion.identity, this.transform).GetComponent<BaseTile>();
                mapTiles[spawnPos.x, spawnPos.z] = tile;
                tile.canBuildingBePlaced = placeableSO.canBuildingBePlacedFlat[spawnPos.x * worldLength + spawnPos.z]; 

            }

            building.transform.position = OGPos;
            building.transform.rotation = Quaternion.Euler(OGRot);
        }
        private void RemoveBaseTiles(BaseBuilding building)
        {
            Vector3Int centerPos = grid.WorldToCell(building.transform.position);

            Vector3Int buildingGridPos = new Vector3Int(building.gridPosition.x, 0, building.gridPosition.y);

            BaseTile desTile;
            BaseBuilding placedBuilding;

            desTile = mapTiles[centerPos.x, centerPos.z];
            placedBuilding = desTile as BaseBuilding;
            if (desTile != null)
            {
                if (placedBuilding != building)
                {
                    mapTiles[centerPos.x, centerPos.z] = building;
                    Destroy(desTile.gameObject);
                }
            }
            

            foreach (Transform target in building.ShapeTransforms)
            {
                Vector3Int desPos = grid.WorldToCell(target.position);
                desTile = mapTiles[desPos.x, desPos.z];
                if (desTile == null) continue;
                placedBuilding = desTile as BaseBuilding;
                if (placedBuilding != building)
                {
                    desTile = mapTiles[desPos.x, desPos.z];
                    mapTiles[desPos.x, desPos.z] = building;
                    Destroy(desTile.gameObject);
                    Debug.Log("Destroyed");
                }
            }

            building.transform.position = grid.CellToWorld(centerPos);
        }
        private void ClearMapTiles(BaseBuilding building)
        {
            Vector3 OGPos = building.transform.position;
            Vector3 OGRot = building.transform.rotation.eulerAngles;


            building.transform.position = grid.CellToWorld(new Vector3Int(building.gridPosition.x, 0, building.gridPosition.y));
            building.transform.rotation = Quaternion.Euler(building.transform.rotation.eulerAngles.x, building.gridOrientation, building.transform.rotation.eulerAngles.z);

            building.transform.position = OGPos;
            building.transform.rotation = Quaternion.Euler(OGRot);

        }
        public Vector3Int GetWorldToCellPosition(Vector3 position)
        {
            return grid.WorldToCell(position);
        }

        public void TurnOnPlacableVisuals()
        {
            foreach (BaseTile tile in mapTiles)
            {
                tile.TurnOnPlacableVisual();
            }
        }
        public void TurnOffPlacableVisuals()
        {
            foreach (BaseTile tile in mapTiles)
            {
                tile.TurnOffPlacableVisual();
            }
        }
    }
}
