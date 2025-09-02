using System;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(EntityController))]
public class Inventory : MonoBehaviour
{
    EntityController Controls;
    public Action<ItemSO> UseItem;


    [SerializeField] public InRangeDataEvent mInRangeData;
    [SerializeField] ItemSO PickedUpItem;
    [Serializable]
    public struct InRangeDataEvent
    {
        public bool inRange;
        public ItemSO item;
        public Action PickupCallBack;

        public void NotInRange()
        {
            inRange = false;
            item = null;
            PickupCallBack = null;
        }

        public void InRange(ItemSO _item, Action _PickupCallback)
        {
            inRange = true;
            item = _item;
            PickupCallBack = _PickupCallback;
        }
    }




    private void Awake()
    {
        Controls = GetComponent<EntityController>();
        mInRangeData = new InRangeDataEvent();
    }

    private void OnEnable()
    {
        Controls.pickup += AttemptPickup;
    }

    private void OnDisable()
    {
        Controls.pickup -= AttemptPickup;
    }

    public void AttemptPickup()
    {

        print("Attempting Pickup");
        //Guard Clauses
        if (!mInRangeData.inRange) return;

        //This would be so much easier with OnCollisionEnter because i could use the collider other to get my stuff
        PickedUpItem = mInRangeData.item;
        mInRangeData.PickupCallBack?.Invoke();
        UseItem?.Invoke(PickedUpItem);
        mInRangeData.NotInRange();

    }
}
