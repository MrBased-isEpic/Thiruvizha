using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Thiruvizha.UI
{
    public class RotatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private float timer;
        [SerializeField] private float RotateTime;

        bool isTimerOn = false;

        public Action OnRotateTimer;


        public void OnPointerEnter(PointerEventData eventData)
        {
            timer = RotateTime;
            isTimerOn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isTimerOn = false;
        }

        private void Update()
        {
            if (isTimerOn)
            {
                timer -= Time.deltaTime;
                if(timer < 0)
                {
                    OnRotateTimer?.Invoke();
                    timer = RotateTime;
                }
            }
        }
    }
}
