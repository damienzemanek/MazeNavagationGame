using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 PlayerLoc { get => PlayerSingleton.Instance.gameObject.transform.position; }

    private void LateUpdate()
    {
        gameObject.transform.position = new Vector3(PlayerLoc.x, 20, PlayerLoc.z);
    }
}
