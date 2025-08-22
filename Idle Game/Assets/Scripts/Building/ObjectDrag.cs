using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offSet;

    // private void OnMouseDown()
    // {
    //     offSet = transform.position - BuildingSystem.GetMouseWorldPosition();
    // }

    // private void OnMouseDrag()
    // {
    //     Vector3 mousePosition = BuildingSystem.GetMouseWorldPosition();
    //     Vector3 position = mousePosition + new Vector3(offSet.x, 0, offSet.z);
    //     transform.position = BuildingSystem.instance.SnapCoordinateToGrid(position);
    // }
}