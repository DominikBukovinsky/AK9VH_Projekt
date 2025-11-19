using UnityEngine;
using UnityEngine.SceneManagement; // Potřeba pro restart (volitelné)

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject player; // Odkaz na žábu pro uložení pozice

    private bool isGameActive = false;
    private bool isPaused = false;

    void Start()
    {
        // Zastaví čas a ukáže menu při startu
        Time.timeScale = 0; 
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        
        LoadPosition(); // Načte pozici, pokud existuje save
    }

    void Update()
    {
        // Řeší ESC pouze pokud už hra začala
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- Funkce pro tlačítka ---

    public void StartGame()
    {
        isGameActive = true;
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1; // Rozběhne hru
    }

    public void QuitGame()
    {
        // Funguje v buildu
        Application.Quit();

        // Funguje v editoru (vypne Play Mode)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Debug.Log("Quitting Game...");
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0; // Zmrazí hru
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1; // Obnoví hru
    }

    public void SaveAndExit()
    {
        PlayerPrefs.SetFloat("SaveX", player.transform.position.x);
        PlayerPrefs.SetFloat("SaveY", player.transform.position.y);
        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.Save();

        // Funguje v buildu
        Application.Quit();

        // Funguje v editoru
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Debug.Log("Saved & Exited");
    }

    // --- Interní logika ---

    private void LoadPosition()
    {
        if (PlayerPrefs.HasKey("SaveExists"))
        {
            float x = PlayerPrefs.GetFloat("SaveX");
            float y = PlayerPrefs.GetFloat("SaveY");
            player.transform.position = new Vector2(x, y);
        }
    }
}