using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Location;


public class UseWithItem : MonoBehaviour, Location
{
    public bool locationInRange;
    public float dist;
    [SerializeField] public float distToPlayer { get => LocationServiceProvider.GetDistanceToPlayer(gameObject); }
    [SerializeField] public Location.InRange inRange;

    public ItemSO itemToUse;
    [SerializeReference] public List<Useable> uses;

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


    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        dist = distToPlayer;
        if (!IsItemCorrect()) return;
        if (!gameObject.TryGetComponent(out UseWithItem usable)) return;

        if (distToPlayer <= 1f)
        {
            inRange.current = true;

            inRange.Toggle(
                () => PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.InRange(usable.Use),
                () => PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.NotInRange()
            );
        }
        else
        {
            inRange.current = false;

            inRange.Toggle(
                () => PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.InRange(usable.Use),
                () => PlayerSingleton.Instance.GetComponent<Inventory>().UseEvent.NotInRange()
            );
        }
    }
}
