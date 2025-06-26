using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class QuestAPI : MonoBehaviour
{
    public QuestPayLoad[] quests;
    public QuestPayLoad[] completedQuests;

    // Wrapper na dane z API
    [System.Serializable]
    public class QuestListResponse
    {
        [JsonProperty("activeQuests")]
        public QuestPayLoad[] activeQuests;

        [JsonProperty("completedQuests")]
        public QuestPayLoad[] completedQuests;
    }

    public IEnumerator GetQuestCoroutine()
    {
        yield return FetchQuestsData(
            (active, completed) => 
            {
                InitializeQuests();
            },
            err => Debug.LogError("ERROR: " + err)
        );
    }

    public void InitializeQuests()
    {
        if (quests == null) 
            return;

        for (int i = 0; i < quests.Length; i++)
        {
            QuestController.instance._allQuests[quests[i].id].startDate = quests[i].startDate;
            QuestController.instance._allQuests[quests[i].id]._requirement.progressCurrent = quests[i].requirementProgressCurrent;
            QuestController.instance.GiveQuest(quests[i].id, false);
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
                quests = wrapper.activeQuests;
                completedQuests = wrapper.completedQuests;

                onComplete?.Invoke(new List<QuestPayLoad>(quests), new List<QuestPayLoad>(completedQuests));
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Deserialization failed: " + ex.Message);
                onError?.Invoke("Failed to parse quests.");
            }
        }
    }

    public IEnumerator UpdateSingleQuest(QuestPayLoad quest, System.Action onComplete = null, System.Action<string> onError = null)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_quest/" + ServerConnector.instance.playerId + "/" + quest.id;

        string json = JsonConvert.SerializeObject(quest);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Update single quest failed: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            Debug.Log("Single quest successfully updated: " + quest.id);
            onComplete?.Invoke();
        }
    }

    public IEnumerator UpdateActiveQuests()
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_quests/" + ServerConnector.instance.playerId;

        List<QuestPayLoad> activePayload = new();
        List<string> completed = new(); // Dodaj jeœli chcesz wysy³aæ completedQuests

        foreach (int questIndex in QuestController.instance._currentQuestsIndex)
        {
            Quest quest = QuestController.instance._allQuests[questIndex];
            QuestPayLoad payload = new()
            {
                id = quest.id,
                startDate = quest.startDate,
                requirementProgressCurrent = quest._requirement.progressCurrent
            };
            activePayload.Add(payload);
        }

        var requestData = new
        {
            activeQuests = activePayload,
            completedQuests = completed
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
