using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Panel
{
    public GameObject panelObject;
    public bool isOn;
}

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public List<Panel> panelObjects = new();

    private void Awake()
    {
        instance = this;
    }

    public void OpenClose(int index)
    {
        if (index >= panelObjects.Count || index < 0)
            return;

        panelObjects[index].isOn = !panelObjects[index].isOn;
        panelObjects[index].panelObject.SetActive(panelObjects[index].isOn);
    }

    public void Open(int index)
    {
        if (index >= panelObjects.Count || index < 0)
            return;

        panelObjects[index].isOn = true;
        panelObjects[index].panelObject.SetActive(true);
    }

    public void Close(int index)
    {
        if (index >= panelObjects.Count || index < 0)
            return;

        panelObjects[index].isOn = false;
        panelObjects[index].panelObject.SetActive(false);
    }
}
