using UnityEngine;


public class BuildingID : MonoBehaviour
{
    public short buildingID;
    public string buildingName;

    public Sprite GetSprite()
    {
        return transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }
}