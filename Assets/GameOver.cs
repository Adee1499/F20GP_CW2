using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOver;
    public static bool IsGameOver = false;
    
    // when returning to menu sometimes it would display the game over screen when pressing play again
    void Start() {
        IsGameOver = false;  
    }
    
    void FixedUpdate() {
        if(IsGameOver) {
            gameOver.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        } 
    }

    public void RetryGame() {
        IsGameOver = false;
        gameOver.SetActive(false);
        SceneManager.LoadScene("PlayerScene");
        SceneManager.LoadScene("MainArea", LoadSceneMode.Additive);
        Time.timeScale = 1f;
    }
    
    public void GoToMenu() {
        IsGameOver = false;  
        gameOver.SetActive(false);
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}