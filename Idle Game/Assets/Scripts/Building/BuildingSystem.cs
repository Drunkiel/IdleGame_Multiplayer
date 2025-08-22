using UnityEngine;
using UnityEngine.UI;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem instance;
    public static bool inBuildingMode;

    [SerializeField] private Grid grid;
    public Vector2 mapSize;

    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject buildingUI;
    public PlacableObject _objectToPlace;

    void Awake()
    {
        instance = this;
    }

    public void BuildingManager()
    {
        inBuildingMode = !inBuildingMode;
        buildingUI.SetActive(inBuildingMode);
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
        return new(position.x, _objectToPlace.transform.position.y, position.z);
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

        OpenUI(true);
    }

    public void OpenUI(bool destroy)
    {
        //UI
        buildingPanel.SetActive(true);

        //Removing listeners
        buildingPanel.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        buildingPanel.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();

        //Adding new listeners
        buildingPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => PlaceButton());

        if (destroy) 
            buildingPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => 
            { 
                Destroy(_objectToPlace.gameObject); 
                buildingPanel.SetActive(false); 
                inBuildingMode = false;
            });
        else
        {
            Vector3 oldPosition = _objectToPlace.transform.position;
            buildingPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => 
            {
                _objectToPlace.transform.position = oldPosition; PlaceButton();
            });
        }
    }

    public void PlaceButton()
    {
        if (CanBePlaced()) 
            _objectToPlace.Place();
        else
            Destroy(_objectToPlace.gameObject);

        buildingPanel.SetActive(false);
        inBuildingMode = false;
    }

    private bool CanBePlaced()
    {
        if (_objectToPlace == null) 
            return false;

        return _objectToPlace.transform.GetComponent<TriggerController>().isTriggered;
    }
}