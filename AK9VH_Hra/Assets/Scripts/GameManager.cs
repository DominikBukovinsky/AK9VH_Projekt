using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject loadingPanel; 
    public GameObject player; 
    
    
    public GameObject stopkyObjekt; 

    private bool isGameActive = false;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 0; 
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        
        
        if(loadingPanel != null) loadingPanel.SetActive(false);
        if(stopkyObjekt != null) stopkyObjekt.SetActive(false);
        
        LoadPosition(); 
    }

    void Update()
    {
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }


    public void StartGame()
    {
        // Misto okamziteho startu, hodi loading screen
        StartCoroutine(LoadingSequence());
    }

    IEnumerator LoadingSequence()
    {
        mainMenuPanel.SetActive(false);

        if(loadingPanel != null) loadingPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(4f);

        if(loadingPanel != null) loadingPanel.SetActive(false);

        if(stopkyObjekt != null) stopkyObjekt.SetActive(true);

        isGameActive = true;
        Time.timeScale = 1; 
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Debug.Log("Quitting Game...");
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1; 
    }

    public void SaveAndExit()
    {
        if(player != null)
        {
            PlayerPrefs.SetFloat("SaveX", player.transform.position.x);
            PlayerPrefs.SetFloat("SaveY", player.transform.position.y);
            PlayerPrefs.SetInt("SaveExists", 1);
            PlayerPrefs.Save();
        }
        QuitGame();
    }

    private void LoadPosition()
    {
        if (PlayerPrefs.HasKey("SaveExists") && player != null)
        {
            float x = PlayerPrefs.GetFloat("SaveX");
            float y = PlayerPrefs.GetFloat("SaveY");
            player.transform.position = new Vector2(x, y);
        }
    }
}