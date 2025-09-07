using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using static BinaryBelief;

public interface ISensor<GoapAgent, T>
{
    public abstract T[] Do(GoapAgent agent, T[] data);

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
        {Beliefs.AmIdling, 0.49f}
    };

}

public enum Beliefs
{
    NoBeliefFound,
    Nothing,
    MyLocation,
    AmIdling
}



[Serializable]
public class LocationBelief : AgentBelief<Vector3>
{
    [SerializeReference] public ISensor<GoapAgent, Vector3> functionality;

    public Vector3[] Location => data;
    [ShowInInspector] public override Beliefs type
    {
        get
        {
            if (functionality is BeliefMyLocation)
                return Beliefs.MyLocation;

            return Beliefs.NoBeliefFound;
        }
    }
    public LocationBelief() { data = new Vector3[1]; }

    public override Vector3[] UpdateBelief(List<AgentBelief<Vector3>> beliefs)
    {
        data = functionality.Do(agent, data);
        return Location;
    }

    [Serializable]
    public class BeliefMyLocation : ISensor<GoapAgent, Vector3>
    {
        private readonly Vector3[] buffer = new Vector3[1];

        public Vector3[] Do(GoapAgent agent, Vector3[] data) => SetMyLocation(agent, data);
        Vector3[] SetMyLocation(GoapAgent agent, Vector3[] data)
        {
            buffer[0] = agent.transform.position;
            return buffer;
        }
    }

}

[Serializable]
public class BinaryBelief : AgentBelief<bool>
{
    [SerializeReference] public ISensor<GoapAgent, bool> functionality;

    public bool[] Is => data;
    [ShowInInspector] public override Beliefs type
    {
        get
        {
            if (functionality is Nothing)
                return Beliefs.Nothing;
            else if(functionality is BeliefAmIdling)
                return Beliefs.AmIdling;

            return Beliefs.NoBeliefFound;
        }
    }

    public BinaryBelief() { data = new bool[1]; }
    public override bool[] UpdateBelief(List<AgentBelief<bool>> beliefs)
    {
        data = functionality.Do(agent, data);
        return Is;
    }

    [Serializable]
    public class Nothing : ISensor<GoapAgent, bool>
    {
        private readonly bool[] buffer = new bool[1];

        public bool[] Do(GoapAgent agent, bool[] data) => DoNothing(agent, data);
        bool[] DoNothing(GoapAgent agent, bool[] data)
        {
            buffer[0] = true;
            return buffer;
        }
    }

    [Serializable]
    public class BeliefAmIdling : ISensor<GoapAgent, bool>
    {
        private readonly bool[] buffer = new bool[1];

        public bool[] Do(GoapAgent agent, bool[] data) => IsIdling(agent, data);
        bool[] IsIdling(GoapAgent agent, bool[] data)
        {
            bool ret = true;

            if (agent.GetComponent<NavMeshAgent>().velocity.magnitude > 0.01f)
                ret = false;

            buffer[0] = ret;
            Debug.Log($"Checking Idle {buffer[0]}");

            return buffer;
        }
    }

}
