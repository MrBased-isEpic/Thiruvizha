using Thiruvizha.Player;
using Thiruvizha.Grids;
using Thiruvizha.UI;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerStateContext player;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        UIManager.instance.OnItemPicked += GivePlayerABuilding;
        UIManager.instance.OnShopOpen += OpenShop;
        UIManager.instance.OnRotateBuildingTimer += RotatePlayerBuilding;

        player.OnMoveStateEntered += TurnOnPlacableVisuals;
        player.OnMoveStateExited += TurnOffPlacableVisuals;
    }

    private void TurnOnPlacableVisuals()
    {
        UIManager.instance.TurnOnRotator();
    }
    private void TurnOffPlacableVisuals()
    {
        UIManager.instance.TurnOffRotator();
    }

    private void OpenShop()
    {
        player.SwitchState(PlayerStateContext.PlayerState.ui);
    }
    private void GivePlayerABuilding()
    {
        BaseBuilding building = Instantiate(GridManager.instance.BaseBuilding).GetComponent<BaseBuilding>();
        player.selectedBuilding = building;
        player.SwitchState(PlayerStateContext.PlayerState.move);
    }
    private void RotatePlayerBuilding()
    {
        player.RotateCurrentBuilding();
    }
}
