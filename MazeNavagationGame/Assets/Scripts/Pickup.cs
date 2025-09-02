using UnityEngine;


public class Pickup : MonoBehaviour, Location
{
    public ItemSO item;

    public void PickedUpItem()
    {
        Destroy(gameObject);
    }

    public bool inRange
    {
        get => PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.inRange;

        set
        {
            PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.inRange = value;

            // + Guard Clauses
            // + Not In range
            if (value == false) { PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.NotInRange(); return; }
            // + Has no pickup script
            if (!gameObject.TryGetComponent(out Pickup pickup)) return;
            //Pickup the Actual item
            PlayerSingleton.Instance.GetComponent<Inventory>().PickupEvent.InRange(pickup.item, pickup.PickedUpItem);
        }
    }

    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        if (LocationServiceProvider.GetDistanceToPlayer(gameObject) <= 1f)
            inRange = true;
        else
            inRange = false;
    }
}
