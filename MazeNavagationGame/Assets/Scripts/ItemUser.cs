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
        inventory.StopUsingItem += StopUsingItem;
    }

    private void OnDisable()
    {
        inventory.UseItem -= EquipItem;
        inventory.StopUsingItem -= StopUsingItem;

    }

    void EquipItem(ItemSO item)
    {
        print($"ItemUser: {gameObject.name} is equipping item {item.name}");
        GameObject equippedItem
            = Instantiate(item.itemPrefab, itemSpawnLocation.position, Quaternion.identity, itemSpawnLocation.transform);

    }

    void StopUsingItem()
    {
        for(int i = 0; i < itemSpawnLocation.childCount; i++)
            Destroy(itemSpawnLocation.GetChild(i).gameObject);

    }

}
