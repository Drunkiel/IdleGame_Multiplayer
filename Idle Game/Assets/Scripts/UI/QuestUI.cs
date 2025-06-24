using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public GameObject questPrefab;
    public GameObject requirementPrefab;
    public Transform parent;

    public void AddQuestToUI(Quest _quest)
    {
        GameObject newQuest = Instantiate(questPrefab, parent);
        newQuest.transform.name = _quest.id.ToString();
        for (int i = 0; i < _quest._requirements.Count; i++)
        {
            GameObject newRequirement = Instantiate(requirementPrefab, newQuest.transform.GetChild(1));
            TMP_Text descriptionText = newRequirement.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text progressText = newRequirement.transform.GetChild(2).GetComponent<TMP_Text>();

            descriptionText.text = _quest._requirements[i].description;
            progressText.text = $"{_quest._requirements[i].progressCurrent} / {_quest._requirements[i].progressNeeded}";
        }

        //Set title
        TMP_Text titleText = newQuest.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        titleText.text = _quest.title;

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

        Destroy(parent.GetChild(index).gameObject);
    }

    public void UpdateQuestUI(int questIndex, int requirementIndex, Quest _quest)
    {
        int index = GetQuestIndex(questIndex);
        if (index == -1)
        {
            //ConsoleController.instance.ChatMessage(SenderType.System, $"There is duplicate quest index: {_quest.id} but needed is {questIndex}");
            return;
        }

        parent.GetChild(index).GetChild(1).GetChild(requirementIndex).GetChild(2).GetComponent<TMP_Text>().text = $"{_quest._requirements[requirementIndex].progressCurrent} / {_quest._requirements[requirementIndex].progressNeeded}";
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