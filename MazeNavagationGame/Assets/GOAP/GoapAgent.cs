using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GoapAgent : MonoBehaviour
{
    public bool thinking;
    public ImportanceData importanceData;
    [SerializeReference] public List<Belief> beliefs = new();
    [SerializeReference] public List<AgentGoal> goals = new();

    private void Update()
    {
        if (!thinking)
        {
            thinking = true;
            StartCoroutine(PriorityManage());
        }
    }

    IEnumerator PriorityManage()
    {
        while (thinking)
        {
            yield return new WaitForEndOfFrame();

            CheckGoals();
            CheckBeliefs();
            ActOnKnowledge();
        }
    }

    void CheckGoals()
    {
        foreach (var goal in goals)
        {
            // simplest possible priority system:
            // every frame, increase priority if any belief is true
            foreach (var belief in beliefs)
            {
                if (belief.Evaluate())
                {
                    goal.currentPriority++;
                }
            }
        }
    }

    void CheckBeliefs()
    {
        foreach (var belief in beliefs)
        {
            Debug.Log($"{belief.GetType().Name} evaluated to {belief.Evaluate()}");
        }
    }

    void ActOnKnowledge()
    {
        // pick the goal with the highest priority
        AgentGoal topGoal = null;
        int highest = int.MinValue;

        foreach (var goal in goals)
        {
            if (goal.currentPriority > highest)
            {
                highest = goal.currentPriority;
                topGoal = goal;
            }
        }

        if (topGoal != null)
        {
            Debug.Log($"Acting on goal: {topGoal.GetType().Name} with priority {highest}");
        }
    }
}
