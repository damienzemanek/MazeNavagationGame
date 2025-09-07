using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class GoapAgent : MonoBehaviour
{
    public bool thinking;
    public float thinkDelay = 3f;

    public AgentGoal currentGoal;
    //public ActionPlan actionPlan;
    public AgentAction currentAction;

    [SerializeReference] public List<IBelief> beliefs = new();
    [SerializeReference] public List<AgentGoal> goals = new();
    [SerializeReference] public List<AgentAction> actions = new();

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
        foreach (IBelief belief in beliefs)
            belief.SetAgent(this);

        foreach (AgentGoal goal in goals)
            if(goal != null) goal.SetPriority(goal.initialPriority);
    }

    void ReEvaluatePlan()
    {
        currentAction = null;
        currentAction = null;
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
    AgentGoal ChoseGoal()
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
        var givenBeliefs = (IReadOnlyList<IBelief>)beliefs;
        foreach(IBelief belief in beliefs)
        {
            bool dueToUpdate = belief.GetRefreshDelay() <= 0f || (Time.time - belief.timeStamp) >= belief.GetRefreshDelay();
            if (dueToUpdate)
                belief.UpdateBelief(givenBeliefs);
        }

    }

    void ActOnKnowledge()
    {
        // pick the goal with the highest priority
        AgentGoal topGoal = null;
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
