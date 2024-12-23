using System.Collections.Generic;
using Thiruvizha.Grids;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.UIElements;


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
                energy = Mathf.Clamp(value, 0, 1);
                if (energy <= 0)
                {
                    OnEnergyLow?.Invoke();
                }
                else if (energy >= 1)
                {
                    OnEnergyHigh?.Invoke();
                }
                else if (energy > .5f)
                {
                    OnEnergyNeutral?.Invoke();
                }
            }
        }

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Transform RayCastOrigin;

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

        public float FOV;
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
            
        }

        private void OnEnergyMinThresholdReached()
        {
            switch (state)
            {
                case NPCState.idle:
                    SwitchState(NPCState.searching);
                    break;
            }
        }

        private void OnEnergyMaxThresholdReached()
        {
            switch (state)
            {
                case NPCState.interacting:
                    BankAcc.instance.SendMoney(1000);
                    SwitchState(NPCState.idle);
                    break;
            }
        }

        private void FixedUpdate()
        {
            switch (state)
            {
                case NPCState.idle:
                    _energy -= (float)(Time.deltaTime * .1f);

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        OnExitReached?.Invoke(this, EventArgs.Empty);
                    }
                    break;


                case NPCState.searching:
                    if (turnAngle > 90)
                    {
                        if (SearchForBuildings(searchForTypes))
                        {
                            SwitchState(NPCState.moving);
                        }
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

                    if(targetBuilding == null)
                    {
                        SwitchState(NPCState.idle);
                        Debug.Log("Building became null : " + searchForTypes[0].ToString());
                        break;
                    }
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
        private bool SearchForBuildings(List<BaseBuilding.BuildingType> buildingTypes)
        {
            BaseBuilding[] buildings = GetBuildingsInRange();

            foreach (BaseBuilding building in buildings) // This is the list of buildings from GetBuildingsInRange().
            {
                if (building == null)
                {
                    continue;
                }
                foreach (BaseBuilding.BuildingType type in buildingTypes) // For every type of building we are looking for,
                {
                    if (building.buildingTilesSO.buildingType == type) // If the building is of that type,
                    {
                        Vector3 dirToBuilding = (RayCastOrigin.position - building.RayCastTarget.position).normalized;

                        if (MathF.Abs((Vector3.Angle(RayCastOrigin.forward, dirToBuilding)) - 180) < FOV / 2) // And if its within the visible range,
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(RayCastOrigin.position, -dirToBuilding, out hit, viewRadius)) // And is not hidden by another building,
                            {
                                if (building == hit.collider.gameObject.GetComponentInParent<BaseBuilding>())
                                {
                                    targetBuilding = building;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private void DebugNpc(string message, BaseBuilding.BuildingType type)
        {
            if (type == BaseBuilding.BuildingType.activity)
                Debug.Log(message + " : " + type.ToString());
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

        private void SwitchState(NPCState state)
        {
            this.state = state;

            switch (this.state)
            {
                case NPCState.idle:


                    if(targetBuilding != null)
                    {
                        targetBuilding.OnBuildingPlaced -= OnBuildingPositionChanged;
                        targetBuilding = null;
                    }
                    agent.speed = .6f;
                    agent.SetDestination(NPCManager.instance.GetDestination(destination).position);
                    break;
                case NPCState.searching:
                    agent.SetDestination(transform.position);
                    turnAngle = 0;
                    startingYAngle = transform.rotation.eulerAngles.y;
                    break;
                case NPCState.moving:

                    if (targetBuilding != null)
                    {
                        agent.SetDestination(targetBuilding.transform.position);
                        targetBuilding.OnBuildingPlaced += OnBuildingPositionChanged;
                    }
                    agent.speed = 3.5f;
                    break;
            }
        }

        private void OnBuildingPositionChanged()
        {
            SwitchState(NPCState.moving);
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
            _energy = UnityEngine.Random.Range(0.5f, 1f);
        }

    }
}
