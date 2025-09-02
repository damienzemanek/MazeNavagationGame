using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    public string itenName;
    public GameObject itemPrefab;
    public GameObject pickupPrefab;
}
