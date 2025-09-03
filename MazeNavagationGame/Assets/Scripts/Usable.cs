using System;
using UnityEngine;


[Serializable]
public abstract class Useable
{
    public abstract void Use();
}

[Serializable]
public class Break : Useable
{
    public GameObject effect;
    public override void Use() => BreakObject();
    void BreakObject()
    {
        effect.SetActive(true);
    }

}
