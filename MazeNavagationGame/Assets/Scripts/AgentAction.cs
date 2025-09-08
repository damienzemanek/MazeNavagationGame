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

    public virtual void Initialize() { }

    public virtual void Start() => functionality.Start();

    public virtual void Update(float deltaTime)
    {
        if (functionality.CanExecute) functionality.Update(deltaTime);

        if (!functionality.Complete) return;

        Effects.ForEach(belief => belief.UpdateBelief(Effects.ToList()));

    }
    public virtual void Stop() => functionality.Stop();

}




