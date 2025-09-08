using System;
using Sirenix.OdinInspector;
using UnityEngine;


public class Pickup : MonoBehaviour, Location
{
    public ItemSO item;
    public float dist;
    [SerializeField] public float distToPlayer { get => LocationServiceProvider.GetDistanceToPlayer(gameObject); }
    [SerializeField] public Location.InRange inRange;
    [ShowInInspector] public float withinRange { get => item.withinRange; }

    float Location.distToPlayer => throw new NotImplementedException();

    public void PickedUpItem()
    {
        Destroy(gameObject);
    }


    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        dist = distToPlayer;
        if (PlayerSingleton.Instance.gameObject.GetComponent<Inventory>().PickedUpItem != null) return;
        if (!gameObject.TryGetComponent(out Pickup pickup)) return;

        if (distToPlayer <= withinRange)
        {
            inRange.current = true;

            inRange.Toggle(
                () => PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.InRange(pickup.item, pickup.PickedUpItem),
                () => PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.NotInRange()
            );
        }
        else
        {
            inRange.current = false;

            inRange.Toggle(
                () => PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.InRange(item, PickedUpItem),
                () => PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.NotInRange()
            );
        }
    }
}
