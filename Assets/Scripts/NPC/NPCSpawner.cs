using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Thiruvizha.NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        public float SpawnRate;

        //Spawn Timer
        private bool canSpawn = false;
        private float timer = 0;

        public enum Type
        {
            Entrance,
            Temple,
            Pond
        }

        public Type SpawnerType;

        public NPCStateContext[] npcPool;

        public void Initialize()
        {
            npcPool = new NPCStateContext[NPCManager.instance.MaxNpcs];
            Transform npcTransform = NPCManager.instance.NPC.transform;
            for (int i = 0; i < NPCManager.instance.MaxNpcs; i++)
            {
                NPCStateContext npc = Instantiate(npcTransform, transform.position, Quaternion.identity,this.transform).GetComponent<NPCStateContext>();
                npcPool[i] = npc;
                npc.OnExitReached += ResetNpc;
                npc.gameObject.SetActive(false);
            }
        }

        private void ResetNpc(object sender, EventArgs e)
        {
            NPCStateContext npc = sender as NPCStateContext;
            npc.transform.position = transform.position;
            npc.gameObject.SetActive(false);
        }

        private void SetNPCDestination(NPCStateContext npc)
        {
            List<NPCSpawner.Type> type = new List<Type>();
            type.Add(Type.Entrance);
            type.Add(Type.Temple);
            type.Add(Type.Pond);

            switch (SpawnerType)
            {
                case NPCSpawner.Type.Entrance:
                    type.Remove(Type.Entrance);
                    break;
                case NPCSpawner.Type.Temple:
                    type.Remove(Type.Temple);
                    break;
                case NPCSpawner.Type.Pond:
                    type.Remove(Type.Pond);
                    break;
            }

            int i = UnityEngine.Random.Range(0, type.Count);

            npc.SetDestination(type[i]);
        }
        public void EnableSpawn()
        {
            canSpawn = true;
        }
        public void DisableSpawn()
        {
            canSpawn = false;
        }
        public bool CanSpawn()
        {
            return canSpawn;
        }

        private void FixedUpdate()
        {
            if (canSpawn)
            {
                timer -= Time.fixedDeltaTime;
                if (timer < 0)
                {
                    foreach (NPCStateContext npc in npcPool)
                    {
                        if (npc.gameObject.activeSelf) continue;
                        else 
                        { 
                            SetNPCDestination(npc);
                            npc.gameObject.SetActive(true);  
                            npc.InitializeEnergy();
                            break; 
                        }
                    }
                    timer = SpawnRate;
                }
            }
        }
    }
}
