﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerAPI _playerAPI;
    public HoldingController _holdingController;

    private bool isFlipped;
    public float speedForce;
    private Vector2 movement;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize(ConnectResponse _response)
    {
        playerId = _response.player_id;
        _playerAPI.currentStatus = (PlayerStatus)Enum.Parse(typeof(PlayerStatus), _response.status);
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
        InventoryController.instance.LoadInventory();
        QuestController.instance.LoadQuests();

        nameText.text = _entityInfo.username;
        StartCoroutine(_playerAPI.UpdateStatus(PlayerStatus.Connected));
        StartCoroutine(_playerAPI.UpdatePositionOnce());
        StartCoroutine(_playerAPI.UpdatePositionLoop());
        StartCoroutine(_playerAPI.HeartbeatLoop());
        GameController.instance.objectsToTeleportMust.Add(gameObject);
    }

    void Update()
    {
        anim.SetFloat("Vertical", isStopped ? 0 : movement.y);
        anim.SetFloat("Horizontal", isStopped ? 0 : movement.x);

        if (movement != Vector2.zero)
        {
            anim.SetFloat("LastVertical", isStopped ? 0 : movement.y);
            anim.SetFloat("LastHorizontal", isStopped ? 0 : movement.x);
        }

        if (string.IsNullOrEmpty(playerId) || isStopped || GameController.isPaused)
            return;

        Vector3 moveDelta = speedForce * Time.deltaTime * (Vector3)movement;
        transform.Translate(moveDelta);
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (isStopped)
        {
            movement = new();
            return;
        }

        //Flipping player to direction they are going
        if (inputValue.x < 0 && !isFlipped)
        {
            isFlipped = true;
            _holdingController._itemController._gearHolder.FlipItems(true);

        }
        else if (inputValue.x > 0 && isFlipped)
        {
            isFlipped = false;
            _holdingController._itemController._gearHolder.FlipItems(false);
        }

        //Flipping player to direction they are going
        //transform.GetChild(1).localScale = new(isFlipped ? -1 : 1 * 1, 1, 1);

        movement = new Vector2(inputValue.x, inputValue.y);
    }

    public void UpdateStats()
    {
        StartCoroutine(_playerAPI.UpdateStats(_entityInfo));
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(_playerAPI.UpdateStatus(PlayerStatus.Disconnected));
    }
}
