using System;
using UnityEngine;


public class GoalsManager : MonoBehaviour
{

}

[Serializable]
public class GoalSit : AgentGoal, IGoal
{
    public GoalSit() { }
}

[Serializable]
public class GoalWander : AgentGoal, IGoal
{
    public GoalWander() { }

}
