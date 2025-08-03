using UnityEngine;

public class SelectorClick : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayers;

    [SerializeField] private Collider2D col;

    public void OnClick()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f, interactableLayers);

        foreach (var hit in hits)
        {
            //Should never happen but who nows
            if (hit.gameObject.Equals(gameObject))
                continue;

            if (!hit.gameObject.TryGetComponent(out MineableObject _mineableObject))
                continue;

            //Get item and check if null
            ItemID _toolItem = PlayerController.instance._holdingController._itemController._gearHolder.GetTool(_mineableObject.toolType);
            if (_toolItem == null)
                continue;

            _mineableObject.TakeHit(_toolItem);
        }
    }
}
