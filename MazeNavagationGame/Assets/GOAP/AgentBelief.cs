using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    string Name { get; }
    Func<bool> condition { get; set; }
    bool refreshing { get; set; }
    float refreshDelay { get; set; }
    object boxedData { get; set; }
    float timeStamp { get; set; }

    void UpdateBelief(IReadOnlyList<IBelief> beliefs);
    void SetAgent(GoapAgent agent);
    float GetRefreshDelay();
     bool EvaluateCondition();
}


[Serializable]
public class AgentBelief<T> : IBelief
{
    //Variables/properties
    protected GoapAgent agent;
    public string Name { get; set; }
    public Func<bool> condition { get; set; } = () => false;
    public string key { get; }
    [SerializeField] public bool refreshing { get; set; }

    [SerializeField][ShowIf("refreshing")] float _refreshDelay;
    public float refreshDelay { get => _refreshDelay; set => _refreshDelay = value; }
    public object boxedData { get => data; set => data = (T[])value; }
    public float timeStamp { get; set; }

    public T[] data;
    public T[] GetData() => data;

    //Methods
    public virtual void SetAgent(GoapAgent agent) => this.agent = agent;
    public virtual float GetRefreshDelay() => refreshDelay;
    protected AgentBelief() { }
    protected AgentBelief(string name) => Name = name;

    public bool EvaluateCondition() => condition();



    //Generic UpdateBelief
    public virtual T[] UpdateBelief(List<AgentBelief<T>> beliefs) => data;

    //Interface UpdateBelief -> Generic
    public void UpdateBelief(IReadOnlyList<IBelief> beliefs)
    {
        buffer.Clear();

        foreach (var b in beliefs)
            if (b is AgentBelief<T> beliefTyped)
                buffer.Add(beliefTyped);

        data = UpdateBelief(buffer);
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