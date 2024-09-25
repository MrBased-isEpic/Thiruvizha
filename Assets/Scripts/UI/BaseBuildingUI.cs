using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseBuildingUI : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] private Image GreenArrow;
    [SerializeField] private Transform PickUpArrow;

    [SerializeField] private Button RotationButton;

    public Action OnRotateButtonPressed;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        SetOrientation();
        RotationButton.onClick.AddListener(() => { OnRotateButtonPressed?.Invoke(); });
        HideArrow();
        HideRotator();
    }

    public void ShowArrow()
    {
        PickUpArrow.gameObject.SetActive(true);
    }
    public void HideArrow()
    {
        PickUpArrow.gameObject.SetActive(false);
    }
    public void ShowRotator()
    {
        RotationButton.gameObject.SetActive(true);
    }
    public void HideRotator()
    {
        RotationButton.gameObject.SetActive(false);
    }

    public void SetArrowFill(float timer, float Maxtimer)
    {
        GreenArrow.fillAmount = timer/Maxtimer;
    }
    public void ResetArrowFill()
    {
        GreenArrow.fillAmount = 0;
    }
    public void SetOrientation()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
