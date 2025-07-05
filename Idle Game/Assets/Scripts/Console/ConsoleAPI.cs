using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ConsoleAPI : MonoBehaviour
{
    [Serializable]
    public class ChatMessage
    {
        public string senderId;
        public string senderUsername;
        public string scene;
        public string message;
        public long timestamp;
    }

    private string lastTimestamp = null;

    public IEnumerator SendChatMessage(string playerId, string message)
    {
        string url = ServerConnector.instance.GetServerUrl() + $"/chat/{playerId}";

        var payload = new
        {
            message
        };

        string json = JsonConvert.SerializeObject(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Chat message send failed: " + request.error);
        }
    }

    public IEnumerator FetchChatMessages()
    {
        yield return new WaitForSeconds(1f);

        string url = ServerConnector.instance.GetServerUrl() + "/chat";

        if (!string.IsNullOrEmpty(lastTimestamp))
        {
            url += $"?since={UnityWebRequest.EscapeURL(lastTimestamp)}";
        }

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch chat messages: " + request.error);
        }
        else
        {
            try
            {
                string json = request.downloadHandler.text;
                List<ChatMessage> messages = JsonConvert.DeserializeObject<List<ChatMessage>>(json);

                foreach (var msg in messages)
                {
                    if (msg.senderId == ServerConnector.instance.playerId)
                        continue;

                    ConsoleController.instance.ChatMessage(msg.senderUsername, msg.message, OutputType.Normal);
                    Debug.Log($"[{msg.senderUsername}] {msg.message}");

                    // Zaktualizuj znacznik czasu ostatniej wiadomoœci
                    if (msg.timestamp > 0)
                    {
                        DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(msg.timestamp).UtcDateTime;
                        lastTimestamp = dt.ToString("o"); // format ISO 8601
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
            }
        }

        //Repeat procces
        StartCoroutine(FetchChatMessages());
    }
}
