using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateContext : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 target = new Vector3(5,0,5);

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target);
    }

    private void FixedUpdate()
    {
        
    }
}
