using UnityEngine.AI;
using UnityEngine;

interface IActionStrategy
{
    bool CanExecute { get; }
    bool Complete { get; }

    void Start()
    {

    }

    void Update(float deltaTime)
    {

    }

    void Stop()
    {

    }

    void CompleteAction()
    {

    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanExecute { get => CanAlwaysIdle(); }
    public bool Complete { get; private set; }
    public IdleStrategy() { }

    public void Start() => Complete = false;
    public void CompleteAction() => Complete = true;

    bool CanAlwaysIdle() => true;
}

public class WanderStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public bool CanExecute { get => CanAlwaysIdle(); }
    public bool Complete { get; private set; }
    public WanderStrategy() { }

    public void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            Vector2 cir = Random.insideUnitCircle * wanderRadius;
            Vector3 randomDir = new Vector3(cir.x, 0, cir.y);

            NavMeshHit hit;

            if(NavMesh.SamplePosition(agent.transform.position + randomDir, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
                return;
            }

        }
    }
    public void CompleteAction() => Complete = true;

    bool CanAlwaysIdle() => true;
}



