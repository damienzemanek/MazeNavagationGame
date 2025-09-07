using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;



[Serializable]
public abstract class AgentGoal
{
    public string Name { get; }
    public abstract int initialPriority { get; set; }
    [ShowInInspector] public int currentPriority { get; set; }

    public HashSet<AgentBelief<T>> DesiredEffects { get; } = new HashSet<AgentBelief<T>>();

    public AgentGoal(string name) => Name = name; 

    public int SetPriority(int amount) => currentPriority = amount;
}

