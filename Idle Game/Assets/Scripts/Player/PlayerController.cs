using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

[Serializable]
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

[Serializable]
public class PlayerStatsPayload
{
    public HeroClass heroClass;
    public int currentLevel;
    public int expPoints;
    public int goldCoins;
    public int strengthPoints;
    public int dexterityPoints;
    public int intelligencePoints;
    public int durablityPoints;
    public int luckPoints;
    public int armorPoints;
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
    public HoldingController _holdingController;

    private bool isFlipped;
    public float speedForce;
    private Vector2 movement;
    private Vector2 newVelocityXZ;
    private float newVelocityY;

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
        //anim.SetFloat("Movement", isStopped ? 0 : movement.magnitude);

        if (string.IsNullOrEmpty(playerId) || isStopped || GameController.isPaused)
            return;

        //Clamping movement speed
        newVelocityXZ = new(rgBody.velocity.x, 0);
        newVelocityY = rgBody.velocity.y;

        if (newVelocityXZ.magnitude > 1f)
            newVelocityXZ = Vector3.ClampMagnitude(newVelocityXZ, 1f);

        if (newVelocityY < -10)
            newVelocityY = -10;

        rgBody.velocity = new(newVelocityXZ.x, newVelocityY);
    }

    private void FixedUpdate()
    {
        if (string.IsNullOrEmpty(playerId) || isStopped || GameController.isPaused)
            return;

        //Make movement depend on direction player is facing
        Vector2 move = new Vector2(movement.x, 0).normalized;

        //Move player
        rgBody.AddForce(move * speedForce);
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        //Flipping player to direction they are going
        if (inputValue.x < 0 && !isFlipped)
            isFlipped = true;
        else if (inputValue.x > 0 && isFlipped)
            isFlipped = false;

        //Flipping player to direction they are going
        transform.GetChild(1).localScale = new(isFlipped ? -1 : 1 * 1, 1, 1);

        movement = new Vector2(inputValue.x, inputValue.y);
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

    public void UpdateStats()
    {
        StartCoroutine(UpdateStats(_entityInfo));
    }

    private IEnumerator UpdateStats(EntityInfo _entityInfo)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_stats/" + playerId;

        PlayerStatsPayload payload = new()
        {
            heroClass = _entityInfo.heroClass,
            currentLevel = _entityInfo.currentLevel,
            expPoints = _entityInfo.expPoints,
            goldCoins = _entityInfo.goldCoins,
            strengthPoints = _entityInfo.strengthPoints,
            dexterityPoints = _entityInfo.dexterityPoints,
            intelligencePoints = _entityInfo.intelligencePoints,
            durablityPoints = _entityInfo.durablityPoints,
            luckPoints = _entityInfo.luckPoints,
            armorPoints = _entityInfo.armorPoints
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
