using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    Beliefs type { get; }
    bool satisfied { get; set; }
    bool refreshing { get; set; }
    float refreshDelay { get; }
    object boxedData { get; set; }
    float timeStamp { get; set; }

    IGoal originalGoal { get; set; }

    void UpdateBelief(IReadOnlyList<IBelief> beliefs);
    void SetAgent(GoapAgent agent);
    float GetRefreshDelay();
    IBelief CreateCopy(IBelief copy);
    Beliefs GetBelief();
    Action<IGoal> BeliefChangedCallback { get; set; }
}


[Serializable]
public class AgentBelief<T> : IBelief
{
    //Variables/properties
    protected GoapAgent agent;
    [ShowInInspector][PropertyOrder(-10)] public virtual Beliefs type { get; }
    public Beliefs GetBelief() => type;
    [ShowInInspector][PropertyOrder(-20)] public bool satisfied { get; set; }
    public string key { get; }

    [SerializeField] bool _refreshing;
    public bool refreshing { get => _refreshing; set => _refreshing = value; }
    [ShowInInspector]
    public float refreshDelay
    {
        get
        {
            float val;
            BeliefsManager.refreshRates.TryGetValue(GetBelief(), out val);
            return val;
        }
    }
    public object boxedData { get => data; set => data = (T[])value; }
    public float timeStamp { get; set; }

    public virtual T[] data { get; set; }
    [ShowInInspector] public IGoal originalGoal { get; set; }

    //Methods
    public virtual void SetAgent(GoapAgent agent) => this.agent = agent;
    public virtual float GetRefreshDelay() => refreshDelay;
    protected AgentBelief() { }
    protected AgentBelief(Beliefs type) => this.type = type;

    public virtual IBelief CreateCopy(IBelief copy)
    {
        if (copy == null) return null;
        var clone = SerializationUtility.CreateCopy(copy);
        //data = new T[data.Length];

        return (IBelief)clone;

    }

    public virtual void UpdateBelief(List<AgentBelief<T>> beliefs) { }

    //Interface UpdateBelief -> Generic
    public void UpdateBelief(IReadOnlyList<IBelief> beliefs)
    {
        buffer.Clear();

        if (agent.node == null) Debug.LogError("No Node found on Agent");

        foreach (var b in beliefs)
            if (b is AgentBelief<T> beliefTyped && b.type == type)
            {
                Debug.Log("precon updating precons");
                SatisfyAPrecondition(b);
                SatisfyAnEffect(b);
                buffer.Add(item: beliefTyped);
            }


        UpdateBelief(buffer);
        timeStamp = Time.time;
    }

    public virtual void SatisfyAPrecondition(IBelief givenBelief)
    {
        agent.SatisfyPrecondition(givenBelief);
    }

    public virtual void SatisfyAnEffect(IBelief givenBelief)
    {
        agent.SatisfyEffect(givenBelief);
    }

    public Action<IGoal> BeliefChangedCallback { get; set; }

    private readonly List<AgentBelief<T>> buffer = new List<AgentBelief<T>>(20);

    public bool Equals(AgentBelief<T> compare)
    {
        if (compare == null) return false;
        if (ReferenceEquals(this, compare)) return true;

        if (data == null || compare.data == null) return data == compare.data;

        if (data.Length != compare.data.Length) return false;

        var comparison = EqualityComparer<T>.Default;
        for (int i = 0; i < data.Length; i++)
            if (!comparison.Equals(data[i], compare.data[i])) return false;

        return true;
    }

}



