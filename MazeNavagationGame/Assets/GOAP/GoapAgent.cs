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

    public AgentGoal currentGoal;
    //public ActionPlan actionPlan;
    public AgentAction currentAction;
    [ShowInInspector] public List<IGoal> GoalQueueView => GetGoalQueueSnapshot();
    [ShowInInspector] public List<AgentAction> ActionQueueView => GetActionQueueSnapshot();
    [ShowInInspector] public List<IBelief> BeliefsHashView => Beliefs.ToList();

    public SimplePriorityQueue<IGoal, int> GoalPriorityQueue = new();
    public SimplePriorityQueue<AgentAction, int> ActionPriorityQueue = new();

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
    //AI gen function so i can see the queue in inspector
    private List<AgentAction> GetActionQueueSnapshot()
    {
        // If you enqueued with NEGATED priorities (max-first), keep OrderBy(...)
        return ActionPriorityQueue
            .Select(a => new { A = a, P = ActionPriorityQueue.GetPriority(a) })
            .OrderBy(x => x.P)          // most negative (i.e., highest real priority) first
            .Select(x => x.A)
            .ToList();

        // If you DIDN'T negate when Enqueue'ing, use:
        // .OrderByDescending(x => x.P)
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
            //AgentGoal goal = ChoseGoal();
            //ActOnKnowledge();
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
        return goals[highestPriorityGoalsIndex];
    }

    //Alters goal's priorities based on beliefs
    void EvaluateBeliefs()
    {
        //var givenBeliefs = (IReadOnlyList<IBelief>)beliefs;
        //foreach(IBelief belief in beliefs)
        //{
        //    if (!belief.refreshing) return;

        //    bool dueToUpdate = belief.GetRefreshDelay() <= 0f || (Time.time - belief.timeStamp) >= belief.GetRefreshDelay();
        //    if (dueToUpdate)
        //        belief.UpdateBelief(givenBeliefs);
        //}

    }

    void ActOnKnowledge()
    {
        // pick the goal with the highest priority
        IGoal topGoal = null;
        int highest = int.MinValue;

        foreach (var goal in goals)
        {
            if (goal.currentPriority > highest)
            {
                highest = goal.currentPriority;
                topGoal = goal;
            }
        }

        if (topGoal != null)
        {
            Debug.Log($"Acting on goal: {topGoal.GetType().Name} with priority {highest}");
        }
    }
}
