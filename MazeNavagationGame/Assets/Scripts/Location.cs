using System;
using UnityEngine;

public class Location : MonoBehaviour
{
    bool inRange
    {
        get => PlayerSingleton.Instance.GetComponent<Inventory>().mInRangeData.inRange;

        set
        {
            PlayerSingleton.Instance.GetComponent<Inventory>().mInRangeData.inRange = value;

            // + Guard Clauses
            // + Not In range
            if (value == false)  { PlayerSingleton.Instance.GetComponent<Inventory>().mInRangeData.NotInRange(); return;  }
            // + Has no pickup script
            if (!gameObject.TryGetComponent(out Pickup pickup)) return;
            //Pickup the Actual item
            PlayerSingleton.Instance.GetComponent<Inventory>().mInRangeData.InRange(pickup.item, pickup.PickedUpItem);
        }
    }

    float distToPlayer { get => GetDistanceToPlayer(); }
    public Action doAction;

    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        if (distToPlayer <= 1f)
            inRange = true;
        else
            inRange = false;
    }


    float GetDistanceToPlayer()
    {
        Vector3 myPos = gameObject.transform.transform.position;
        Vector3 plyrPos = PlayerSingleton.Instance.gameObject.transform.position;
        float dist = Vector3.Distance(myPos, plyrPos);
        return dist;
    }
}
