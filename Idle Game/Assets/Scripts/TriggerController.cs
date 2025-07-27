using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public bool isTriggered;
    public bool reverseReturn;
    public List<string> objectsTag = new();

    void OnTriggerEnter2D(Collider2D collider)
    {
        CheckCollision(collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        CheckCollision(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        CheckCollision(collider, false);
    }

    void CheckCollision(Collider2D collider, bool enter = true)
    {
        if (objectsTag.Count == 0)
            return;

        if (objectsTag.Contains(collider.tag))
        {
            isTriggered = reverseReturn ? !enter : enter;
            return;
        }
    }
}