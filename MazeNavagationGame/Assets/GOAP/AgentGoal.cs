using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IGoal
{
    Goals type { get; }
    int initialPriority { get; }
    int currentPriority { get; set; }

    [SerializeField] List<AgentAction> RequiredActionsToAchieveGoal { get; set; }
    [SerializeField] List<IBelief> DesiredEffects { get; set; }
    int SetPriority(int amount);
    void SetGoapAgentToDesiredEffects(GoapAgent agent);

    bool AllRequiedActionsPreConditionsSatisfied();
}


[Serializable]
public class AgentGoal : IGoal
{
    [PropertyOrder(-10)] public virtual Goals type { get; }

    [SerializeField][PropertyOrder(0)] public int _initialPriority;
    public int initialPriority { get => _initialPriority; }
    [ShowInInspector][PropertyOrder(1)] public int currentPriority { get; set; }



    [SerializeReference] [PropertyOrder(2)] List<IBelief> _DesiredEffects;
    public List<IBelief> DesiredEffects { get => _DesiredEffects; set => _DesiredEffects = value; }
    public List<IBelief> GetDesiredEffects() => (this as IGoal).DesiredEffects;



    [SerializeReference][PropertyOrder(3)] List<AgentAction> _RequiredActionsToAchieveGoal;
    public List<AgentAction> RequiredActionsToAchieveGoal { get => _RequiredActionsToAchieveGoal; set => _RequiredActionsToAchieveGoal = value; }
    
    
    public void SetGoapAgentToDesiredEffects(GoapAgent agent)
    {
        GetDesiredEffects().ForEach(belief => belief.SetAgent(agent));
    }
    public int SetPriority(int amount) => currentPriority = amount;


    public bool AllRequiedActionsPreConditionsSatisfied()
    {
        if (RequiredActionsToAchieveGoal == null) return true;

        return RequiredActionsToAchieveGoal
            .Where(action => action != null)
            .SelectMany(action => action.Preconditions ?? Enumerable.Empty<IBelief>())
            .All(precond => precond != null && precond.EvaluateCondition() == true);
          
    }

    public AgentGoal() { }
}

