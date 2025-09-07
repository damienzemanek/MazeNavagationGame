using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using Sirenix.Serialization;


//Interface for ODIN Serialization for inspector visual
public interface IBelief
{
    Beliefs type { get; }
    Func<bool> condition { get; set; }
    bool refreshing { get; set; }
    float refreshDelay { get; }
    object boxedData { get; set; }
    float timeStamp { get; set; }

    void UpdateBelief(IReadOnlyList<IBelief> beliefs);
    void SetAgent(GoapAgent agent);
    float GetRefreshDelay();
    bool EvaluateCondition();
    IBelief CreateCopy(IBelief copy);

    Beliefs GetBelief();

}


[Serializable]
public class AgentBelief<T> : IBelief
{
    //Variables/properties
    protected GoapAgent agent;
    [ShowInInspector][PropertyOrder(-10)] public virtual Beliefs type { get; }
    public Beliefs GetBelief() => type;
    public Func<bool> condition { get; set; } = () => false;
    public string key { get; }
    [SerializeField] public bool refreshing { get; set; }
    [ShowInInspector] public float refreshDelay
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

    public T[] data;
    public T[] GetData() => data;

    //Methods
    public virtual void SetAgent(GoapAgent agent) => this.agent = agent;
    public virtual float GetRefreshDelay() => refreshDelay;
    protected AgentBelief() { }
    protected AgentBelief(Beliefs type) => this.type = type;

    public bool EvaluateCondition() => condition();

    public virtual IBelief CreateCopy(IBelief copy)
    {
        if (copy == null) return null;

        var clone = SerializationUtility.CreateCopy(copy);

        return (IBelief)clone;
   
    }

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



