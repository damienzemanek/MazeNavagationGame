using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;


[Serializable]
public class AgentAction
{
    public virtual string Name { get; set; }
    [ShowInInspector] public virtual float Cost { get; private set; }

    [SerializeField] public List<AgentBelief<T>> Preconditions = new List<AgentBelief<T>>();
    [SerializeField] public List<AgentBelief<T>> Effects = new List<AgentBelief<T>>();

    IActionStrategy strategy;
    public bool Complete => strategy.Complete;

    public virtual void Initialize() { }

    public virtual void Start() => strategy.Start();

    public virtual void Update(float deltaTime)
    {
        if (strategy.CanExecute) strategy.Update(deltaTime);

        if (!strategy.Complete) return;

        Effects.ForEach(belief => belief.UpdateBelief(Effects.ToList()));

    }
    public virtual void Stop() => strategy.Stop();
}
