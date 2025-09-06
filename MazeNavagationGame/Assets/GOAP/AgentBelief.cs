using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[Serializable]
public abstract class Belief
{
    public abstract bool Evaluate();
}

[Serializable]
public class AgentBelief : Belief
{
    protected GoapAgent agent;
    public string Name { get; }

    Func<bool> condition = () => false;

    public AgentBelief() { }
    protected AgentBelief(string name)
    {
        Name = name;
    }

    public override bool Evaluate() => condition();

}

[Serializable]
public class LocationBelief : AgentBelief
{
    Func<Vector3> observedLocation = () => Vector3.zero;

    public LocationBelief() { }

    public LocationBelief(string name, Func<Vector3> observedLocation) : base(name)
    {
        this.observedLocation = observedLocation;
    }

    public Vector3 Location => observedLocation();
    public bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(agent.transform.position, pos) < range;

    //Default Override
    public override bool Evaluate()
    {
        return InRangeOf(Location, 5f);
    }

    //Overload
    public bool Evaluate(float dist)
    {
        return InRangeOf(agent.transform.position, dist);
    }
}







public class BeliefFactory
{
    readonly GoapAgent agent;
    readonly Dictionary<string, AgentBelief> beliefs;

    public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
    {
        this.agent = agent;
        this.beliefs = beliefs;
    }

}

