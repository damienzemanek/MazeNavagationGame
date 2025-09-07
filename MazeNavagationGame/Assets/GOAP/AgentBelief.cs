using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    string Name { get; }
    float refreshDelay { get; set; }
    object boxedData { get; set; }
    float timeStamp { get; set; }

    void UpdatePriorities(IReadOnlyList<IBelief> beliefs);

    public abstract void SetAgent(GoapAgent agent);

    public abstract float GetRefreshDelay();
}



[Serializable]
public class AgentBelief<T> : ScriptableObject, IBelief
{
    //Variables/properties
    protected GoapAgent agent;
    public string Name { get; set; }
    public string key { get; }
    [SerializeField] float _refreshDelay;
    public float refreshDelay { get => _refreshDelay; set => _refreshDelay = value; }
    public object boxedData { get => data; set => data = (T[])value; }
    public float timeStamp { get; set; }

    public T[] data;

    //Methods
    public virtual void SetAgent(GoapAgent agent) => this.agent = agent;
    public virtual float GetRefreshDelay() => refreshDelay;
    protected AgentBelief(string name) => Name = name;

    //Cloner
    public static AgentBelief<T> CreateCopy(AgentBelief<T> copy)
    {
        return Instantiate(copy);
    }


    //Generic UpdatePriorities
    public virtual T[] UpdatePriorities(List<AgentBelief<T>> beliefs) => data;

    //Interface UpdatePriorities -> Generic
    public void UpdatePriorities(IReadOnlyList<IBelief> beliefs)
    {
        buffer.Clear();

        foreach (var b in beliefs)
            if (b is AgentBelief<T> beliefTyped)
                buffer.Add(beliefTyped);

        data = UpdatePriorities(buffer);
        timeStamp = Time.time;
    }
    private readonly List<AgentBelief<T>> buffer = new List<AgentBelief<T>>(20);


}





public class BeliefStates<T>
{
    private readonly Dictionary<string, AgentBelief<T>> beliefs = new();

    public void Set(AgentBelief<T> belief) => beliefs[belief.Name] = belief;

    public bool TryGet(string key, out AgentBelief<T> belief)
    {
        if (beliefs.TryGetValue(key, out belief))
            return true;
        //Else
        belief = null;
        return false;
    }

    public bool IsTrue(string key, float minimumConfidence = 0.6f)
    {
        if (TryGet(key, out var belief))
        {
            if (belief.data is bool b) return b;
        }

        return false;
    }

}