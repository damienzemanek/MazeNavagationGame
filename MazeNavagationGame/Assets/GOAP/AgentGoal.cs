using System;
using UnityEngine;



[Serializable]
public abstract class AgentGoal
{ 
    public abstract int currentPriority { get; set; }

    public int SetPriority(int amount) => currentPriority = amount;
}

[Serializable]
public class Stand : AgentGoal
{
    public int _currentPriority;
    public override int currentPriority { get => _currentPriority; set => _currentPriority = value; }



}

[Serializable]
public class Move : AgentGoal
{
    public int _currentPriority;
    public override int currentPriority { get => _currentPriority; set => _currentPriority = value; }



}


[Serializable]
public class Patrol : AgentGoal
{
    public int _currentPriority;
    public override int currentPriority { get => _currentPriority; set => _currentPriority = value; }



}

