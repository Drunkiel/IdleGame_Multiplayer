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
    Idle,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string playerId;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private Rigidbody2D rgBody;
    private PlayerStatus currentStatus;

    public void Initialize(string id)
    {
        playerId = id;
        idText.text = playerId;
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

        //Example of how to update player status
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameController.instance.ChangeScene("Test");
            //currentStatus = PlayerStatus.Idle;
            //StartCoroutine(UpdateStatus(currentStatus));
        }

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    GameController.instance.ChangeScene();
        //    currentStatus = PlayerStatus.Idle;
        //    //StartCoroutine(UpdateStatus(currentStatus));
        //}
    }

    IEnumerator UpdateStatus(PlayerStatus newStatus)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_scene/" + playerId;
        string jsonData = "{\"scene\":\"" + newStatus.ToString() + "\"}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");

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

            if (string.IsNullOrEmpty(playerId)) continue;

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
}
