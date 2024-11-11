using System;
using UnityEngine;
using UnityEngine.UI;

namespace Thiruvizha.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        // UI Elements
        [SerializeField] private PaletteUI Palette;
        [SerializeField] private RotatorUI Rotator;
        [SerializeField] private Button PaletteButton;
        [SerializeField] private Button ClosePaletteButton;

        //Events
        public Action OnShopOpen;
        public Action OnItemPicked;
        public Action OnRotateBuildingTimer;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            PaletteButton.onClick.AddListener(() => { OpenPalette(); });
            ClosePaletteButton.onClick.AddListener(() => { ClosePalette(); });
            Palette.OnItemDragDetected += ClosePalette;
            Rotator.OnRotateTimer += OnRotateTimer;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Palette.gameObject.SetActive(false);
            PaletteButton.gameObject.SetActive(true);
            Rotator.gameObject.SetActive(false);
        }
        private void OpenPalette()
        {
            Palette.gameObject.SetActive(true);
            PaletteButton.gameObject.SetActive(false);
            OnShopOpen?.Invoke();
        }
        private void ClosePalette()
        {
            Palette.gameObject.SetActive(false);
            PaletteButton.gameObject.SetActive(true);
            OnItemPicked?.Invoke();
        }
        public void TurnOnRotator()
        {
            Rotator.gameObject.SetActive(true);
        }
        public void TurnOffRotator()
        {
            Rotator.gameObject.SetActive(false);
        }
        private void OnRotateTimer()
        {
            OnRotateBuildingTimer?.Invoke();
        }
    }
}
