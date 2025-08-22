using UnityEngine;

public class PlacableObject : MonoBehaviour
{
    public bool isPlaced;
    public Vector2Int sizeInCells;
    public GameObject objectToManipulate;
    public InteractableObject _interactableObject;

    private void Start()
    {
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        Destroy(GetComponent<ObjectDrag>());
        Destroy(GetComponent<TriggerController>());
        BuildingSystem.instance._objectToPlace = null;

        if (objectToManipulate != null)
            objectToManipulate.SetActive(true);

        isPlaced = true;
    }

    private void CalculateSizeInCells()
    {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        sizeInCells = new(Mathf.RoundToInt(collider.size.x), Mathf.RoundToInt(collider.size.y));
    }
}