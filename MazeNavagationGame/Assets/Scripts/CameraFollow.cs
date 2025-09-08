using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 PlayerLoc { get => PlayerSingleton.Instance.gameObject.transform.position; }
    public float hiehgt = 10;

    private void LateUpdate()
    {
        gameObject.transform.position = new Vector3(PlayerLoc.x, PlayerLoc.y + hiehgt, PlayerLoc.z);
    }
}
