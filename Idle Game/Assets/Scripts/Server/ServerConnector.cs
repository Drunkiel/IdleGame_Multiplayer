using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class ConnectResponse
{
    public string player_id;
    public string status;
}

public class ServerConnector : MonoBehaviour
{
    public static ServerConnector instance;
    [HideInInspector] public string playerId;
    [SerializeField] private string serverUrl = "http://localhost:3000";

    public Action<string> OnConnected;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(ConnectToServer());
    }

    IEnumerator ConnectToServer()
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(serverUrl + "/connect", "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = JsonUtility.FromJson<ConnectResponse>(request.downloadHandler.text);
            playerId = json.player_id;
            Debug.Log("Connected to server with ID: " + playerId);
            OnConnected?.Invoke(playerId);
        }
        else
            Debug.LogError("Failed to connect to server: " + request.error);
    }

    public string GetServerUrl() => serverUrl;
}
