using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Thiruvizha.NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        public int MaxNpcs;
        public float SpawnRate;


        //Spawn Timer
        private bool canSpawn = false;
        private float timer = 0;

        public NPCStateContext[] npcPool;

        public void Initialize(Thiruvizha.Grids.GridManager game)
        {
            npcPool = new NPCStateContext[MaxNpcs];
            Transform npcTransform = game.NPC.transform;
            for (int i = 0; i < MaxNpcs; i++)
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

        public void EnableSpawn()
        {
            canSpawn = true;
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
                            npc.gameObject.SetActive(true);  
                            npc.InitializeEnergy();
                            Debug.Log("This Ran");
                            break; 
                        }
                    }
                    timer = SpawnRate;
                }
            }
        }
    }
}
