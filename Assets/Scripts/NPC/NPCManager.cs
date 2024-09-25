using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thiruvizha.NPC
{
    public class NPCManager : MonoBehaviour
    {
        public static NPCManager instance;
        public Transform NPC;

        [SerializeField] private NPCSpawner[] spawners;
        public int MaxNpcs;

        [SerializeField] float SwitchInterval;
        private float timer;

        public Material PurpleMat;
        public Material OrangeMat;
        public Material RedMat;

        private void Awake()
        {
            instance = this;
            timer = SwitchInterval;
        }

        private void Start()
        {
            InitializeSpawners();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                SwitchSpawners();
                timer = SwitchInterval;
            }
        }

        private void InitializeSpawners()
        {
            foreach (NPCSpawner spawner in spawners)
            {
                spawner.Initialize();
                spawner.EnableSpawn();
            }

            int i = Random.Range(0, spawners.Length);
            spawners[i].DisableSpawn();
        }
        private void SwitchSpawners()
        {
            List<NPCSpawner> turnedOnSpawners = new List<NPCSpawner>();
            NPCSpawner turnedOffSpawner = null;
            foreach (NPCSpawner spawner in spawners)
            {
                if (!spawner.CanSpawn())
                {
                    turnedOffSpawner = spawner;
                    continue;
                }
                turnedOnSpawners.Add(spawner);
            }

            int i = Random.Range(0, turnedOnSpawners.Count);

            turnedOnSpawners[i].DisableSpawn();
            turnedOffSpawner.EnableSpawn();
        }
        public Transform GetDestination(NPCSpawner.Type destinationType)
        {
            switch (destinationType)
            {
                case NPCSpawner.Type.Entrance:
                    return spawners[0].transform;
                case NPCSpawner.Type.Temple:
                    return spawners[1].transform;
                case NPCSpawner.Type.Pond:
                    return spawners[2].transform;
                default:
                    return spawners[0].transform;
            }
        }
    }
}
