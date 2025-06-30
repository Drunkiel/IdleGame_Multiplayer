using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class QuestAPI : MonoBehaviour
{
    public List<QuestPayLoad> quests = new();
    public List<QuestPayLoad> finishedQuests = new();

    // Wrapper na dane z API
    [System.Serializable]
    public class QuestListResponse
    {
        [JsonProperty("activeQuests")]
        public List<QuestPayLoad> activeQuests;

        [JsonProperty("completedQuests")]
        public List<QuestPayLoad> completedQuests;
    }

    public IEnumerator GetQuestCoroutine()
    {
        yield return FetchQuestsData(
            (active, completed) => InitializeQuests(),
            err => Debug.LogError("ERROR: " + err)
        );
    }

    public void InitializeQuests()
    {
        if (quests == null || quests.Count == 0)
            return;

        QuestController _questController = QuestController.instance;

        foreach (var quest in quests)
        {
            _questController._allQuests[quest.id].startDate = quest.startDate;
            _questController._allQuests[quest.id]._requirement.progressCurrent = quest.requirementProgressCurrent;
            _questController.GiveQuest(quest.id, false);

            // Check if quest is finished
            if (_questController._allQuests[quest.id].CheckIfFinished())
            {
                int index = _questController._questUI.GetQuestIndex(quest.id);
                if (index >= 0 && index < _questController._questUI._questCards.Count)
                    _questController._questUI._questCards[index].completeButton.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator FetchQuestsData(System.Action<List<QuestPayLoad>, List<QuestPayLoad>> onComplete, System.Action<string> onError = null)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/quests/" + ServerConnector.instance.playerId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Get quests failed: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;

            try
            {
                QuestListResponse wrapper = JsonConvert.DeserializeObject<QuestListResponse>(json);
                quests = wrapper.activeQuests ?? new List<QuestPayLoad>();
                finishedQuests = wrapper.completedQuests ?? new List<QuestPayLoad>();

                onComplete?.Invoke(quests, finishedQuests);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Deserialization failed: " + ex.Message);
                onError?.Invoke("Failed to parse quests.");
            }
        }
    }

    public IEnumerator UpdateAllQuests()
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_quests/" + ServerConnector.instance.playerId;

        var requestData = new
        {
            activeQuests = quests,
            completedQuests = finishedQuests
        };

        string json = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Quest update failed: " + request.error);
        else
            Debug.Log("Active quests successfully updated.");
    }
}
