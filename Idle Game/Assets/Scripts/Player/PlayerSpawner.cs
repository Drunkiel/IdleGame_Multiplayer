using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PositionEntry
{
    public string player_id;
    public string username;
    public PositionData position;
    public string scene;
}

[System.Serializable]
public class PositionListWrapper
{
    public List<PositionEntry> positions;
}

[System.Serializable]
public class PlayerData
{
    public string player_id;
    public string username;
    public PlayerStatus status;
    public string scene;
}

[System.Serializable]
public class PlayerListWrapper
{
    public List<PlayerData> players;
}

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject ghostPrefab;

    private Dictionary<string, PlayerGhost> ghosts = new();

    void Start()
    {
        ServerConnector.instance.OnConnected += OnConnected;
        StartCoroutine(PollOtherPlayers());
        StartCoroutine(PollOtherPositions());
    }

    void OnConnected(ConnectResponse _response)
    {
        //Create local player
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        PlayerController _controller = player.GetComponent<PlayerController>();
        _controller.Initialize(_response);
        GameController.instance.ChangeScene(_response.scene);
    }

    IEnumerator PollOtherPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            UnityWebRequest request = UnityWebRequest.Get(ServerConnector.instance.GetServerUrl() + "/players");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = "{\"players\":" + request.downloadHandler.text + "}";
                var list = JsonUtility.FromJson<PlayerListWrapper>(json);

                foreach (var player in list.players)
                {
                    if (player.player_id.Equals(ServerConnector.instance.playerId))
                        continue;

                    //Check if player is in the same scene
                    if (player.scene != SceneManager.GetActiveScene().name)
                    {
                        //Destroy ghosts
                        if (ghosts.ContainsKey(player.player_id))
                        {
                            if (ghosts[player.player_id] == null)
                                continue;

                            Destroy(ghosts[player.player_id].gameObject);
                            ghosts.Remove(player.player_id);
                        }
                        continue;
                    }

                    if (ghosts.ContainsKey(player.player_id))
                        ghosts[player.player_id].SetStatus(player.status);
                    else
                    {
                        GameObject ghost = Instantiate(ghostPrefab);
                        var ghostScript = ghost.GetComponent<PlayerGhost>();
                        ghostScript.playerId = player.player_id;
                        ghostScript.username = player.username;
                        ghostScript.SetStatus(player.status);
                        ghosts.Add(player.player_id, ghostScript);
                    }
                }
            }
        }
    }

    IEnumerator PollOtherPositions()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            UnityWebRequest request = UnityWebRequest.Get(ServerConnector.instance.GetServerUrl() + "/positions");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = "{\"positions\":" + request.downloadHandler.text + "}";
                var list = JsonUtility.FromJson<PositionListWrapper>(json);

                HashSet<string> updatedPlayerIds = new();

                foreach (var entry in list.positions)
                {
                    if (entry.player_id.Equals(ServerConnector.instance.playerId))
                        continue;

                    if (entry.scene != SceneManager.GetActiveScene().name)
                        continue;

                    updatedPlayerIds.Add(entry.player_id);

                    if (ghosts.ContainsKey(entry.player_id))
                    {
                        Vector3 pos = new(entry.position.x, entry.position.y, entry.position.z);
                        ghosts[entry.player_id].SetPosition(pos);
                    }
                    else
                    {
                        GameObject ghost = Instantiate(ghostPrefab);
                        var ghostScript = ghost.GetComponent<PlayerGhost>();
                        ghostScript.playerId = entry.player_id;
                        ghostScript.username = entry.username;
                        ghostScript.SetStatus(PlayerStatus.Connected);
                        ghostScript.SetPosition(new(entry.position.x, entry.position.y, entry.position.z), true);
                        ghosts.Add(entry.player_id, ghostScript);
                    }
                }

                //Destroy unactive ghosts
                List<string> toRemove = new();
                foreach (var id in ghosts.Keys)
                {
                    if (!updatedPlayerIds.Contains(id))
                        toRemove.Add(id);
                }

                foreach (string id in toRemove)
                {
                    if (ghosts[id] == null)
                        continue;

                    Destroy(ghosts[id].gameObject);
                    ghosts.Remove(id);
                }
            }
        }
    }
}
