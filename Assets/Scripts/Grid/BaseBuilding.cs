using System;
using UnityEngine;

namespace Thiruvizha.Grids
{
    public class BaseBuilding : BaseTile, IBaseBuilding
    {
        public Vector2Int gridPosition;
        public float gridOrientation = 0;
        public BuildingTilesSO buildingTilesSO;
        public Transform ShapeTransforms;

        public Action OnRotation;

        public float lastValidOrientation = 0;

        public enum BuildingType
        {
            shop,
            ride,
            activity
        }
        private bool isinValidPosition;


        [SerializeField] private BaseBuildingUI UI;


        private void OnEnable()
        {
            canBuildingBePlaced = false;
        }
        public void Interact(Thiruvizha.NPC.NPCStateContext npc)
        {
            npc.RegenerateEnergy();
        }

        public void Start()
        {
            UI.OnRotateButtonPressed += ()=> 
            {
                RotateClockWise();
                OnRotation?.Invoke();
            };
        }

        //UI Functions
        public void TurnOnArrow()
        {
            UI.ShowArrow();
        }
        public void TurnOffArrow()
        {
            UI.HideArrow();
        }
        public void TurnOnRotator()
        {
            UI.ShowRotator();
        }
        public void TurnOffRotator()
        {
            UI.HideRotator();
        }
        public void SetArrowFill(float timer, float maxTimer)
        {
            UI.SetArrowFill(timer, maxTimer);
        }

        //Rotation Functions
        public void RotateClockWise()
        {
            if (transform.rotation.eulerAngles.y >= 270)
            {
                transform.rotation = Quaternion.identity;
                Debug.Log("Building Reset");
            }
            else
                transform.Rotate(new Vector3(0, 90, 0));
            Debug.Log("GameObject Rotated");
            UI.SetOrientation();
        }
        public void RotateY(float y)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);
            UI.SetOrientation();
        }
    }
}
