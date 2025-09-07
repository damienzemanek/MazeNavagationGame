using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;


public interface IGoapPlanner
{
    ActionPlan Plan(GoapAgent agent, List<AgentGoal> goals, AgentGoal mostRecentGoal = null);
}

public class Planner : IGoapPlanner
{
    public ActionPlan Plan(GoapAgent agent, List<AgentGoal> goals, AgentGoal mostRecentGoal = null)
    {
        List<AgentGoal> orderedGoals = goals
            .Where(goal => goal.GetDesiredEffects().Any(belief => !belief.EvaluateCondition()))
            .OrderByDescending(goal => goal == mostRecentGoal ? goal.currentPriority - 0.01 : goal.currentPriority)
            .ToList();

        return null;
    }
}


public class ActionPlan 
{
    public AgentGoal Goal { get; }
    public Stack<AgentAction> Actions { get; }
    public float totalCost { get; set; }

    public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
    {
        Goal = goal;
        Actions = actions;
        this.totalCost = totalCost;
    }

}
