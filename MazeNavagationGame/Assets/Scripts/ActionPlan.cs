using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[Serializable]
public class ActionPlan
{
    bool initComplete;
    public IGoal Goal { get; }
    public NodeAction Head { get; private set; }
    public NodeAction Tail { get; private set; }
    public NodeAction Current { get; private set; }

    [ShowInInspector] public List<NodeAction> Nodes = new();
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
    }

    public bool MoveNext()
    {
        if (Current?.next == null) return false;
        Current = Current.next;
        return true;
    }

    public bool MovePrev()
    {
        if (Current?.prev == null) return false;
        Current = Current.prev;
        return true;
    }


}


public class NodeAction
{
    public readonly AgentAction action;
    [DisplayAsString] public NodeAction prev, next;

    public NodeAction(AgentAction action) => this.action = action;

}

