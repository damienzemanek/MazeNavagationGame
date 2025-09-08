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
using UnityEngine.Rendering;

public class GoapAgent : MonoBehaviour
{
    public float thinkDelay = 3f;
    public int thinkDelayCount = 0;
    public int thinkDelayAmountsToReEvaluate = 3;

    [ShowInInspector] public List<AgentAction> previousActions;

    [ShowInInspector] public IGoal currentGoal;
    [ShowInInspector] public List<IGoal> GoalQueueView => GetGoalQueueSnapshot();
    [ShowInInspector] public List<IBelief> BeliefsHashView => Beliefs.ToList();

    public SimplePriorityQueue<IGoal, int> GoalPriorityQueue = new();
    [BoxGroup("Action Plan")] public AgentAction currentAction;
    [BoxGroup("Action Plan")][ShowInInspector] public ActionPlan currentActionPlan;

    public HashSet<IBelief> Beliefs = new HashSet<IBelief>();
    [SerializeReference] public List<IGoal> goals = new();
    private void Start()
    {
        currentAction = null;
        currentGoal = null;
        currentActionPlan = null;

        Initialize();
        StartCoroutine(PriorityManage());

        //print("Started AI");
    }

    private void Update()
    {
        if (Beliefs.Count > 0 && currentActionPlan != null) EvaluateBeliefs();
    }


    void Initialize()
    {
        var liveGoals = goals.Where(g => g != null).ToList();
        liveGoals.ForEach(goal =>
        {
            goal.SetPriority(goal.initialPriority);
            goal.SetGoapAgentToDesiredEffects(this);
            goal.SetOriginalGoalsToBeliefs();
        });

        CreateGoalQueue(liveGoals);
        CreateBeliefHashSet(liveGoals);
        IGoal goal = ChoseGoal();
        GenerateActionPlan(goal, Beliefs);

        //print("Goap Agent finished Initializing");
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

        //print($"finished creating goal queue, count:[{GoalPriorityQueue.Count}]");
    }

    HashSet<Beliefs> seen = new HashSet<Beliefs>();

    void CreateBeliefHashSet(List<IGoal> liveGoals)
    {
        HashSet<IBelief> ret = new HashSet<IBelief>();

        liveGoals
            .Where(goal => goal != null)
            .SelectMany(goal =>
            {
                List<IBelief> allBeliefs =
                    (goal.DesiredEffects ?? Enumerable.Empty<IBelief>())
                    .Concat((goal.RequiredActionsToAchieveGoal ?? Enumerable.Empty<AgentAction>())
                        .Where(action => action != null)
                        .SelectMany(action =>
                            (action.Preconditions ?? Enumerable.Empty<IBelief>())
                            .Concat(action.Effects ?? Enumerable.Empty<IBelief>())
                        ))
                    .Where(belief => belief != null)
                    .Distinct()
                    .ToList();
                return allBeliefs;
            })
            .ToList()
            .ForEach(belief =>
            {

                //print($"Checking belief {belief.GetBelief().ToString()}");
                if (!seen.Contains(belief.type)) seen.Add(belief.type);
                else return;

                IBelief copy = belief.CreateCopy(belief);
                copy.refreshing = true;
                copy.satisfied = true;
                copy.SetAgent(this);
                copy.BeliefChangedCallback += CallbackBeliefActionUse;

                ret.Add(copy);
            });

        Beliefs = ret;
        //print($"finished creating belief hash, count:[{Beliefs.Count}]");
    }



    IEnumerator PriorityManage()
    {
        while (true)
        {
            yield return new WaitForSeconds(thinkDelay);
            thinkDelayCount++;
            print("Thinking");
            IGoal goal = ChoseGoal();
            CreateGoalQueue(goals);
            if (CheckIfPreconditionsAreSatisifed())
                UseTopAction(out currentAction, goal);
            else
                UsePreviousAction();

            AttemptToMoveToNextAction();
            if (thinkDelayCount > thinkDelayAmountsToReEvaluate)
            { GenerateActionPlan(goal, Beliefs); thinkDelayCount = 0; }
        }
    }

    //Gets the goal with the highest priority
    IGoal ChoseGoal()
    {
        int highestPriorityGoalsIndex = -1;
        int highestPriority = 0;

        for (int i = 0; i < goals.Count; i++)
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

    void AttemptToMoveToNextAction()
    {
        print($"thinking checking if current action complete : {currentAction.functionality.GetType()} : {currentAction.Complete}");
        if (!currentAction.Complete) return;
        if (!currentActionPlan.ActionPriorityQueue.First.PreConditionsSatisfied) return;
        currentAction = null;
        //currentActionPlan.ActionPriorityQueue.Dequeue();
    }

    //Alters goal's priorities based on beliefs
    void EvaluateBeliefs()
    {
        foreach (IBelief belief in Beliefs)
        {
            if (!belief.refreshing) return;
            bool dueToUpdate = belief.GetRefreshDelay() <= 0f || (Time.time - belief.timeStamp) >= belief.GetRefreshDelay();
            if (dueToUpdate)
            {
                //print($"Updating Belief {belief.type}");
                belief.UpdateBelief((IReadOnlyList<IBelief>)Beliefs.ToList());
            }
        }

    }

    //If NOT go to previous action :)
    bool CheckIfPreconditionsAreSatisifed()
    {
        print("thinking checking precons");
        bool ret = false;
        if (currentAction == null || currentAction.functionality == null) return true; //new Action check

        if (currentAction.PreConditionsSatisfied) ret = true;

        print($"thinking -> Conditions Satisifed? {ret}");
        return ret;

    }

    void CallbackBeliefActionUse(IGoal origGoal)
    {
        print("Callback from belief to action being done");
        if (origGoal != currentGoal) return;
        UseTopAction(out AgentAction top, currentGoal);
    }

    //Attempt to do the top action
    bool UseTopAction(out AgentAction top, IGoal checkGoal)
    {
        print("action doing");
        if (currentActionPlan.ActionPriorityQueue.Count == 0) { top = null; return false; }
        top = currentActionPlan.ActionPriorityQueue.Dequeue();
        top.Start();
        print($"action -> Doing top action {top.functionality.GetType()}");
        if (!previousActions.Contains(top))
            previousActions.Add(top);
        return true;
    }

    void UsePreviousAction()
    {
        //print($"Thinking -> Attempting Prev Action");

        if (previousActions[0] == null) return;
        for (int i = 0; i < previousActions.Count; i++)
        {
            if (currentAction != previousActions[i] || previousActions.Count == 1)
            {
                previousActions[i].Start();

                //print($"Thinking -> Doing previous top action {previousActions[i].functionality.GetType()}");

                return;
            }
        }

    }
    void GenerateActionPlan(IGoal chosenGoal, HashSet<IBelief> beliefs)
    {
        ActionPlan newActionPlan = new ActionPlan(chosenGoal, beliefs);
        currentActionPlan = newActionPlan;
        print("Generated new ActionPlan");
    }

    public void SatisfyPrecondition(IBelief satsifyBelief)
    {
        print("satisfied? : goap agent trying to satisfy cond " + satsifyBelief.type);
        foreach(AgentAction action in currentActionPlan.GetActionQueueSnapshot())
            action.SatisfyPrecondition(satsifyBelief);

    }
}
