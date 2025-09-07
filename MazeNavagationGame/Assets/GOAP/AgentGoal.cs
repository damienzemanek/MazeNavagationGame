using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IGoal
{
    string Name { get; }
    int initialPriority { get; set; }
    int currentPriority { get; set; }
    [SerializeField] List<IBelief> DesiredEffects { get; set; }
    int SetPriority(int amount);
}


[Serializable]
public class AgentGoal : IGoal
{
    public string Name { get; }
    [SerializeField][ShowInInspector] public int initialPriority { get; set; }
    [ShowInInspector] public int currentPriority { get; set; }

    [ShowInInspector]
    [SerializeField] List<IBelief> IGoal.DesiredEffects { get; set; } = new List<IBelief>();
    public List<IBelief> GetDesiredEffects() => (this as IGoal).DesiredEffects;

    public AgentGoal(string name) => Name = name; 

    public int SetPriority(int amount) => currentPriority = amount;
    public AgentGoal() { }
}

