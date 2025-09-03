using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public abstract class Useable
{
    public abstract void Use();
}

[Serializable]
public class Break : Useable
{
    public GameObject effect;
    public float breakTime;
    public override void Use() => BreakObject();
    void BreakObject()
    {
        effect.SetActive(true);

        GameObject ObjectBeingBroken = effect.transform.parent.gameObject;
        WaitExtension.Wait(PlayerSingleton.Instance, breakTime, () => GameObject.Destroy(ObjectBeingBroken));
    }

}

[Serializable]
public class Progression : Useable
{
    public int progress;
    public int maxProgres;
    public float progressEffectTime;

    public GameObject effect;
    public GameObject completeEffect;

    public UnityEvent OnComplete;
    public override void Use() => Progress();
    void Progress()
    {
        effect.SetActive(true);
        WaitExtension.Wait(PlayerSingleton.Instance, progressEffectTime, () => effect.SetActive(false));

        progress++;

        if (progress >= maxProgres)
            Complete();
    }

    void Complete()
    {
        Debug.Log("Progression Objective Complete");
        completeEffect.SetActive(true);
        effect.SetActive(false);
        OnComplete?.Invoke();
    }

}
