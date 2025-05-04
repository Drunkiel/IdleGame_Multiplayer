using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PositionEntry
{
    public string player_id;
    public PositionData position;
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
    public PlayerStatus status;
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
    public Transform spawnPoint;

    private Dictionary<string, PlayerGhost> ghosts = new();

    void Start()
    {
        ServerConnector.instance.OnConnected += OnConnected;
        StartCoroutine(PollOtherPlayers());
        StartCoroutine(PollOtherPositions());
    }

    void OnConnected(string myId)
    {
        //Create local player
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.Initialize(myId);
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
                    if (player.player_id == ServerConnector.instance.playerId)
                        continue;

                    //If ghost exists update status
                    if (ghosts.ContainsKey(player.player_id))
                        ghosts[player.player_id].SetStatus(player.status);
                    else
                    {
                        // Nowy ghost
                        GameObject ghost = Instantiate(ghostPrefab);
                        var ghostScript = ghost.GetComponent<PlayerGhost>();
                        ghostScript.playerId = player.player_id;
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
                    if (entry.player_id == ServerConnector.instance.playerId)
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
                        ghostScript.SetStatus(PlayerStatus.Connected);
                        ghostScript.SetPosition(new(entry.position.x, entry.position.y, entry.position.z));
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
                    Destroy(ghosts[id].gameObject);
                    ghosts.Remove(id);
                }
            }
        }
    }
}
