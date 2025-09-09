using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public interface ISensor<GoapAgent, T>
{
    T[] Do(GoapAgent agent);

    T[] data { get; set; }

}


//class IBeliefSeesPointOfInterest : IBeliefFunctionality
//{
//    public bool SeesPointOfInterest();
//}

//class IBeliefMovingToPointOfInterest : IBeliefFunctionality
//{
//    public bool M0ovingToPointOfInterest();
//}

public static class BeliefsManager
{
    public static Dictionary<Beliefs, float> refreshRates = new Dictionary<Beliefs, float>
    {
        {Beliefs.NoBeliefFound, 0},
        {Beliefs.Nothing, 5},
        {Beliefs.MyLocation, 0.5f},
        {Beliefs.AmIdling, 0.49f},
        {Beliefs.CanSeePlayer, 0.1f},
        {Beliefs.CanAttackPlayer, 0.05f}
    };

}

public enum Beliefs
{
    NoBeliefFound,
    Nothing,
    MyLocation,
    AmIdling,
    CanSeePlayer,
    CanAttackPlayer
}



[Serializable]
public class LocationBelief : AgentBelief<Vector3>
{
    [SerializeReference] public ISensor<GoapAgent, Vector3> functionality;

    [ShowInInspector][PropertyOrder(10)] public Vector3[] Location => data;
    [ShowInInspector]
    public override Beliefs type
    {
        get
        {
            if (functionality is BeliefMyLocation)
                return Beliefs.MyLocation;

            return Beliefs.NoBeliefFound;
        }
    }
    public LocationBelief() { data = new Vector3[1]; }

    public override void UpdateBelief(List<AgentBelief<Vector3>> beliefs)
    {
        base.UpdateBelief(beliefs);
        data = functionality.Do(agent);
    }

    [Serializable]
    public class BeliefMyLocation : ISensor<GoapAgent, Vector3>
    {
        public Vector3[] data { get; set; } = new Vector3[1];

        public Vector3[] Do(GoapAgent agent) => SetMyLocation(agent);
        Vector3[] SetMyLocation(GoapAgent agent)
        {
            data[0] = agent.transform.position;
            return data;
        }
    }

}

[Serializable]
public class BinaryBelief : AgentBelief<bool>
{
    [SerializeReference] public ISensor<GoapAgent, bool> functionality;

    [ShowInInspector, ReadOnly]
    [PropertyOrder(10)]
    public bool[] Is => functionality != null ? functionality.data : System.Array.Empty<bool>();
    [ShowInInspector]
    public override Beliefs type
    {
        get
        {
            if (functionality is Nothing)
                return Beliefs.Nothing;
            else if (functionality is BeliefAmIdling)
                return Beliefs.AmIdling;
            else if (functionality is BeliefCanSeePlayer)
                return Beliefs.CanSeePlayer;
            else if (functionality is BeliefCanAttackPlayer)
                return Beliefs.CanAttackPlayer;
            else
                Debug.LogError("No type found, please manually add it");

            return Beliefs.NoBeliefFound;
        }
    }
    public override void SatisfyAPrecondition(IBelief givenBelief, bool val)
    {
        //Debug.Log($"PreCheck Binary Belief for {type} : {Is[0]}");
        base.SatisfyAPrecondition(givenBelief, Is[0]);
        if (!Is[0])
            agent.OnPreconditionLost(givenBelief);
    }

    public override void SatisfyAnEffect(IBelief givenBelief, bool val)
    {
        //Debug.Log($"PreCheck Binary Belief for {type} : {Is[0]}");
        base.SatisfyAnEffect(givenBelief, Is[0]);

    }

    public BinaryBelief() { data = new bool[1]; }
    public override void UpdateBelief(List<AgentBelief<bool>> beliefs)
    {
        base.UpdateBelief(beliefs);
        Debug.Log($"Attempting update belief data, with functionality {functionality}");
        data = functionality.Do(agent);
    }

    [Serializable]
    public class Nothing : ISensor<GoapAgent, bool>
    {
        public bool[] data { get; set; } = new bool[1];

        public bool[] Do(GoapAgent agent) => DoNothing(agent);
        bool[] DoNothing(GoapAgent agent)
        {
            data[0] = true;
            return data;
        }
    }

    [Serializable]
    public class BeliefAmIdling : ISensor<GoapAgent, bool>
    {
        public bool[] data { get; set; } = new bool[1];

        public bool[] Do(GoapAgent agent) => IsIdling(agent);
        bool[] IsIdling(GoapAgent agent)
        {
            bool ret = true;

            if (agent.GetComponent<NavMeshAgent>().velocity.magnitude > 0.01f)
                ret = false;

            data[0] = ret;
            //Debug.Log($"Checking Idle {buffer[0]}");

            return data;
        }
    }

    [Serializable]
    public class BeliefCanSeePlayer : ISensor<GoapAgent, bool>
    {
        public bool[] data { get; set; } = new bool[1];
        public Sensor viewSensor;

        public bool[] Do(GoapAgent agent) => CanSeePlayer(agent);
        bool[] CanSeePlayer(GoapAgent agent)
        {
            bool ret = false;

            if (viewSensor.inRange.current == true)
                ret = true;
            else
                ret = false;

            data[0] = ret;
            Debug.Log($"Thinking Can SEE Player? {data[0]}");

            return data;
        }
    }

    [Serializable]
    public class BeliefCanAttackPlayer : ISensor<GoapAgent, bool>
    {
        public bool[] data { get; set; } = new bool[1];
        public Sensor attackSensor;

        public bool[] Do(GoapAgent agent) => CanAttackPlayer(agent);
        bool[] CanAttackPlayer(GoapAgent agent)
        {
            bool ret = false;

            if (attackSensor.inRange.current == true)
                ret = true;
            else
                ret = false;

            data[0] = ret;
            Debug.Log($"Thinking Can ATTACK Player? {data[0]}");

            return data;
        }
    }

}
