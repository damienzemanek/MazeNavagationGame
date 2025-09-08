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
    public float thinkDelay = 3f;
    public int thinkDelayCount = 0;
    public int thinkDelayAmountsToReEvaluate = 3;

    [ShowInInspector] public IGoal currentGoal;
    [ShowInInspector] public List<IGoal> GoalQueueView => GetGoalQueueSnapshot();
    [ShowInInspector] public List<IBelief> BeliefsHashView => Beliefs.ToList();

    public SimplePriorityQueue<IGoal, int> GoalPriorityQueue = new();
    [BoxGroup("Action Plan")] public AgentAction currentAction;
    [BoxGroup("Action Plan")] [ShowInInspector] public ActionPlan currentActionPlan;

    public HashSet<IBelief> Beliefs = new HashSet<IBelief>();
    [SerializeReference] public List<IGoal> goals = new();
    private void Start()
    {
        Initialize();
        StartCoroutine(PriorityManage());
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
        EvaluateBeliefs();
        IGoal goal = ChoseGoal();
        GenerateActionPlan(goal, Beliefs);

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
                //print($"Checking belief {belief.GetBelief().ToString()}");
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
        if (GoalPriorityQueue.TryFirst(out IGoal first))
            if (!first.AllRequiedActionsPreConditionsSatisfied())
                { top = null; return false; }

        top = GoalPriorityQueue.Dequeue();
        return true;
    }



    IEnumerator PriorityManage()
    {
        while (true)
        {
            yield return new WaitForSeconds(thinkDelay);
            thinkDelayCount++;
            print(thinkDelayCount);

            EvaluateBeliefs();
            IGoal goal = ChoseGoal();
            CreateGoalQueue(goals);
            UseTopAction(out currentAction);
            if (CheckIfCurrentActionsEffectsAreSatisfied()) UseTopAction(out currentAction);

            if (thinkDelayCount > thinkDelayAmountsToReEvaluate)
            { GenerateActionPlan(goal, Beliefs); thinkDelayCount = 0; }
        }
    }

    //Gets the goal with the highest priority
    IGoal ChoseGoal()
    {
        int highestPriorityGoalsIndex = -1;
        int highestPriority = 0;

        for(int i = 0;  i < goals.Count; i++)
        {
            if (goals[i].currentPriority > highestPriority)
            {
                highestPriorityGoalsIndex = i;
                highestPriority = goals[i].currentPriority;
                //print($"chosen New highest pri goal {goals[i]} pri {goals[i].currentPriority}");
            }
        }

        currentGoal = goals[highestPriorityGoalsIndex];
        //print($"chosen highest pri goal {goals[highestPriorityGoalsIndex]}");
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

    //If so move to next action in the queue
    bool CheckIfCurrentActionsEffectsAreSatisfied()
    {
        var effects = currentAction?.Effects;
        if (effects == null) return true;

        return effects.All(effect => effect != null && (effect.condition?.Invoke() ?? false));
    }

    //Attempt to do the top action
    bool UseTopAction(out AgentAction top)
    {
        if (currentActionPlan.ActionPriorityQueue.Count == 0) { top = null; return false; }
        top = currentActionPlan.ActionPriorityQueue.Dequeue();
        top.Start();
        print($"ActionPlan -> Doing top action {top}");
        return true;
    }

    void GenerateActionPlan(IGoal chosenGoal, HashSet<IBelief> beliefs)
    {
        ActionPlan newActionPlan = new ActionPlan(chosenGoal, beliefs);
        currentActionPlan = newActionPlan;
        print("Generated new ActionPlan");
    }
}
