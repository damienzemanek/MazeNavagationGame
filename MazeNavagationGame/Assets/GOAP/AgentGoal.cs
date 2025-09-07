using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IGoal
{
    string Name { get; }
    int initialPriority { get; }
    int currentPriority { get; set; }
    [SerializeField] List<IBelief> DesiredEffects { get; set; }
    int SetPriority(int amount);
    void SetGoapAgentToDesiredEffects(GoapAgent agent);

}


[Serializable]
public class AgentGoal : IGoal
{
    public string Name { get; }
    [SerializeField][PropertyOrder(0)] public int _initialPriority;
    public int initialPriority { get => _initialPriority; }
    [ShowInInspector][PropertyOrder(1)] public int currentPriority { get; set; }

     [SerializeReference] [PropertyOrder(2)] List<IBelief> _DesiredEffects;
     public List<IBelief> DesiredEffects { get => _DesiredEffects; set => _DesiredEffects = value; }
     public List<IBelief> GetDesiredEffects() => (this as IGoal).DesiredEffects;
    public void SetGoapAgentToDesiredEffects(GoapAgent agent)
    {
        GetDesiredEffects().ForEach(belief => belief.SetAgent(agent));
    }

    public AgentGoal(string name) => Name = name; 

    public int SetPriority(int amount) => currentPriority = amount;
    public AgentGoal() { }
}

