using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ItemUser : MonoBehaviour
{
    Inventory inventory;
    public Transform itemSpawnLocation;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        inventory.UseItem += EquipItem;
    }

    private void OnDisable()
    {
        inventory.UseItem -= EquipItem;
    }

    void EquipItem(ItemSO item)
    {
        print($"ItemUser: {gameObject.name} is equipping item {item.name}");
        GameObject equippedItem
            = Instantiate(item.itemPrefab, itemSpawnLocation.position, Quaternion.identity, gameObject.transform);

    }
}
