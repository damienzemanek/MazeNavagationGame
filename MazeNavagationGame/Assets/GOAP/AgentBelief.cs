using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    string key { get; }
    object boxedData { get; set; }
    float Confidence { get; set; }
    float timeStamp { get; set; }

    void UpdatePriorities(IReadOnlyList<IBelief> beliefs);

    public abstract void SetAgent(GoapAgent agent);
}


public class BeliefStates<T>
{
    private readonly Dictionary<string, Belief<T>> beliefs = new();

    public void Set(Belief<T> belief) => beliefs[belief.key] = belief;

    public bool TryGet(string key, float minimumConfidence, out Belief<T> belief)
    {
        if (beliefs.TryGetValue(key, out belief) && belief.Confidence >= minimumConfidence)
            return true;
        //Else
        belief = null;
        return false;
    }

    public bool IsTrue(string key, float minimumConfidence = 0.6f)
    {
        if(TryGet(key, minimumConfidence, out var belief))
        {
            if (belief.data is bool b) return b;
        }

        return false;
    }

}


[Serializable]
public abstract class Belief<T> : IBelief
{
    public string key { get; }
    public object boxedData { get => data; set => data = (T[])value; }
    [ShowInInspector] public float Confidence { get; set; }
    public float timeStamp { get; set; }

    public T[] data;
    public abstract T[] UpdatePriorities(List<Belief<T>> beliefs);

    public abstract void SetAgent(GoapAgent agent);

    public void UpdatePriorities(IReadOnlyList<IBelief> beliefs)
    {
        var interfaceBeliefsToTypeBeliefs = new List<Belief<T>>();

        foreach (var b in beliefs)
            if (b is Belief<T> beliefTyped)
                interfaceBeliefsToTypeBeliefs.Add(beliefTyped);

        data = UpdatePriorities(interfaceBeliefsToTypeBeliefs);
        timeStamp = Time.time;
    }

}


[Serializable]
public class AgentBelief<T> : Belief<T>
{
    protected GoapAgent agent;
    public string Name { get; }

    public AgentBelief() { }
    protected AgentBelief(string name) => Name = name;

    public override void SetAgent(GoapAgent agent)
    {
        this.agent = agent;
    }

    public override T[] UpdatePriorities(List<Belief<T>> beliefs) => data;
}

public interface IBeliefFunctionality<GoapAgent, T>
{
    public abstract T[] Do(GoapAgent agent, T[] data);

}

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
//    public bool MovingToPointOfInterest();
//}



[Serializable]
public class LocationBelief : AgentBelief<Vector3>
{
    [SerializeReference] public IBeliefFunctionality<GoapAgent, Vector3> functionality;

    public Vector3[] Location => data;


    public LocationBelief() { }

    public LocationBelief(string name, Vector3[] data) : base(name)
    {
        this.data = data;
    }
    public bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(agent.transform.position, pos) < range;

    public override Vector3[] UpdatePriorities(List<Belief<Vector3>> beliefs)
    {
        data = functionality.Do(agent, data);
        return Location;
    }

}


