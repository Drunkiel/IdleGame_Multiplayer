using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCard : MonoBehaviour
{
    public Image image;
    public TMP_Text nameText;
    public Button buildBTN;

    public void AssignData(Sprite sprite, string buildingName, Action action)
    {
        image.sprite = sprite;
        nameText.text = buildingName;
        buildBTN.onClick.AddListener(() => action.Invoke());
    }
}