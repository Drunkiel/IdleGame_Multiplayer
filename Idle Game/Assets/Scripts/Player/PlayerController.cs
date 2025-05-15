using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PositionData
{
    public float x, y, z;

    public PositionData(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
}

public enum PlayerStatus
{
    Connected,
    Disconnected,
    Afk,
    Idle,
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public bool isStopped;

    public string playerId;
    public EntityInfo _entityInfo;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Rigidbody2D rgBody;
    [SerializeField] private PlayerStatus currentStatus;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize(ConnectResponse _response)
    {
        playerId = _response.player_id;
        currentStatus = (PlayerStatus)Enum.Parse(typeof(PlayerStatus), _response.status);
        _entityInfo = new EntityInfo(
            _response.heroClass,
            _response.username,
            _response.currentLevel,
            _response.expPoints,
            _response.goldCoins, 
            _response.strengthPoints, 
            _response.dexterityPoints,
            _response.intelligencePoints, 
            _response.durablityPoints, 
            _response.luckPoints, 
            UIController.instance.panelObjects[0].panelObject.GetComponent<StatisticsUI>()
        );

        nameText.text = _entityInfo.username;
        StartCoroutine(UpdateStatus(PlayerStatus.Connected));
        StartCoroutine(UpdatePositionOnce());
        StartCoroutine(UpdatePositionLoop());
        StartCoroutine(HeartbeatLoop());
        GameController.instance.objectsToTeleportMust.Add(gameObject);
    }

    void Update()
    {
        if (string.IsNullOrEmpty(playerId)) 
            return;

        //Basic movement
        float move = Input.GetAxis("Horizontal");
        transform.Translate(new(5f * move * Time.deltaTime, 0, 0));
        if (move != 0)
            transform.GetChild(1).localScale = new(Mathf.Sign(move) * 1, 1, 1);

        if (Input.GetKeyDown(KeyCode.I))
        {
            UIController.instance.OpenClose(0);
        }
    }

    IEnumerator UpdateStatus(PlayerStatus newStatus)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_status/" + playerId;
        string jsonData = "{\"status\":\"" + newStatus.ToString() + "\"}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        currentStatus = newStatus;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Status update failed: " + request.error);
    }

    IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            string url = ServerConnector.instance.GetServerUrl() + "/heartbeat/" + playerId;
            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            yield return request.SendWebRequest();
        }
    }

    IEnumerator UpdatePositionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (string.IsNullOrEmpty(playerId)) 
                continue;

            Vector3 pos = transform.position;
            string jsonData = JsonUtility.ToJson(new PositionData(pos));

            UnityWebRequest request = UnityWebRequest.Put(
                ServerConnector.instance.GetServerUrl() + "/update_position/" + playerId,
                jsonData
            );
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    IEnumerator UpdatePositionOnce()
    {
        Vector3 pos = transform.position;
        PositionData data = new(pos);

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = UnityWebRequest.Put(ServerConnector.instance.GetServerUrl() + "/update_position/" + playerId, json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(UpdateStatus(PlayerStatus.Disconnected));
    }
}
