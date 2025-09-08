using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Priority_Queue;
using Sirenix.OdinInspector;
using UnityEngine;
using static NPC;

[Serializable]
public class ActionPlan 
{
    bool initComplete;
    public IGoal Goal { get; }
    [ShowInInspector] public List<AgentAction> ActionPlanView => GetActionQueueSnapshot();

    [ShowInInspector] public SimplePriorityQueue<AgentAction, int> ActionPriorityQueue = new();
    public float totalCost { get; set; }

    public ActionPlan(IGoal goal, HashSet<IBelief> beliefs)
    {
        Goal = goal;
        GenerateActions(goal);
        initComplete = true;
    }

    void GenerateActions(IGoal goal)
    {
        foreach (AgentAction action in goal.RequiredActionsToAchieveGoal)
        {
            ActionPriorityQueue.Enqueue(action, ActionPriorityQueue.Count + 1);
        }

        //Debug.Log($"Action Plan Count: [{ActionPriorityQueue.Count}]");
    }


    //AI gen function so i can see the queue in inspector
    public List<AgentAction> GetActionQueueSnapshot()
    {
        if (!initComplete) return null;
        // If you enqueued with NEGATED priorities (max-first), keep OrderBy(...)
        return ActionPriorityQueue
            .Select(a => new { A = a, P = ActionPriorityQueue.GetPriority(a) })
            .OrderBy(x => x.P)          // most negative (i.e., highest real priority) first
            .Select(x => x.A)
            .ToList();
    }

}
