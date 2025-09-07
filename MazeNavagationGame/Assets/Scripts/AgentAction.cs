using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;

public class AgentAction 
{
    public string Name { get; }
    [ShowInInspector] public float Cost { get; private set; }

    public HashSet<AgentBelief<T>> Preconditions { get; } = new HashSet<AgentBelief<T>>();
    public HashSet<AgentBelief<T>> Effects { get; } = new HashSet<AgentBelief<T>>();

    IActionStrategy strategy;
    public bool Complete => strategy.Complete;

    public void Start() => strategy.Start();

    public void Update(float deltaTime)
    {
        if(strategy.CanExecute) strategy.Update(deltaTime);

        if (!strategy.Complete) return;

        Effects.ForEach(belief => belief.UpdatePriorities(Effects.ToList()));

    }
    public void Stop() => strategy.Stop();
}
