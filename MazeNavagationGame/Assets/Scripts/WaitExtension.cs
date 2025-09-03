using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class WaitExtension
{
    public static void Wait(this MonoBehaviour mono, float delay, UnityAction callback)
    {
        mono.StartCoroutine(ExecuteAction(delay, callback));
    }

    public static void WaitAFrame(this MonoBehaviour mono, UnityAction callback)
    {
        mono.StartCoroutine(ExecuteAfterAFRame(callback));
    }

    private static IEnumerator ExecuteAction(float delay, UnityAction callback)
    {
        yield return new WaitForSecondsRealtime(delay);
        callback?.Invoke();
        yield break;
    }

    private static IEnumerator ExecuteAfterAFRame(UnityAction callback)
    {
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
        yield break;
    }
}