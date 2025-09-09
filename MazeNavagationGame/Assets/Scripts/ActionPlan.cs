using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ActionPlan
{
    bool initComplete;
    public IGoal Goal { get; }
    public NodeAction Head { get; private set; }
    public NodeAction Tail { get; private set; }
    public NodeAction Current { get; private set; }

    [SerializeReference] public List<NodeAction> Nodes = new();
    public ActionPlan(IGoal goal, HashSet<IBelief> beliefs)
    {
        Goal = goal;
        BuildChain(goal);
        initComplete = true;
    }

    void BuildChain(IGoal goal)
    {
        NodeAction prev = null;
        foreach (AgentAction action in goal.RequiredActionsToAchieveGoal)
        {
            if (action == null) continue;
            var n = new NodeAction(action);
            if (Head == null) Head = n;
            if (prev != null) { prev.next = n; n.prev = prev; }
            Nodes.Add(n);

            prev = n;
        }
        Tail = prev;
        Current = Head;
    }

    public bool MoveNext()
    {
        if (Current?.next == null) return false;
        Current = Current.next;
        Debug.Log($"walk forward success :  {Current.prev.action.functionality} --> {Current.action.functionality} ");
        return true;
    }

    public bool MovePrev()
    {
        if (Current?.prev == null) return false;
        Current = Current.prev;
        Debug.Log($"walk BACK success :  {Current.next.action.functionality} --> {Current.action.functionality} ");

        return true;
    }


}


[Serializable]
public class NodeAction
{
    [ShowInInspector][PropertyOrder(-99)] public string Name => (action != null) ? GetActionName() : "";
    [ShowInInspector] public readonly AgentAction action;
    public NodeAction prev, next;

    [ShowInInspector] public string prevName => (prev != null) ? GetPrevName() : "";
    [ShowInInspector] public string nextName => (next != null) ? GetNextName() : "";

    public string GetPrevName() => prev.action.functionality.ToString();
    public string GetNextName() => next.action.functionality.ToString();

    public string GetActionName() => action.functionality.ToString();

    public NodeAction(AgentAction action) => this.action = action;

}

