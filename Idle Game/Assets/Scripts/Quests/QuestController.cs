using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestPayLoad
{
    public int id;
    public DateTime startDate;
    public int requirementProgressCurrent;
}

[Serializable]
public class UpdateQuestsRequest
{
    public List<QuestPayLoad> activeQuests;
    public List<string> completedQuests;
}

public class QuestController : MonoBehaviour
{
    public static QuestController instance;
    public QuestUI _questUI;

    public List<Quest> _allQuests;
    public List<int> currentQuestsIndex;
    public List<int> completedQuests;

    public short idToCheck;
    public List<short> killQuestIndexes;
    public List<short> collectQuestIndexes;
    public List<short> talkQuestIndexes;

    public QuestAPI _questAPI;

    private void Awake()
    {
        instance = this;
    }

    public void LoadQuests()
    {
        StartCoroutine(_questAPI.GetQuestCoroutine());
    }

    public void GiveQuest(int questIndex, bool load = true)
    {
        //Check if index is bigger than are quests
        if (questIndex >= _allQuests.Count)
            return;

        //Check if quest is activated
        if (currentQuestsIndex.Contains(questIndex))
            return;

        //Add quest
        currentQuestsIndex.Add(questIndex);
        _allQuests[questIndex].startDate = DateTime.Now;
        _questUI.AddQuestToUI(_allQuests[questIndex]);

        //if (PopUpController.instance != null)
        //    PopUpController.instance.CreatePopUp(PopUpInfo.QuestAccepted, _allQuests[questIndex].title);

        //Set listeners
        switch (_allQuests[questIndex]._requirement.type)
        {
            case RequirementType.Kill:
                killQuestIndexes.Add((short)questIndex);
                break;

            case RequirementType.Collect:
                collectQuestIndexes.Add((short)questIndex);
                break;

            case RequirementType.Talk:
                talkQuestIndexes.Add((short)questIndex);
                break;
        }

        if (load)
            StartCoroutine(_questAPI.UpdateAllQuests());
    }

    public void FinishQuest(int questIndex)
    {
        int correctIndex = _questUI.GetQuestIndex(questIndex);

        //Remove quest from the API
        _questAPI.finishedQuests.Add(_questAPI.quests[correctIndex]);
        _questAPI.quests.RemoveAt(correctIndex);
        StartCoroutine(_questAPI.UpdateAllQuests());

        //Removing quest
        _questUI.RemoveQuestUI(questIndex);
        currentQuestsIndex.Remove(questIndex);
        completedQuests.Add(questIndex);
        _allQuests[questIndex].onFinishEvent.Invoke();
        //PopUpController.instance.CreatePopUp(PopUpInfo.QuestCompleted, _allQuests[questIndex].title);

        //Reset progress
        _allQuests[questIndex]._requirement.progressCurrent = 0;

        //Remove from listeners
        switch (_allQuests[questIndex]._requirement.type)
        {
            case RequirementType.Kill:
                killQuestIndexes.Remove((short)questIndex);
                break;

            case RequirementType.Collect:
                collectQuestIndexes.Remove((short)questIndex);
                break;

            case RequirementType.Talk:
                talkQuestIndexes.Remove((short)questIndex);
                break;
        }
    }

    public void InvokeKillEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < killQuestIndexes.Count; i++)
        {
            //Simple check
            if (killQuestIndexes[i] < 0 || killQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[killQuestIndexes[i]]._requirement;
            if (requirements.type != RequirementType.Kill)
                continue;

            EventListener(killQuestIndexes[i]);
        }
    }

    public void InvokeCollectEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < collectQuestIndexes.Count; i++)
        {
            //Simple check
            if (collectQuestIndexes[i] < 0 || collectQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[collectQuestIndexes[i]]._requirement;
            if (requirements.type != RequirementType.Collect)
                continue;

            EventListener(collectQuestIndexes[i]);
        }
    }

    public void InvokeTalkEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < talkQuestIndexes.Count; i++)
        {
            //Simple check
            if (talkQuestIndexes[i] < 0 || talkQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[talkQuestIndexes[i]]._requirement;
            if (requirements.type != RequirementType.Talk)
                continue;

            EventListener(talkQuestIndexes[i]);
        }
    }

    private void EventListener(int questIndex)
    {
        //Update UI
        _allQuests[questIndex]._requirement.UpdateStatus(idToCheck);
        _questUI.UpdateQuestUI(questIndex, _allQuests[questIndex]);

        //Check if quest is finished
        if (_allQuests[questIndex].CheckIfFinished())
            _questUI._questCards[_questUI.GetQuestIndex(questIndex)].completeButton.gameObject.SetActive(true);
    }
}