using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Quest
{
    public int id;
    public string title;
    public string description;
    public DateTime startDate;
    public Vector3Int durationTime;
    public int expReward;
    public int goldReward;
    public Requirement _requirement;
    public UnityEvent onFinishEvent;

    public bool CheckIfFinished()
    {
        if (_requirement.progressCurrent < _requirement.progressNeeded)
            return false;

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