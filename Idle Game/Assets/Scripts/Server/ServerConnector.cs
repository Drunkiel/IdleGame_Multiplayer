using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[Serializable]
public class ConnectResponse
{
    public string player_id;
    public string status;
}

[Serializable]
public class RegisterRequest
{
    public string username;
    public string password;
    public int heroClass;
}

[Serializable]
public class RegisterResponse
{
    public string message;
}

public class ServerConnector : MonoBehaviour
{
    public static ServerConnector instance;
    [HideInInspector] public string playerId;
    [HideInInspector] public string playerUsername;
    [SerializeField] private string serverUrl = "http://localhost:3000";
    public LoginUI _loginUI;

    public Action<string> OnConnected;

    void Awake()
    {
        instance = this;
    }

    public void JoinServer()
    {
        if (!_loginUI.CanLogIn())
            return;

        StartCoroutine(ConnectToServer());
    }

    public void RegisterPlayer()
    {
        StartCoroutine(RegisterUser());
    }

    private IEnumerator ConnectToServer()
    {
        LoginRequest loginRequest = new()
        {
            username = _loginUI.loginUsernameInput.text,
            password = _loginUI.loginPasswordInput.text
        };

        string jsonData = JsonUtility.ToJson(loginRequest);

        using UnityWebRequest request = new(serverUrl + "/login", "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ConnectResponse response = JsonUtility.FromJson<ConnectResponse>(request.downloadHandler.text);
            playerId = response.player_id;
            playerUsername = loginRequest.username;
            Debug.Log("Connected to server with ID: " + playerId);
            OnConnected?.Invoke(playerId);
        }
        else
        {
            _loginUI.OpenRegPage();
            Debug.LogError("Failed to connect to server: " + request.error);
            Debug.LogWarning("Server response: " + request.downloadHandler.text);
        }
    }

    private IEnumerator RegisterUser()
    {
        RegisterRequest registerData = new()
        {
            username = _loginUI.registerUsernameInput.text,
            password = _loginUI.registerPasswordInput.text,
            heroClass = _loginUI.dropdownHeroClass.value // lub inna zmienna reprezentuj¹ca wybran¹ klasê
        };

        string jsonData = JsonUtility.ToJson(registerData);

        using UnityWebRequest request = new(serverUrl + "/register", "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            _loginUI.OpenLogPage();
            RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
            Debug.Log("Registration complete: " + response.message);
        }
        else
        {
            Debug.LogError("Registration failed: " + request.error);
            Debug.LogWarning("Response: " + request.downloadHandler.text);
        }
    }

    public string GetServerUrl() => serverUrl;
}
