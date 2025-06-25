using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuestAPI : MonoBehaviour
{
    // Pobieranie listy questów gracza z endpointu /quests/:player_id
    public IEnumerator GetQuests(System.Action<List<QuestPayLoad>> onComplete, System.Action<string> onError = null)
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

            // Za³ó¿my, ¿e backend zwraca tablicê questów, wiêc potrzebujemy opakowaæ j¹ w coœ co JsonUtility potrafi zdeserializowaæ.
            // Unity JsonUtility nie deserializuje tablicy jako root, wiêc zróbmy wrapper:
            QuestsWrapper wrapper = JsonUtility.FromJson<QuestsWrapper>("{\"quests\":" + json + "}");

            onComplete?.Invoke(wrapper.quests);
        }
    }

    // Wrapper do poprawnej deserializacji tablicy
    [System.Serializable]
    private class QuestsWrapper
    {
        public List<QuestPayLoad> quests;
    }

    // Aktualizacja jednego questa na endpoint /update_quest/:player_id/:quest_id
    public IEnumerator UpdateSingleQuest(QuestPayLoad quest, System.Action onComplete = null, System.Action<string> onError = null)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_quest/" + ServerConnector.instance.playerId + "/" + quest.id;

        string json = JsonUtility.ToJson(quest);
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

    // Twoja ju¿ istniej¹ca funkcja aktualizacji wielu questów (bez zmian, ale tutaj dla kompletnoœci)
    public IEnumerator UpdateActiveQuests()
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_quests/" + ServerConnector.instance.playerId;

        List<QuestPayLoad> payloadList = new();

        foreach (int questIndex in QuestController.instance._currentQuestsIndex)
        {
            Quest quest = QuestController.instance._allQuests[questIndex];
            QuestPayLoad payload = new()
            {
                id = quest.id,
                startDate = quest.startDate,
                requirementProgressCurrent = quest._requirement.progressCurrent
            };
            payloadList.Add(payload);
        }

        UpdateQuestsRequest requestData = new()
        {
            activeQuests = payloadList,
            completedQuests = new List<string>()
        };

        string json = JsonUtility.ToJson(requestData);

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
