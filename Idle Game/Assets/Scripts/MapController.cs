using UnityEngine;

public class MapController : MonoBehaviour
{
    public Grid grid;

    void Start()
    {
        BuildingSystem.instance.grid = grid;
    }
}
