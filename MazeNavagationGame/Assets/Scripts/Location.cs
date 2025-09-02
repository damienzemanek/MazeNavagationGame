using System;
using UnityEngine;


public interface Location 
{
    public abstract bool inRange { get; set; }
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
