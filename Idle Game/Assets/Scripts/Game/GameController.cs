using System;
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

[Serializable]
public class SceneConfig
{
    public Vector2 spawnPosition;
    public bool cameraFollowX;
    public bool cameraFollowY;
}

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static bool isPC = true;
    public static bool isPaused;

    public List<GameObject> objectsToTeleportMust = new();
    public List<GameObject> objectsToTeleportAdditional = new();
    public List<SceneConfig> _sceneConfigs = new();

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
            PlayerController.instance.transform.position = _sceneConfigs[currentScene.buildIndex].spawnPosition;
            CameraController.instance.Config(_sceneConfigs[currentScene.buildIndex].cameraFollowX, _sceneConfigs[currentScene.buildIndex].cameraFollowY, _sceneConfigs[currentScene.buildIndex].spawnPosition);
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

        PlayerController.instance.transform.position = _sceneConfigs[nextScene.buildIndex].spawnPosition;
        CameraController.instance.Config(_sceneConfigs[nextScene.buildIndex].cameraFollowX, _sceneConfigs[nextScene.buildIndex].cameraFollowY, _sceneConfigs[nextScene.buildIndex].spawnPosition);

        SceneManager.UnloadSceneAsync(currentScene);
    }

    private IEnumerator UpdateScene(string sceneName)
    {
        string url = ServerConnector.instance.GetServerUrl() + "/update_scene/" + PlayerController.instance.playerId;
        string jsonData = "{\"scene\":\"" + sceneName + "\"}";

        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Status update failed: " + request.error);
    }
}
