using TMPro;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public short entityID;
    public EntityInfo _entityInfo;

    private void Start()
    {
        transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = _entityInfo.username;
    }
}
