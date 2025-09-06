using UnityEngine;


public class Sensor : MonoBehaviour, Location
{
    public float dist;
    [SerializeField] public float distToPlayer { get => LocationServiceProvider.GetDistanceToPlayer(gameObject); }
    [SerializeField] public Location.InRange inRange;
    [SerializeField] float _withinRange;
    public float withinRange { get => _withinRange; set => _withinRange = value; }

    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        dist = distToPlayer;
        if (PlayerSingleton.Instance.gameObject.GetComponent<Inventory>().PickedUpItem != null) return;

        if (distToPlayer <= withinRange)
        {
            inRange.current = true;

            
        }
        else
        {
            inRange.current = false;

            
        }
    }

    public void OnDrawGizmos()
    {

        if (!(this is Location loc) || PlayerSingleton.Instance == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center: transform.position, withinRange);
            return;
        }
        Gizmos.color = loc.IsInRange() ? Color.red : Color.green;
        Gizmos.DrawWireSphere(center: transform.position, withinRange);
    }


}
