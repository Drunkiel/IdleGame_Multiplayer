using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorLevel
{
    public int floor;
    public Vector2 position;
}

public class ElevatorController : MonoBehaviour
{
    public List<FloorLevel> floorLevels = new();
    [SerializeField] private int currentFloorIndex = 0;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float threshold = 0.05f;
    [SerializeField] private GameObject blockCollider;

    private Vector2 targetPosition;
    private bool isMoving = false;

    public void UseElevator()
    {
        if (floorLevels.Count == 0 || isMoving) 
            return;

        // PrzejdŸ do nastêpnego poziomu z zawiniêciem
        currentFloorIndex = (currentFloorIndex + 1) % floorLevels.Count;
        targetPosition = floorLevels[currentFloorIndex].position;

        StartCoroutine(MoveElevator(PlayerController.instance.transform));
    }

    public void GoToFloor(int index)
    {
        if (isMoving || currentFloorIndex == index || index < 0 || index >= floorLevels.Count)
            return;

        currentFloorIndex = index;
        targetPosition = floorLevels[currentFloorIndex].position;

        StartCoroutine(MoveElevator(null));
    }

    private IEnumerator MoveElevator(Transform objectToAttach)
    {
        isMoving = true;
        blockCollider.SetActive(true);

        // Podczepiamy obiekt do windy
        if (objectToAttach != null)
        {
            PlayerController.instance.isStopped = true;
            objectToAttach.SetParent(transform);
        }

        // Dopóki nie dotrzemy do celu
        while (Vector3.Distance(transform.position, targetPosition) > threshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        if (objectToAttach != null)
        {
            objectToAttach.SetParent(null);
            PlayerController.instance.isStopped = false;
        }

        isMoving = false;
        blockCollider.SetActive(false);
    }
}
