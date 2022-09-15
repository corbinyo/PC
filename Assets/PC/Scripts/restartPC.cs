using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
public class restartPC : MonoBehaviour
{
    private void Awake()
    {
        // Register callback for everytime a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initially load the second scene additive
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    // Called when a new scene was loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (loadMode == LoadSceneMode.Additive)
        {
            // Set the additive loaded scene as active
            // So new instantiated stuff goes here
            // And so we know which scene to unload later
            SceneManager.SetActiveScene(scene);
        }
    }


    public static void ReloadScene()
    {
        var currentScene = SceneManager.GetActiveScene();
        var index = currentScene.buildIndex;

        SceneManager.UnloadScene(currentScene);

        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }

    public static void LoadScene(string name)
    {
        var currentScene = SceneManager.GetActiveScene();

        SceneManager.UnloadScene(currentScene);

        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }

    public static void LoadScene(int index)
    {
        var currentScene = SceneManager.GetActiveScene();

        SceneManager.UnloadScene(currentScene);

        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }
}