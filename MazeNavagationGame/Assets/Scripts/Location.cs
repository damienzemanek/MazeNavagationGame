using System;
using UnityEngine;


public interface Location 
{
    // public abstract bool inRange { get; set; }
    public abstract float withinRange { get; set; }
    public abstract float distToPlayer { get; }

    [Serializable]
    public struct InRange
    {
        public bool current;
        [HideInInspector] public bool last;

        public void Toggle(Action onEnter, Action onExit)
        {
            if (current == last) return;

            if (current) onEnter?.Invoke();
            else onExit?.Invoke();

            last = current;
        }
    }
}

public static class LocationServiceProvider
{
    public static float GetDistanceToPlayer(GameObject myObj)
    {
        Vector3 myPos = myObj.transform.transform.position;
        Vector3 plyrPos = PlayerSingleton.Instance.gameObject.transform.position;
        float dist = Vector3.Distance(myPos, plyrPos);

        return dist;
    }
}
