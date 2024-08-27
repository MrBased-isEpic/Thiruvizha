using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Thiruvizha.Grids;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NPCStateContext : MonoBehaviour
{
    public float energy;
    private float _energy
    {
        get {  return energy; }
        set
        {
            energy = value;
            energy = Mathf.Clamp(value, -1,1);
            if (energy < -.7f)
            {
                OnEnergyLow?.Invoke();
            }
            else if (energy > .7f)
            {
                OnEnergyHigh?.Invoke();
            }
        }
    }

    //Events
    private Action OnEnergyLow;
    private Action OnEnergyHigh;


    private List<BaseBuilding.BuildingType> searchForTypes = new List<BaseBuilding.BuildingType>();
    private NavMeshAgent agent;

    //Search variables
    public LayerMask BaseBuildingLM;
    private BaseBuilding targetBuilding;

    public float viewAngle;
    public float viewRadius;
    
    public enum NPCState
    {
        idle,
        searching,
        moving,
        interacting
    }
    private NPCState state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = NPCState.idle;
    }

    private void Start()
    {
        OnEnergyHigh += OnEnergyMinThresholdReached;
        OnEnergyLow += OnEnergyMinThresholdReached;
    }

    private void OnEnergyMinThresholdReached()
    {
        searchForTypes.Add(BaseBuilding.BuildingType.shop);
        Debug.Log("Added");
        searchForTypes.Remove(BaseBuilding.BuildingType.ride);
        switch (state)
        {
            case NPCState.idle:
                SwitchState(NPCState.searching);
                break;
            case NPCState.interacting:
                SwitchState(NPCState.idle);
                break;
            case NPCState.moving:
                SwitchState(NPCState.searching);
                break;
        }
    }

    private void OnEnergyMaxThresholdReached()
    { 
        searchForTypes.Add(BaseBuilding.BuildingType.ride);
        switch (state)
        {
            case NPCState.interacting:
                SwitchState(NPCState.idle);
                searchForTypes.Remove(BaseBuilding.BuildingType.shop);
                Debug.Log("Removed");
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case NPCState.idle:
                _energy -= (float)(Time.deltaTime * .1);
                break;


            case NPCState.searching:
                //_energy -= (float)(Time.deltaTime * .1);
                BaseBuilding[] buildings = GetBuildingsInRange();
                SearchForBuildings(searchForTypes);
                break;


            case NPCState.moving:
                //_energy -= (float)(Time.deltaTime * .1);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(NPCState.interacting);
                }
                break;


            case NPCState.interacting:
                targetBuilding.Interact(this);
                if (_energy > 0.7)
                {
                    SwitchState(NPCState.idle);
                }
                break;
        }
    }


    private void SearchForBuildings(List<BaseBuilding.BuildingType> buildingTypes)
    {
        BaseBuilding[] buildings = GetBuildingsInRange();

        foreach (BaseBuilding building in buildings)
        {
            foreach (BaseBuilding.BuildingType type in searchForTypes)
            {
                if (building.buildingTilesSO.buildingType == type)
                {
                    if (Vector3.Angle(transform.forward, building.transform.position - transform.position) <= viewAngle)
                    {
                        agent.SetDestination(targetBuilding.transform.position);
                        targetBuilding = building;
                        SwitchState(NPCState.moving);
                    }
                }
            }
        }
    }
    private void SwitchState(NPCState state)
    {
        this.state = state;

        switch (this.state)
        {
            case NPCState.searching:
                Debug.Log("Npc is now Searching");
                break;
            case NPCState.moving:
                Debug.Log("Npc is now Moving");
                break;
        }
    }

    private BaseBuilding[] GetBuildingsInRange()
    {
        Collider[] results = new Collider[10];
        BaseBuilding[] buildings = new BaseBuilding[10];
        
        //results = Physics.SphereCastAll(transform.position, 100, transform.forward, 100f, BaseBuildingLM);
        if(Physics.OverlapSphereNonAlloc(this.transform.position, viewRadius, results, BaseBuildingLM) > 0)
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null)
                {
                    BaseBuilding building = results[i].gameObject.GetComponent<BaseBuilding>();
                    buildings[i] = building;
                }
            }
        }
        return buildings;
    }

    public void RegenerateEnergy()
    {
        _energy += Time.deltaTime * .2f;
    }

}
