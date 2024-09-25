using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    [CreateAssetMenu (fileName = "BuildingTilesSO")]
    public class BuildingTilesSO : ScriptableObject
    {
        public Transform prefab;
        public BaseBuilding.BuildingType buildingType;
    }
}
