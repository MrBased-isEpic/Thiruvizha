using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.Grids
{
    public class BaseTile : MonoBehaviour
    {
        public bool canBuildingBePlaced = true;
        [SerializeField] private Transform NotPlacableVisual;

        private void Start()
        {
            TurnOffPlacableVisual();
        }

        public void TurnOnPlacableVisual()
        {
            if (!canBuildingBePlaced)
            {
                if(NotPlacableVisual != null)
                    NotPlacableVisual.gameObject.SetActive(true);
            }
        }
        public void TurnOffPlacableVisual()
        {
            if (NotPlacableVisual != null)
                NotPlacableVisual.gameObject.SetActive(false);
        }
    }
}
