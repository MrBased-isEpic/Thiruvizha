using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    public class BaseBuilding : BaseTile, IBaseBuilding
    {
        public enum BuildingType
        {
            shop,
            ride
        }
        private bool isinValidPosition;

        public Vector2Int gridPosition = Vector2Int.zero;
        public BuildingTilesSO buildingTilesSO;

        public void Interact(NPCStateContext npc)
        {
            if (buildingTilesSO.buildingType == BuildingType.shop)
            {
                npc.RegenerateEnergy();
            }
        }
    }
}
