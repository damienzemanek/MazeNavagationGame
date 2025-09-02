using UnityEngine;

public class Breakable : MonoBehaviour, Location
{
    public ItemSO itemToUse;

    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        if (LocationServiceProvider.GetDistanceToPlayer(gameObject) <= 1f)
            inRange = true;
        else
            inRange = false;
    }

    public bool inRange 
    {
        get => PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.inRange;

        set
        {
            PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.inRange = value;

            // + Guard Clauses
            // + Not In range
            if (value == false) { PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.NotInRange(); return; }
            // + Has no Breakable script
            if (!gameObject.TryGetComponent(out Breakable breakable)) return;
            //Pickup the Actual item
            PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.InRange(breakable.Use, IsItemCorrect());
        }
    }

    public void Use()
    {

    }

    public bool IsItemCorrect()
    {
        if (PlayerSingleton.Instance.GetComponent<Inventory>().PickedUpItem == itemToUse)
            return true;
        else
            return false;
    }
}
