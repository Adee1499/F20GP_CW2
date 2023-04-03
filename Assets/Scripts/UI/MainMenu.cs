using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GoToGame() {
        SceneManager.LoadScene("PlayerScene");
        SceneManager.LoadScene("MainArea", LoadSceneMode.Additive);
    }
    
    public void QuitGame() {
        Application.Quit();
    }
    
}