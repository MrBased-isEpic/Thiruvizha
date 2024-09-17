using Thiruvizha.Player;
using Thiruvizha.Grids;
using Thiruvizha.UI;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] private PlayerStateContext playerStateContext;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        UIManager.instance.OnItemPicked += GivePlayerABuilding;
        UIManager.instance.OnShopOpen += OpenShop;
    }

    private void OpenShop()
    {
        playerStateContext.SwitchState(PlayerStateContext.PlayerState.ui);
    }

    private void GivePlayerABuilding()
    {
        BaseBuilding building = Instantiate(GridManager.instance.BaseBuilding).GetComponent<BaseBuilding>();
        playerStateContext.selectedBuilding = building;
        playerStateContext.SwitchState(PlayerStateContext.PlayerState.move);
    }

}
