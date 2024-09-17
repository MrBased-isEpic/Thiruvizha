using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    [CreateAssetMenu (fileName = "BuildingTilesSO")]
    public class BuildingTilesSO : ScriptableObject
    {
        public BaseBuilding.BuildingType buildingType;
        public Vector2Int[] ShapeTiles;
    }
}
