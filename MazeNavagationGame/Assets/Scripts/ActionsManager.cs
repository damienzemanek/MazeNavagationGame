using System;
using System.Collections;
using Sirenix.OdinInspector;
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

    public bool CanStartThenStart() => Complete = false;
    public void Update(float deltaTime) { }

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

    public bool CanStartThenStart()
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
                return true;
            }

        }

        return true;
    }

    public void Update(float deltaTime) { }

    bool CanAlwaysIdle() => true;
}

[Serializable]
public class FollowActionFunctionality : IActionFunctionality
{
    public NavMeshAgent agent;
    public Sensor viewRadiusSensor;
    public Sensor attackRadiusSensor;

    public bool CanExecute { get => true; }
    public bool Complete { get; private set; }
    public FollowActionFunctionality() { }

    public bool CanStartThenStart()
    {
        if (!viewRadiusSensor.inRange.current) return false;

        Debug.Log("ActionPlan -> Attempting Follow");
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var initHit, 5f, NavMesh.AllAreas))
                agent.Warp(newPosition: initHit.position);
        }
        Complete = false;
        return true;
    }

    public void Update(float delatTime)
    {
        if (!viewRadiusSensor.inRange.current) return;
        agent.SetDestination(viewRadiusSensor.lastSeenLoc.position);
        Complete = attackRadiusSensor.inRange.current;
    }
    public void CompleteAction() => Complete = true;

    //bool HasLineOfSight()
    //{
    //    RaycastHit hit;
    //    if(Raycast)
    //}
}

[Serializable]
public class AttackActionFunctionality : IActionFunctionality
{
    public NavMeshAgent agent;
    public Sensor sensor;
    public GameObject attackObject;
    bool attacking = false;
    public float attackCooldown;

    public bool CanExecute { get => true; }
    public bool Complete { get; private set; }
    public AttackActionFunctionality() { }

    public bool CanStartThenStart()
    {
        Debug.Log("ActionPlan -> Attemping ATTACK");
        if (!sensor.inRange.current) return false;
        return true;
    }

    public void Update(float delatTime)
    {
        Debug.Log("Updating ATTACK");
        if (!sensor.inRange.current) return;
        if (!attacking)
        {
            Debug.Log("Attacking");
            sensor.StartCoroutine(Attack(attackObject));
            agent.SetDestination(target: sensor.lastSeenLoc.position);
        }


    }

    IEnumerator Attack(GameObject atkObj)
    {
        atkObj.SetActive(true);
        attacking = true;
        yield return new WaitForSeconds(3f);
        atkObj.SetActive(false);
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;

    }

    public void CompleteAction() => Complete = true;

    //bool HasLineOfSight()
    //{
    //    RaycastHit hit;
    //    if(Raycast)
    //}
}