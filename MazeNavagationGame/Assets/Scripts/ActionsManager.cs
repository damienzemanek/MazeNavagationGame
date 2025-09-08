using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ActionsManager : MonoBehaviour
{


}


[Serializable]
public class IdleActionFunctionality : IActionFunctionality
{
    public bool CanExecute { get => CanAlwaysIdle(); }
    public bool Complete { get; private set; }
    public IdleActionFunctionality() { }

    public void Start() => Complete = false;
    public void CompleteAction() => Complete = true;

    bool CanAlwaysIdle() => true;
}

[Serializable]
public class WanderActionFunctionality : IActionFunctionality
{
    public NavMeshAgent agent;
    public float wanderRadius;

    public bool CanExecute { get => CanAlwaysIdle(); }
    bool complt = true;
    [ShowInInspector] public bool Complete { get => complt; private set => complt = value; }
    public WanderActionFunctionality() { }

    public void Start()
    {
        //Debug.Log("ActionPlan -> Starting Wander");
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var initHit, 5f, NavMesh.AllAreas))
                agent.Warp(newPosition: initHit.position);
        }
        //Debug.Log($"AI -> Agent: [{agent.gameObject.name}] is on nav mesh: [{agent.isOnNavMesh}]");

        for (int i = 0; i < 5; i++)
        {
            Vector2 cir = UnityEngine.Random.insideUnitCircle * wanderRadius;
            Vector3 randomDir = new Vector3(cir.x, 0, cir.y);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(agent.transform.position + randomDir, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log("AI -> Setting agent position");
                return;
            }

        }
    }

    bool CanAlwaysIdle() => true;
}

[Serializable]
public class FollowActionFunctionality : IActionFunctionality
{
    public NavMeshAgent agent;
    public Sensor sensor;

    public bool CanExecute { get => true; }
    public bool Complete { get; private set; }
    public FollowActionFunctionality() { }

    public void Start()
    {
        Debug.Log("ActionPlan -> Attempting Follow");
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var initHit, 5f, NavMesh.AllAreas))
                agent.Warp(newPosition: initHit.position);
        }
        if (!sensor.inRange.current) return;

        agent.SetDestination(sensor.lastSeenLoc.position);
    }
    public void CompleteAction() => Complete = true;

    //bool HasLineOfSight()
    //{
    //    RaycastHit hit;
    //    if(Raycast)
    //}
}