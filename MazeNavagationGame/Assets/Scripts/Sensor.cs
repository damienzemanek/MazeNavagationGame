using UnityEngine;


public class Sensor : MonoBehaviour, Location
{
    public string Name;
    public float dist;
    [SerializeField] public float distToPlayer { get => LocationServiceProvider.GetDistanceToPlayer(gameObject); }
    [SerializeField] public Location.InRange inRange;
    [SerializeField] float _withinRange;
    [SerializeField] public Transform lastSeenLoc;
    public float withinRange { get => _withinRange; set => _withinRange = value; }

    //Using update cause you dont want me to use colliders. 
    private void Update()
    {
        dist = distToPlayer;

        if (distToPlayer <= withinRange)
        {
            inRange.current = true;
            lastSeenLoc = PlayerSingleton.Instance.gameObject.transform;
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
