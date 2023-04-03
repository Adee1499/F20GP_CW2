using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start() 
    {
        // Check if the main area scene is already loaded
        Scene targetScene = SceneManager.GetSceneByBuildIndex(1);
        if (!targetScene.isLoaded) {
            // Load the scene if it's not already loaded
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }
    }
}
