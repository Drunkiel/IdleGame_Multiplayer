using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum Attributes
{
    Strength,
    Dexterity,
    Intelligence,
    Durability,
    Luck
}

public enum HeroClass
{
    Mage,
    Warrior,
    Scout
}

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static bool isPC = true;
    public static bool isPaused;

    public List<GameObject> objectsToTeleportMust = new();
    public List<GameObject> objectsToTeleportAdditional = new();

    private void Awake()
    {
        instance = this;
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    private IEnumerator LoadAsyncScene(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name.Equals(sceneName))
        {
            StartCoroutine(UpdateScene(currentScene.name));
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //Wait until scene is loaded
        while (!asyncLoad.isDone)
            yield return null;

        Scene nextScene = SceneManager.GetSceneByName(sceneName);
        StartCoroutine(UpdateScene(nextScene.name));

        //Move objects to other scene
        for (int i = 0; i < objectsToTeleportMust.Count; i++)
            SceneManager.MoveGameObjectToScene(objectsToTeleportMust[i], nextScene);

        SceneManager.UnloadSceneAsync(currentScene);
    }

    private IEnumerator UpdateScene(string sceneName)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_scene/" + ServerConnector.instance.playerId;
        string jsonData = "{\"scene\":\"" + sceneName + "\"}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Status update failed: " + request.error);
    }
}
