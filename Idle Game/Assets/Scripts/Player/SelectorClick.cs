using UnityEngine;
using UnityEngine.InputSystem;

public class SelectorClick : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayers;

    [SerializeField] private Collider2D col;

    public void OnClick(InputAction.CallbackContext context)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f, interactableLayers);

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                Debug.Log("Zderzenie z: " + hit.gameObject.name);
            }
        }
    }
}
