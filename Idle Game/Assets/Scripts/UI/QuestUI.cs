using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public List<QuestCard> _questCards;
    public GameObject questCardPrefab;
    public Transform parent;

    public void AddQuestToUI(Quest _quest)
    {
        //Set name for object
        GameObject newQuest = Instantiate(questCardPrefab, parent);
        newQuest.transform.name = _quest.id.ToString();

        //Assign quest card data
        QuestCard _questCard = newQuest.GetComponent<QuestCard>();
        _questCard.titleText.text = _quest.title;
        TimeSpan timeLeft = (_quest.startDate + new TimeSpan(_quest.durationTime.x * 24 + _quest.durationTime.y, _quest.durationTime.z, 0)) - DateTime.Now;
        _questCard.durationTimeText.text = $"{timeLeft.Days}d:{timeLeft.Hours}h:{timeLeft.Minutes}m";
        _questCard.expText.text = $"{_quest.expReward} EXP";
        _questCard.goldText.text = $"{_quest.goldReward} Gold";
        _questCard.completeButton.onClick.AddListener(() =>
        {
            QuestController.instance.FinishQuest(_quest.id);
        });
        _questCards.Add(_questCard);
        UpdateQuestUI(_quest.id, _quest);

        //Reset UI to load changes
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }

    public void RemoveQuestUI(int questIndex)
    {
        int index = GetQuestIndex(questIndex);
        if (index == -1)
        {
            //ConsoleController.instance.ChatMessage(SenderType.System, $"Can't remove quest from UI: {questIndex}");
            return;
        }

        Destroy(_questCards[index].gameObject);
    }

    public void UpdateQuestUI(int questIndex, Quest _quest)
    {
        int index = GetQuestIndex(questIndex);
        if (index == -1)
        {
            //ConsoleController.instance.ChatMessage(SenderType.System, $"There is duplicate quest index: {_quest.id} but needed is {questIndex}");
            return;
        }

        _questCards[index].requirementText.text = $"{_quest._requirement.description} | {_quest._requirement.progressCurrent}/{_quest._requirement.progressNeeded}";
    }

    public int GetQuestIndex(int questIndex)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (int.TryParse(parent.GetChild(i).name, out int index) && questIndex == index)
                return i;
        }

        return -1;
    }
}