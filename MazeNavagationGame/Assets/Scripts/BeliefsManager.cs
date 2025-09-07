using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Nothing Belief", menuName = "ScriptableObjects/Beliefs")]
[Serializable]
public class NothingBelief : AgentBelief<bool>
{
    [SerializeReference] public IBeliefFunctionality<GoapAgent, bool> functionality;

    public bool[] Nothing => data;

    public NothingBelief(string name) : base(name) { }

    public NothingBelief(string name, bool[] data) : base(name)
    {
        this.data = data;
    }
    public override bool[] UpdatePriorities(List<AgentBelief<bool>> beliefs)
    {
        data = functionality.Do(agent, data);
        return Nothing;
    }

}

public interface IBeliefFunctionality<GoapAgent, T>
{
    public abstract T[] Do(GoapAgent agent, T[] data);

}

[Serializable]
class BeliefMyLocation : IBeliefFunctionality<GoapAgent, Vector3>
{
    private readonly Vector3[] buffer = new Vector3[1];

    public Vector3[] Do(GoapAgent agent, Vector3[] data) => SetMyLocation(agent, data);
    Vector3[] SetMyLocation(GoapAgent agent, Vector3[] data)
    {
        buffer[0] = agent.transform.position;
        return buffer;
    }
}

//class IBeliefSeesPointOfInterest : IBeliefFunctionality
//{
//    public bool SeesPointOfInterest();
//}

//class IBeliefMovingToPointOfInterest : IBeliefFunctionality
//{
//    public bool M0ovingToPointOfInterest();
//}



[CreateAssetMenu(fileName = "Location Belief", menuName = "ScriptableObjects/Beliefs/Location")]
[Serializable]
public class LocationBelief : AgentBelief<Vector3>
{
    [SerializeReference] public IBeliefFunctionality<GoapAgent, Vector3> functionality;

    public Vector3[] Location => data;

    public LocationBelief(string name) : base(name) { }

    public LocationBelief(string name, Vector3[] data) : base(name)
    {
        this.data = data;
    }
    public override Vector3[] UpdatePriorities(List<AgentBelief<Vector3>> beliefs)
    {
        data = functionality.Do(agent, data);
        return Location;
    }

}

