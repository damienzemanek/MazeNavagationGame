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
    bool condition { get; set; }
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
    bool CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(IGoal goal);

}


[Serializable]
public class AgentBelief<T> : IBelief
{
    //Variables/properties
    protected GoapAgent agent;
    [ShowInInspector][PropertyOrder(-10)] public virtual Beliefs type { get; }
    public Beliefs GetBelief() => type;
    [ShowInInspector][PropertyOrder(-20)] public bool condition { get; set; }
    public string key { get; }

    [SerializeField] bool _refreshing;
    public bool refreshing { get => _refreshing; set => _refreshing = value; }
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

        return (IBelief)clone;
   
    }

    //Generic UpdateBelief
    public virtual T[] UpdateBelief(List<AgentBelief<T>> beliefs)
    {
        if (agent.currentGoal != null)
            if(CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(agent.currentGoal))
                BeliefChangedCallback?.Invoke(originalGoal);

        condition = true;
        return data;
    }

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
    public virtual bool CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(IGoal goal)
    {
        if(goal == null) return false;

        return (goal.RequiredActionsToAchieveGoal?
            .Where(action => action != null)
            .SelectMany(action => action.Effects ?? Enumerable.Empty<IBelief>())
            .Any(belief => ReferenceEquals(belief, this)))
            ?? false;
    }
    public Action<IGoal> BeliefChangedCallback { get; set; }

    private readonly List<AgentBelief<T>> buffer = new List<AgentBelief<T>>(20);


}



