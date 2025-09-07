using System;
using UnityEngine;


public enum Goals
{
    Sit,
    Wander
}

public class GoalsManager : MonoBehaviour
{

}

[Serializable]
public class GoalSit : AgentGoal, IGoal
{
    public override Goals type { get => Goals.Sit; }
    public GoalSit() { }
}

[Serializable]
public class GoalWander : AgentGoal, IGoal
{
    public override Goals type { get => Goals.Wander; }
    public GoalWander() { }

}
