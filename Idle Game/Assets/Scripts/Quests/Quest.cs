using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class Quest
{
    public float id;
    public string title;
    public string description;
    public List<Requirement> _requirements;
    public UnityEvent onFinishEvent;

    public bool CheckIfFinished()
    {
        for (int i = 0; i < _requirements.Count; i++)
        {
            if (_requirements[i].progressCurrent < _requirements[i].progressNeeded)
                return false;
        }

        return true;
    }
}

[Serializable]
public class Requirement
{
    public RequirementType type;
    public string description;
    public short targetID;
    public int progressCurrent;
    public int progressNeeded;

    public void UpdateStatus(short id)
    {
        if (id != targetID)
            return;

        progressCurrent += 1;
    }
}

public enum RequirementType
{
    Kill,
    Collect,
    Talk,
}