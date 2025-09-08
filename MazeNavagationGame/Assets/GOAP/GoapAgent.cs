using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class GoapAgent : MonoBehaviour
{
    public float thinkDelay = 3f;
    public int thinkDelayCount = 0;
    public int resetActionQueueCount = 0;
    public int resetActionQueueCountsToReEvaluate = 5;
    public int thinkDelayAmountsToReEvaluate = 3;

    [ShowInInspector] public IGoal currentGoal;
    [ShowInInspector] public List<IGoal> GoalQueueView => GetGoalQueueSnapshot();
    [ShowInInspector] public List<IBelief> BeliefsHashView => Beliefs.ToList();

    public SimplePriorityQueue<IGoal, int> GoalPriorityQueue = new();
    [BoxGroup("Action Plan")][ShowInInspector] public NodeAction node;
    [BoxGroup("Action Plan")][ShowInInspector] public ActionPlan currentActionPlan;
    ActionPlan initialActionPlan;


    public HashSet<IBelief> Beliefs = new HashSet<IBelief>();
    [SerializeReference] public List<IGoal> goals = new();

    private void Start()
    {
        node = null;
        currentGoal = null;
        currentActionPlan = null;

        Initialize();
        StartCoroutine(PriorityManage());

        //print("Started AI");
    }

    private void Update()
    {
        if (Beliefs.Count > 0 && currentActionPlan != null) EvaluateBeliefs();

        if (node != null)
        {
            node.action.Update(Time.deltaTime);
        }
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
        GenerateActionPlan();
    }

    void GenerateActionPlan()
    {
        IGoal goal = ChoseGoal();
        GenerateActionPlan(goal, Beliefs);
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

            //chose goal
            IGoal goal = ChoseGoal();
            CreateGoalQueue(goals);


            //rebuild plan periodiclly (may delete later)
            if (thinkDelayCount > thinkDelayAmountsToReEvaluate || node == null)
            {
                GenerateActionPlan(goal, Beliefs);
                thinkDelayCount = 0;
            }


            if (node.action.functionality == null)
                UseCurrAction(out node, goal);

            if (!CheckIfEffectsAreSatisfied())
            {
                if (node.action.CanStartThenStart() == false)
                    resetActionQueueCount++;
            }
            else if (AttemptToMoveToNextAction() == false)
            {
                if (node.action.CanStartThenStart() == false)
                    resetActionQueueCount++;
            }
            else
                Debug.Log("Moving to next aciton");



            if (resetActionQueueCount > resetActionQueueCountsToReEvaluate)
            {
                resetActionQueueCount = 0;
                GoBackToInitialActionPlan();
                node.action.functionality = null;
            }
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

    //Ai gen help
    bool AttemptToMoveToNextAction()
    {
        if (node == null || node.action == null || !node.action.Complete) return false;

        if (!currentActionPlan.MoveNext()) return false; //Atempts a walk forward
        node = currentActionPlan.Current; //if succesfful set the new current node as the next

        if (node.action.CanStartThenStart()) return true; //if can start, movefwd else walk back

        while (currentActionPlan.MovePrev())
        {
            node = currentActionPlan.Current;
            if (node.action.CanStartThenStart()) return true;
        }

        return false;
    }

    //Alters goal's priorities based on beliefs
    void EvaluateBeliefs()
    {
        foreach (IBelief belief in Beliefs)
        {
            if (!belief.refreshing) continue;
            bool dueToUpdate = belief.GetRefreshDelay() <= 0f || (Time.time - belief.timeStamp) >= belief.GetRefreshDelay();
            if (dueToUpdate)
            {
                //print($"Updating Belief {belief.type}");
                belief.UpdateBelief((IReadOnlyList<IBelief>)Beliefs.ToList());
            }
        }

    }

    //If NOT go to previous action :)
    bool CheckIfEffectsAreSatisfied()
    {
        //print("thinking checking effects");
        bool ret = false;
        if (node == null || node.action == null || node.action.functionality == null)
            ret = false;

        if (node.action.EffectsSatisfied) ret = true;
        return ret;

    }

    //Attempt to do the top action
    //Ai gen helped alot here, I needed help understanding how the dbly linked list works
    //i notated parts to understand it better
    bool UseCurrAction(out NodeAction outNode, IGoal checkGoal)
    {
        print(message: $"Using top action: for goal {checkGoal.type}");
        outNode = null;

        if (currentActionPlan == null || currentActionPlan.Current == null) return false;

        var currNode = currentActionPlan?.Current;

        //if (nows) action is complete, move cursor next
        if (node != null && node.action.Complete)
        {
            if (!currentActionPlan.MoveNext()) return false;
            currNode = currentActionPlan.Current;
        }

        //Try start/restart current
        if (!currNode.action.CanStartThenStart())
        {
            //walk back cursor until it can run
            while (currentActionPlan.MovePrev()) // <- walks back
                if (currentActionPlan.Current.action.CanStartThenStart()) // <- Attempts a start
                    break; // <- if start successfull then break out the walk back

            //Whatrver node was succesfully started is where the cursor is at
            currNode = currentActionPlan.Current;
        }

        outNode = currNode;
        return true;

    }

    //Same here ai -gen
    void OnPreconditionLost()
    {
        if (currentActionPlan?.Current == null) return;

        do
        {
            if (!currentActionPlan.MovePrev()) break;
        }
        while (!currentActionPlan.Current.action.CanStartThenStart());
    }

    void GenerateActionPlan(IGoal chosenGoal, HashSet<IBelief> beliefs)
    {
        ActionPlan newActionPlan = new ActionPlan(chosenGoal, beliefs);
        currentActionPlan = newActionPlan;
        initialActionPlan = (ActionPlan)SerializationUtility.CreateCopy(newActionPlan);
        node = currentActionPlan?.Current;
        print($"Generated new ActionPlan, node: {node}");
    }

    void GoBackToInitialActionPlan()
    {
        currentActionPlan = initialActionPlan;
    }

    public void SatisfyPrecondition(IBelief satsifyBelief)
    {
        print($"SATISFY: atempt satisfy {satsifyBelief.type} cur action {node.action}");
        if (node.action != null)
            node.action.SatisfyPrecondition(satsifyBelief);
    }

    public void SatisfyEffect(IBelief satsifyBelief)
    {
        print($"SATISFY: atempt satisfy {satsifyBelief.type} cur action {node.action}");
        if (node.action != null)
            node.action.SatisfyEffect(satsifyBelief);

    }
}
