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
        transform.position = new(transform.position.x, transform.position.y, 0);

        if (objectToManipulate != null)
            objectToManipulate.SetActive(true);

        isPlaced = true;
    }

    private void CalculateSizeInCells()
    {
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        sizeInCells = new(Mathf.RoundToInt(collider.size.x), Mathf.RoundToInt(collider.size.y));
    }
}