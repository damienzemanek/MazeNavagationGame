using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;
using UnityEngine.AI;


[Serializable]
public class AgentAction
{
    [SerializeReference] public List<IBelief> Preconditions = new List<IBelief>();
    [SerializeReference] public List<IBelief> Effects = new List<IBelief>();
    [SerializeReference] public IActionFunctionality functionality;
    public bool Complete => functionality.Complete;

    [ShowInInspector] public bool PreConditionsSatisfied => PreconditionsSatisfied();
    [ShowInInspector] public bool EffectsSatisfied => Effectssatisfied();


    public virtual void Initialize() { }

    public virtual bool CanStartThenStart() => functionality.CanStartThenStart();

    public virtual void Update(float deltaTime)
    {
        if (functionality == null) return;

        if (functionality.CanExecute) functionality.Update(deltaTime);

        if (!functionality.Complete) return;
    }
    public virtual void Stop() => functionality.Stop();

    bool PreconditionsSatisfied()
    {
        foreach (var precon in Preconditions)
            if (!precon.satisfied) return false;
        return true;
    }

    bool Effectssatisfied()
    {
        foreach (var eff in Effects)
            if (!eff.satisfied) return false;
        return true;
    }

    public void SatisfyPrecondition(IBelief belief)
    {
        Debug.Log("satisfied? : Action trying to satisfy cond " +  belief.type);
        foreach (var precon in Preconditions)
        {
            if (precon == null) continue;
            if (precon.type != belief.type) continue;
            precon.satisfied = true;

            Debug.Log(message: $"Precon {precon.type} satisfied? : {precon.satisfied}");

        }
    }

    public void SatisfyEffect(IBelief belief)
    {
        Debug.Log("satisfied? : Action trying to satisfy cond " + belief.type);
        foreach (IBelief effect in Effects)
        {
            if (effect == null) continue;
            if (effect.type != belief.type) continue;
            effect.satisfied = true;

            Debug.Log(message: $"Effect {effect.type} satisfied? : {effect.satisfied}");

        }
    }


    //Ai gen compare for the boxed data
    static bool BoxedDataEqual(object a, object b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        if (a.GetType() != b.GetType()) return false;

        // If both are arrays, compare element-by-element
        if (a is System.Array aa && b is System.Array bb)
            return aa.Length == bb.Length
                && aa.Cast<object>().SequenceEqual(bb.Cast<object>());

        // Fallback: normal equality
        return Equals(a, b);
    }
}




