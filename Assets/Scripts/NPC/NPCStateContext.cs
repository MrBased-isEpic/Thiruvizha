using System.Collections.Generic;
using Thiruvizha.Grids;
using UnityEngine;
using UnityEngine.AI;
using System;


namespace Thiruvizha.NPC
{
    public class NPCStateContext : MonoBehaviour
    {
        public float energy;
        private float _energy
        {
            get { return energy; }
            set
            {
                energy = value;
                energy = Mathf.Clamp(value, -1, 1);
                if (energy < -.7f)
                {
                    OnEnergyLow?.Invoke();
                }
                else if (energy >= .7f)
                {
                    OnEnergyHigh?.Invoke();
                }
                else if (energy < .5f)
                {
                    OnEnergyNeutral?.Invoke();
                }
            }
        }

        [SerializeField] private MeshRenderer meshRenderer;

        //Events
        private Action OnEnergyLow;
        private Action OnEnergyNeutral;
        private Action OnEnergyHigh;

        public event EventHandler OnExitReached;

        public List<BaseBuilding.BuildingType> searchForTypes = new List<BaseBuilding.BuildingType>();
        private NavMeshAgent agent;

        //Search variables
        public LayerMask BaseBuildingLM;
        private BaseBuilding targetBuilding;

        public float viewAngle;
        public float viewRadius;
        private float startingYAngle;
        private float turnAngle = 0;

        //Idle variables
        public float idleSpeed;

        public enum NPCState
        {
            idle,
            searching,
            moving,
            interacting
        }
        public NPCState state;
        public NPCSpawner.Type destination;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            state = NPCState.idle;
        }

        private void OnEnable()
        {
            SwitchState(NPCState.idle); 
        }

        private void Start()
        {
            OnEnergyHigh += OnEnergyMaxThresholdReached;
            OnEnergyLow += OnEnergyMinThresholdReached;
            OnEnergyNeutral += OnEnergyNeuThresholdReached;
        }

        private void OnEnergyNeuThresholdReached()
        {
            //if (searchForTypes.Contains(BaseBuilding.BuildingType.shop))
            //{
            //    searchForTypes.Remove(BaseBuilding.BuildingType.shop);
            //}
            //if (searchForTypes.Contains(BaseBuilding.BuildingType.ride))
            //    searchForTypes.Remove(BaseBuilding.BuildingType.ride);
        }

        private void OnEnergyMinThresholdReached()
        {
            //if (!searchForTypes.Contains(BaseBuilding.BuildingType.shop))
            //{
            //    searchForTypes.Add(BaseBuilding.BuildingType.shop);
            //}
            switch (state)
            {
                case NPCState.idle:
                    SwitchState(NPCState.searching);
                    break;
            }
        }

        private void OnEnergyMaxThresholdReached()
        {
            //if (!searchForTypes.Contains(BaseBuilding.BuildingType.ride))
            //{
            //    searchForTypes.Add(BaseBuilding.BuildingType.ride);
            //}
            switch (state)
            {
                case NPCState.interacting:
                    SwitchState(NPCState.idle);
                    break;
                //case NPCState.idle:
                //    SwitchState(NPCState.searching);
                //    break;
            }
        }

        private void FixedUpdate()
        {
            switch (state)
            {
                case NPCState.idle:
                    _energy -= (float)(Time.deltaTime * .01);

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        OnExitReached?.Invoke(this, EventArgs.Empty);
                    }
                    break;


                case NPCState.searching:
                    if (turnAngle > 90)
                    {
                        BaseBuilding[] buildings = GetBuildingsInRange();
                        SearchForBuildings(searchForTypes);
                        turnAngle = 0;
                        startingYAngle = transform.rotation.eulerAngles.y;
                    }

                    transform.Rotate(new Vector3(0, 1, 0));
                    turnAngle = Mathf.Abs(transform.rotation.eulerAngles.y - startingYAngle);
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

        public void ChangeColor()
        {
            switch(destination)
            {
                case NPCSpawner.Type.Temple:
                    meshRenderer.material = NPCManager.instance.RedMat;
                    break;
                case NPCSpawner.Type.Pond:
                    meshRenderer.material = NPCManager.instance.PurpleMat;
                    break;
                case NPCSpawner.Type.Entrance:
                    meshRenderer.material = NPCManager.instance.OrangeMat;
                    break;

            }
        }
        private void SearchForBuildings(List<BaseBuilding.BuildingType> buildingTypes)
        {
            BaseBuilding[] buildings = GetBuildingsInRange();

            foreach (BaseBuilding building in buildings) // This is the list of buildings from GetBuildingsInRange().
            {
                if (building == null)
                {
                    continue;
                }
                foreach (BaseBuilding.BuildingType type in searchForTypes) // For every type of building we are looking for.
                {
                    if (building.buildingTilesSO.buildingType == type) // If the building is of that type,
                    {
                        if (Vector3.Angle(transform.forward, building.transform.position - transform.position) <= viewAngle) // If the building within the visible range,
                        {
                            agent.SetDestination(building.transform.position);
                            targetBuilding = building;
                            SwitchState(NPCState.moving);
                        }
                    }
                }
            }
            //Debug.Log("Not Found");
        }
        private void SwitchState(NPCState state)
        {
            this.state = state;

            switch (this.state)
            {
                case NPCState.idle:
                    agent.speed = .6f;
                    agent.SetDestination(NPCManager.instance.GetDestination(destination).position);
                    break;
                case NPCState.searching:
                    agent.SetDestination(transform.position);
                    turnAngle = 0;
                    startingYAngle = transform.rotation.eulerAngles.y;
                    break;
                case NPCState.moving:
                    agent.speed = 3.5f;
                    break;
            }
        }

        private BaseBuilding[] GetBuildingsInRange()
        {
            Collider[] results = new Collider[10];
            BaseBuilding[] buildings = new BaseBuilding[10];

            //results = Physics.SphereCastAll(transform.position, 100, transform.forward, 100f, BaseBuildingLM);
            if (Physics.OverlapSphereNonAlloc(this.transform.position, viewRadius, results, BaseBuildingLM) > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i] != null)
                    {
                        BaseBuilding building = results[i].gameObject.GetComponentInParent<BaseBuilding>();
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

        public void SetDestination(NPCSpawner.Type destination)
        {
            this.destination = destination;

            searchForTypes.Clear();
            switch(destination)
            {
                case NPCSpawner.Type.Entrance:
                    searchForTypes.Add(BaseBuilding.BuildingType.ride);
                    break;
                case NPCSpawner.Type.Temple:
                    searchForTypes.Add(BaseBuilding.BuildingType.shop);
                    break;
                case NPCSpawner.Type.Pond:
                    searchForTypes.Add(BaseBuilding.BuildingType.activity);
                    break;
            }
            ChangeColor();
        }
        public void InitializeEnergy()
        {
            _energy = UnityEngine.Random.Range(-.65f, -.67f);
        }

    }
}
