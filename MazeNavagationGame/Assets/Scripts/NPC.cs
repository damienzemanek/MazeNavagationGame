using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;
using static NPC;

public class NPC : MonoBehaviour
{
    [Serializable]
    public class Goal
    {
        public string goalName;
        public Transform loc;
        public bool moving;
    
    }


    [SerializeReference] public List<Goal> Goals;


    void GoToLoc(Goal goal)
    {
        if (goal.moving == true) return;
        StartCoroutine(Moving(goal));
    }

    IEnumerator Moving(Goal goal)
    {
        print("Moving");
        transform.Translate(goal.loc.position);
        yield return new WaitForEndOfFrame();
    }


    private void Start()
    {
        GoToLoc(Goals[0]);
    }

}
