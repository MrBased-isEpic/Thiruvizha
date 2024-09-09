using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    public class BaseBuilding : BaseTile, IBaseBuilding
    {
        private void OnEnable()
        {
            canBuildingBePlaced = false;
        }

        public Vector2Int gridPosition;
        public BuildingTilesSO buildingTilesSO;
        public enum BuildingType
        {
            shop,
            ride
        }
        private bool isinValidPosition;

        public void Interact(Thiruvizha.NPC.NPCStateContext npc)
        {
            switch(buildingTilesSO.buildingType)
            {
                case BuildingType.shop:
                    npc.RegenerateEnergy();
                    break;
                case BuildingType.ride:
                    npc.SpendEnergy();
                    break;
            }
        }
    }
}
