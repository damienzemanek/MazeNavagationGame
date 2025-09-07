using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Net;
using static NPC;
using Priority_Queue;
using Sirenix.OdinInspector;

public class GoapAgent : MonoBehaviour
{
    public bool thinking;
    public float thinkDelay = 3f;

    [ShowInInspector] public IGoal currentGoal;
    public AgentAction currentAction;

    [ShowInInspector] public List<IGoal> GoalQueueView => GetGoalQueueSnapshot();
    [ShowInInspector] public List<IBelief> BeliefsHashView => Beliefs.ToList();

    public SimplePriorityQueue<IGoal, int> GoalPriorityQueue = new();
    [ShowInInspector] public ActionPlan currentActionPlan;

    public HashSet<IBelief> Beliefs = new HashSet<IBelief>();
    [SerializeReference] public List<IGoal> goals = new();
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (!thinking)
        {
            thinking = true;
            StartCoroutine(PriorityManage());
        }
    }

    void Initialize()
    {
        var liveGoals = goals.Where(g => g != null).ToList();
        liveGoals.ForEach(goal =>
        {
            goal.SetPriority(goal.initialPriority);
            goal.SetGoapAgentToDesiredEffects(this);
        });

        CreateGoalQueue(liveGoals);
        CreateBeliefHashSet(liveGoals);

        print("Goap Agent finished Initializing");
    }

    //AI gen function so i can see the queue in inspector
    private List<IGoal> GetGoalQueueSnapshot()
    {
        // Enumerate items, sort by stored priority, then project to a list.
        // We enqueued with NEGATED priorities, so ascending order == highest first.
        return GoalPriorityQueue
            .Select(g => new { G = g, P = GoalPriorityQueue.GetPriority(g) })
            .OrderBy(x => x.P)              // most negative (highest real priority) first
            .Select(x => x.G)
            .ToList();
    }

    void CreateGoalQueue(List<IGoal> goals)
    {
        GoalPriorityQueue.Clear();
        goals.Where(g => g != null)
             .ToList()
             .ForEach(g => GoalPriorityQueue.Enqueue(g, -g.currentPriority));

        print($"finished creating goal queue, count:[{GoalPriorityQueue.Count}]");
    }

    HashSet<Beliefs> seen = new HashSet<Beliefs>();

    void CreateBeliefHashSet(List<IGoal> liveGoals)
    {
        HashSet<IBelief> ret = new HashSet<IBelief>();
        
        liveGoals
            .Where(goal => goal?.DesiredEffects != null)
            .SelectMany(goal => goal.DesiredEffects)
            .ToList() 
            .ForEach(belief => 
            {
                print($"Checking belief {belief.GetBelief().ToString()}");
                if (!seen.Contains(belief.type)) seen.Add(belief.type);
                else return;

                var copy = belief.CreateCopy(belief);
                copy.refreshing = true;

                ret.Add(copy);
            });

        Beliefs = ret;
        print($"finished creating belief hash, count:[{Beliefs.Count}]");
    }

    bool TryDequeueTop(out IGoal top)
    {
        if (GoalPriorityQueue.Count == 0) { top = null; return false; }
        top = GoalPriorityQueue.Dequeue();
        return true;
    }


    IEnumerator PriorityManage()
    {
        while (thinking)
        {
            yield return new WaitForSeconds(thinkDelay);



            EvaluateBeliefs();
            IGoal goal = ChoseGoal();
            print(goal.type.ToString());
            DetermineActionPlan(goal);
        }
    }

    //Gets the goal with the highest priority
    IGoal ChoseGoal()
    {
        int highestPriorityGoalsIndex = 0;
        int highestPriority = 0;

        for(int i = 0;  i < goals.Count; i++)
        {
            if (goals[i].currentPriority > highestPriorityGoalsIndex)
            {
                highestPriorityGoalsIndex = i;
                highestPriority = goals[i].currentPriority;
            }
        }

        currentGoal = goals[highestPriorityGoalsIndex];
        return goals[highestPriorityGoalsIndex];
    }

    //Alters goal's priorities based on beliefs
    void EvaluateBeliefs()
    {
       
        foreach (IBelief belief in Beliefs)
        {
            if (!belief.refreshing) return;

            bool dueToUpdate = belief.GetRefreshDelay() <= 0f || (Time.time - belief.timeStamp) >= belief.GetRefreshDelay();
            if (dueToUpdate)
                belief.UpdateBelief((IReadOnlyList<IBelief>)Beliefs.ToList());
        }

    }

    void DetermineActionPlan(IGoal chosenGoal)
    {

    }
}
