using UnityEngine;


[RequireComponent(typeof(Location))]
public class Pickup : MonoBehaviour
{
    public ItemSO item;

    public void PickedUpItem()
    {
        Destroy(gameObject);
    }
}
