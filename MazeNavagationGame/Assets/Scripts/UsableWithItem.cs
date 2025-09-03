using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UseWithItem : MonoBehaviour, Location
{
    public ItemSO itemToUse;
    [SerializeReference] public List<Useable> uses;


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
            if (!gameObject.TryGetComponent(out UseWithItem usable)) return;
            //Pickup the Actual item
            PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.InRange(usable.Use, IsItemCorrect());
        }
    }

    public void Use()
    {
        print("Using");
        for(int i = 0; i < uses.Count; i++)
            uses[i].Use();

        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public bool IsItemCorrect()
    {
        if (PlayerSingleton.Instance.GetComponent<Inventory>().PickedUpItem == itemToUse)
            return true;
        else
            return false;
    }
}
