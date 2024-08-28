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
            else if (energy >= .7f)
            {
                OnEnergyHigh?.Invoke();
            }
            else if(energy < .5f)
            {
                OnEnergyNeutral?.Invoke();
            }
        }
    }

    //Events
    private Action OnEnergyLow;
    private Action OnEnergyNeutral;
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
    public NPCState state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = NPCState.idle;
    }

    private void Start()
    {
        OnEnergyHigh += OnEnergyMaxThresholdReached;
        OnEnergyLow += OnEnergyMinThresholdReached;
        OnEnergyNeutral += OnEnergyNeuThresholdReached;
    }

    private void OnEnergyNeuThresholdReached()
    {
        if(searchForTypes.Contains(BaseBuilding.BuildingType.shop))
        {
            searchForTypes.Remove(BaseBuilding.BuildingType.shop);
            Debug.Log("Removed shop");
        }
        if(searchForTypes.Contains(BaseBuilding.BuildingType.ride))
            searchForTypes.Remove(BaseBuilding.BuildingType.ride);
    }

    private void OnEnergyMinThresholdReached()
    {
        if (!searchForTypes.Contains(BaseBuilding.BuildingType.shop))
        {
            searchForTypes.Add(BaseBuilding.BuildingType.shop);
            Debug.Log("Shop Added");
        }
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
        Debug.Log("Ride Added");
        if (!searchForTypes.Contains(BaseBuilding.BuildingType.ride))
        {
            searchForTypes.Add(BaseBuilding.BuildingType.ride);
            Debug.Log("Ride Added");
        }
        switch (state)
        {
            case NPCState.interacting:
                SwitchState(NPCState.searching);
                break;
            case NPCState.idle:
                SwitchState(NPCState.searching);
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case NPCState.idle:
                _energy -= (float)(Time.deltaTime * .01);
                break;


            case NPCState.searching:
                BaseBuilding[] buildings = GetBuildingsInRange();
                transform.Rotate(new Vector3(0,1,0));
                SearchForBuildings(searchForTypes);
                break;


            case NPCState.moving:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(NPCState.interacting);
                }
                break;


            case NPCState.interacting:
                targetBuilding.Interact(this);
                break;
        }
    }


    private void SearchForBuildings(List<BaseBuilding.BuildingType> buildingTypes)
    {
        BaseBuilding[] buildings = GetBuildingsInRange();

        int i = 0;
        foreach (BaseBuilding.BuildingType type in searchForTypes)
        {
            Debug.Log(i+":"+type);
            i++;
        }
        foreach (BaseBuilding building in buildings) // This is the list of buildings from GetBuildingsInRange().
        {
            if(building == null) continue;           
            foreach (BaseBuilding.BuildingType type in searchForTypes) // For every type of building we are looking for.
            {
                if (building.buildingTilesSO.buildingType == type) // If the building is of that type,
                {
                    if (Vector3.Angle(transform.forward, building.transform.position - transform.position) <= viewAngle) // If the building within the visible range,
                    {
                        Debug.Log(building +" : "+ type);
                        agent.SetDestination(building.transform.position);
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
                break;
            case NPCState.moving:
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

    public void SpendEnergy()
    {
        _energy -= Time.deltaTime * .4f;
    }

}
