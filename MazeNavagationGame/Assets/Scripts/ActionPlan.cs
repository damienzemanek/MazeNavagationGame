using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Priority_Queue;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ActionPlan 
{
    public AgentGoal Goal { get; }

    [ShowInInspector] public SimplePriorityQueue<AgentAction, int> ActionPriorityQueue = new();
    public float totalCost { get; set; }

    public ActionPlan(AgentGoal goal, SimplePriorityQueue<AgentAction, int> queue, float totalCost)
    {
        Goal = goal;
        ActionPriorityQueue = queue;
        this.totalCost = totalCost;
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
    }

}
