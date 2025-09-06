using System;
using Sirenix.OdinInspector;
using UnityEngine;



[Serializable]
public abstract class AgentGoal
{
    public abstract int initialPriority { get; set; }
    [ShowInInspector] public int currentPriority { get; set; }

    public int SetPriority(int amount) => currentPriority = amount;
}

[Serializable]
public class Stand : AgentGoal
{
    [SerializeField] int _initialPriority;
    public override int initialPriority { get => _initialPriority; set => _initialPriority = value; }



}

[Serializable]
public class Move : AgentGoal
{
    [SerializeField] int _initialPriority;
    public override int initialPriority { get => _initialPriority; set => _initialPriority = value; }



}


[Serializable]
public class Patrol : AgentGoal
{
    [SerializeField] int _initialPriority;
    public override int initialPriority { get => _initialPriority; set => _initialPriority = value; }



}

