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
    public HeroClass heroClass;
    public string username;
    public int currentLevel;
    public int expPoints;
    public int goldCoins;
    public int strengthPoints;
    public int dexterityPoints;
    public int intelligencePoints;
    public int durablityPoints;
    public int luckPoints;
}

[Serializable]
public class RegisterRequest
{
    public string username;
    public string password;
    public string heroClass;
}

[Serializable]
public class RegisterResponse
{
    public string message;
}

public class ServerConnector : MonoBehaviour
{
    public static ServerConnector instance;
    public string playerId;
    [SerializeField] private string serverUrl = "http://localhost:3000";
    public LoginUI _loginUI;

    public Action<ConnectResponse> OnConnected;

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
            ConnectResponse _response = JsonUtility.FromJson<ConnectResponse>(request.downloadHandler.text);
            playerId = _response.player_id;
            Debug.Log("Connected to server with ID: " + _response.player_id);
            OnConnected?.Invoke(_response);
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
            heroClass = _loginUI.dropdownHeroClass.value.ToString(),
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
