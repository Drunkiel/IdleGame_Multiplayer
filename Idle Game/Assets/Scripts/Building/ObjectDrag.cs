using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    public Vector3 offSet;

    private void OnMouseDown()
    {
        offSet = transform.position - MouseMovement.instance.GetPosition();
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = MouseMovement.instance.GetPosition();
        Vector3 position = mousePosition + new Vector3(offSet.x, offSet.y, 0);
        transform.position = BuildingSystem.instance.SnapCoordinateToGrid(position);
        BuildingSystem.instance.ModifyCollider(GetComponent<TriggerController>().isTriggered);
    }
}