using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;
using UnityEngine.AI;


[Serializable]
public class AgentAction
{
    public virtual string Name { get; set; }
    [ShowInInspector] public virtual float Cost { get; private set; }

    [SerializeReference] public List<IBelief> Preconditions = new List<IBelief>();
    [SerializeReference] public List<IBelief> Effects = new List<IBelief>();

    IAction functionality;
    public bool Complete => functionality.Complete;

    public virtual void Initialize() { }

    public virtual void Start() => functionality.Start();

    public virtual void Update(float deltaTime)
    {
        if (functionality.CanExecute) functionality.Update(deltaTime);

        if (!functionality.Complete) return;

        Effects.ForEach(belief => belief.UpdateBelief(Effects.ToList()));

    }
    public virtual void Stop() => functionality.Stop();
}


public class IdleAction : IAction
{
    public bool CanExecute { get => CanAlwaysIdle(); }
    public bool Complete { get; private set; }
    public IdleAction() { }

    public void Start() => Complete = false;
    public void CompleteAction() => Complete = true;

    bool CanAlwaysIdle() => true;
}

public class WanderAction : IAction
{
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public bool CanExecute { get => CanAlwaysIdle(); }
    public bool Complete { get; private set; }
    public WanderAction() { }

    public void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 cir = UnityEngine.Random.insideUnitCircle * wanderRadius;
            Vector3 randomDir = new Vector3(cir.x, 0, cir.y);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(agent.transform.position + randomDir, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
                return;
            }

        }
    }
    public void CompleteAction() => Complete = true;

    bool CanAlwaysIdle() => true;
}


