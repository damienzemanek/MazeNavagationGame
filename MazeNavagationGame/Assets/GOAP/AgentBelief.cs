using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


public enum BeliefType
{
    MyLocation,
    SeesPointOfInterest,
    MovingToPointOfInterest,
    Healthy
}

//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    string key { get; }
    BeliefType type { get; }
    object boxedData { get; set; }
    float Confidence { get; set; }
    float timeStamp { get; set; }

    void UpdatePriorities(IReadOnlyList<IBelief> beliefs);
}


[Serializable]
public abstract class Belief<T> : IBelief
{
    public string key { get; }

    [ShowInInspector] public BeliefType type { get; set; }
    public object boxedData { get => data; set => data = (T)value; }
    [ShowInInspector]  public float Confidence { get; set; }
    public float timeStamp { get; set; }

    public T data;
    public abstract T UpdatePriorities(List<Belief<T>> beliefs);

    public void UpdatePriorities(IReadOnlyList<IBelief> beliefs)
    {
        UpdatePriorities(beliefs);
        timeStamp = Time.time;
    }

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
public class AgentBelief<T> : Belief<T>
{
    protected GoapAgent agent;
    public string Name { get; }

    Func<T> valueFunc;

    public AgentBelief() { }
    protected AgentBelief(string name) => Name = name;

    public override T UpdatePriorities(List<Belief<T>> beliefs) => valueFunc();
}




[Serializable]
public class LocationBelief : AgentBelief<Vector3>
{
    Func<Vector3> observedLocation = () => Vector3.zero;

    public LocationBelief() { }

    public LocationBelief(string name, Func<Vector3> observedLocation) : base(name)
    {
        this.observedLocation = observedLocation;
    }

    public Vector3 Location => observedLocation();
    public bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(agent.transform.position, pos) < range;

    public override Vector3 UpdatePriorities(List<Belief<Vector3>> beliefs)
    {
        return observedLocation();
    }

    public bool UpdatePriorities(List<Belief<Vector3>> beliefs, float dist)
    {
        return InRangeOf(agent.transform.position, dist);
    }

    //Incr Confidence
    bool SeesPointOfInterest()
    {

        return false;
    }

    //Incr Confidence
    bool MovingToPointOfInterest()
    {
        return false;
    }
}


