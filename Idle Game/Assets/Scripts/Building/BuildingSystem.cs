using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

[Serializable]
public class Building
{
    public BuildingID _buildingID;
    public List<BuildingRecipe> _buildingRecipes;
}

[Serializable]
public class BuildingRecipe
{
    public short itemID;
    public int quantity;
}

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem instance;
    public static bool inBuildingMode;

    public List<Building> _buildings;
    public Grid grid;
    public Vector2 mapSize;

    [SerializeField] private GameObject decisionPanel;
    [SerializeField] private GameObject colliderPrefab;
    public PlacableObject _objectToPlace;
    private GameObject objectCollider;

    void Awake()
    {
        instance = this;
    }

    public void BuildingManager(int index)
    {
        InitializeWithObject(_buildings[index]._buildingID.gameObject);
        UIController.instance.Close(3);
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = grid.WorldToCell(position);

        //Checking if object is out of bounds
        if (position.x > mapSize.x ||
            position.x < -mapSize.x ||
            position.z > mapSize.y ||
            position.z < -mapSize.y
            ) return SnapCoordinateToGrid(Vector3.zero);

        position = grid.GetCellCenterWorld(cellPosition);
        return new(position.x, position.y, -1);
    }

    public void InitializeWithObject(GameObject prefab)
    {
        if (inBuildingMode)
            return;

        inBuildingMode = true;

        GameObject newObject = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
        _objectToPlace = newObject.GetComponent<PlacableObject>();
        newObject.transform.position = SnapCoordinateToGrid(transform.position);
        newObject.AddComponent<ObjectDrag>();
        objectCollider = Instantiate(colliderPrefab, newObject.transform);

        OpenUI(true);
    }

    public void OpenUI(bool destroy)
    {
        //UI
        decisionPanel.SetActive(true);
        Button acceptButton = decisionPanel.transform.GetChild(0).GetComponent<Button>();
        Button declineButton = decisionPanel.transform.GetChild(1).GetComponent<Button>();

        //Removing listeners
        acceptButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();

        //Adding new listeners
        acceptButton.onClick.AddListener(() => PlaceButton());

        if (destroy)
            declineButton.onClick.AddListener(() =>
            {
                Destroy(_objectToPlace.gameObject);
                decisionPanel.SetActive(false);
                inBuildingMode = false;
            });
        else
        {
            Vector3 oldPosition = _objectToPlace.transform.position;
            declineButton.onClick.AddListener(() =>
            {
                _objectToPlace.transform.position = oldPosition;
                PlaceButton();
            });
        }
    }

    public void PlaceButton()
    {
        if (CanBePlaced())
            _objectToPlace.Place();
        else
            Destroy(_objectToPlace.gameObject);

        decisionPanel.SetActive(false);
        inBuildingMode = false;
        Destroy(objectCollider);
        UIController.instance.Open(3);
    }

    private bool CanBePlaced()
    {
        if (_objectToPlace == null)
            return false;

        return _objectToPlace.transform.GetComponent<TriggerController>().isTriggered;
    }

    public void ModifyCollider(bool value)
    {
        if (value)
            objectCollider.GetComponent<SpriteRenderer>().color = Color.green;
        else
            objectCollider.GetComponent<SpriteRenderer>().color = Color.red;
    }
}