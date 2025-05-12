using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem instance;
    public static bool isInteracting;

    public GameObject[] allUI;

    private void Awake()
    {
        instance = this;
    }
}
