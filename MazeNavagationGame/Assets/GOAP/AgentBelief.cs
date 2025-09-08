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
    bool CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(IGoal goal);

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
        //Debug.Log("thinking updating belif");
        if (agent.currentGoal != null)
            if (CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(agent.currentGoal))
            {
                Debug.Log("thinking updating belif change callback");
                BeliefChangedCallback?.Invoke(originalGoal);

            }
        return data;
    }

    //Interface UpdateBelief -> Generic
    public void UpdateBelief(IReadOnlyList<IBelief> beliefs)
    {
        buffer.Clear();

        foreach (var b in beliefs)
            if (b is AgentBelief<T> beliefTyped)
            {
                Debug.Log("precon updating precons");
                SatisfyAPrecondition(agent.currentGoal, b);
                foreach (IGoal goal in agent.GoalPriorityQueue)
                    SatisfyAPrecondition(goal, b);

                buffer.Add(item: beliefTyped);
            }


        data = UpdateBelief(buffer);
        timeStamp = Time.time;
    }
    public virtual bool CheckIfBeliefIsRequiredEffectToAchieveCURRENTGoal(IGoal goal)
    {
        if(goal == null) return false;
        


    

        return true;

    }

    public void SatisfyAPrecondition(IGoal goal, IBelief givenBelief)
    {
        Debug.Log($"precon attempting to satify precon {goal.type} : {givenBelief.type}");
        var match =
            (goal.RequiredActionsToAchieveGoal ?? Enumerable.Empty<AgentAction>())
            .SelectMany(a => a?.Preconditions ?? Enumerable.Empty<IBelief>())
            .FirstOrDefault(b => b != null && b.type == givenBelief.type);

        if (match == null)
        {
            Debug.LogWarning($"No precondition of type {givenBelief.type} found for goal '{goal}'.");
            return;
        }
        if(givenBelief == match)
            match.satisfied = true;

        if (match.satisfied == true) Debug.Log($"precon SATISIFUIED++++ {match}");
    }

    public Action<IGoal> BeliefChangedCallback { get; set; }

    private readonly List<AgentBelief<T>> buffer = new List<AgentBelief<T>>(20);

    public bool Equals(AgentBelief<T> compare)
    {
        if (compare == null) return false;
        if(ReferenceEquals(this, compare)) return true;

        if (data == null || compare.data == null) return data == compare.data;

        if(data.Length != compare.data.Length) return false;

        var comparison = EqualityComparer<T>.Default;
        for(int i = 0; i < data.Length; i++)
            if (!comparison.Equals(data[i], compare.data[i])) return false;

        return true;
    }

}



