using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance;

    private void Awake()
    {
        Instance = this;
    }
}
