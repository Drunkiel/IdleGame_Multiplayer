using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAPI : MonoBehaviour
{
    public PlayerStatus currentStatus;

    public IEnumerator UpdateStatus(PlayerStatus newStatus)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_status/" + ServerConnector.instance.playerId;
        string jsonData = "{\"status\":\"" + newStatus.ToString() + "\"}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        currentStatus = newStatus;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Status update failed: " + request.error);
    }

    public IEnumerator UpdateStats(EntityInfo _entityInfo)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_stats/" + ServerConnector.instance.playerId;

        PlayerStatsPayload payload = new()
        {
            heroClass = _entityInfo.heroClass,
            currentLevel = _entityInfo.currentLevel,
            expPoints = _entityInfo.expPoints,
            goldCoins = _entityInfo.goldCoins,
            strengthPoints = _entityInfo._baseAttributes.strengthPoints,
            dexterityPoints = _entityInfo._baseAttributes.dexterityPoints,
            intelligencePoints = _entityInfo._baseAttributes.intelligencePoints,
            durablityPoints = _entityInfo._baseAttributes.durablityPoints,
            luckPoints = _entityInfo._baseAttributes.luckPoints
        };

        string jsonData = JsonUtility.ToJson(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Stats update failed: " + request.error);
    }

    public IEnumerator UpdatePositionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (string.IsNullOrEmpty(ServerConnector.instance.playerId))
                continue;

            Vector3 pos = new(transform.position.x, transform.position.y, 0);
            string jsonData = JsonUtility.ToJson(new PositionData(pos));

            UnityWebRequest request = UnityWebRequest.Put(
                ServerConnector.instance.GetServerUrl() + "/update_position/" + ServerConnector.instance.playerId,
                jsonData
            );
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    public IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            string url = ServerConnector.instance.GetServerUrl() + "/heartbeat/" + ServerConnector.instance.playerId;
            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            yield return request.SendWebRequest();
        }
    }

    public IEnumerator UpdatePositionOnce()
    {
        Vector3 pos = new(transform.position.x, transform.position.y, 0);
        PositionData data = new(pos);

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = UnityWebRequest.Put(ServerConnector.instance.GetServerUrl() + "/update_position/" + ServerConnector.instance.playerId, json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }
}
