using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public static MouseMovement instance;

    private Vector2 offset = new(0, 0.5f);

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetGridPos = Vector2Int.FloorToInt(mousePos + new Vector2(0.5f, 0));

        Vector2 positionWithOffset = transform.position - (Vector3)offset;
        Vector2Int currentGridPos = Vector2Int.FloorToInt(positionWithOffset + new Vector2(0.5f, 0.5f));

        int moveX = Mathf.Clamp(targetGridPos.x - currentGridPos.x, -1, 1);
        int moveY = Mathf.Clamp(targetGridPos.y - currentGridPos.y, -1, 1);

        Vector2Int nextGridPos = currentGridPos + new Vector2Int(moveX, moveY);
        transform.position = new Vector3(nextGridPos.x, nextGridPos.y, transform.position.z) + (Vector3)offset;
    }

    public Vector3 GetPosition()
    {
        return transform.position;        
    }
}
