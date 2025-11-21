using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject player;
    public Button continueButton;
    
    [Header("Systems")]
    public Stopky stopky;

    private bool isGameActive = false;
    private bool isPaused = false;
    private Vector2 startPosition;

    void Start()
    {
        Time.timeScale = 0; // Tím se zastaví i Timer
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);

        startPosition = player.transform.position;

        if (PlayerPrefs.HasKey("SaveExists"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    void Update()
    {
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();

        // 1. Reset pozice
        player.transform.position = startPosition;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if(rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Reset času
        if(stopky != null) stopky.ResetCas();

        StartGameLogic();
    }

    public void ContinueGame()
    {
        LoadGameData();
        StartGameLogic();
    }

    private void StartGameLogic()
    {
        isGameActive = true;
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1; // Tím se rozběhne Timer
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0; // Zastaví Timer
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1; // Rozběhne Timer
    }

    public void SaveAndExitToMenu()
    {
        // 1. Uložení
        PlayerPrefs.SetFloat("SaveX", player.transform.position.x);
        PlayerPrefs.SetFloat("SaveY", player.transform.position.y);
        
        if (stopky != null)
        {
            PlayerPrefs.SetFloat("SaveTime", stopky.ZiskatAktualniCas());
        }

        PlayerPrefs.SetInt("SaveExists", 1);
        PlayerPrefs.Save();

        isGameActive = false;
        isPaused = false;

        pauseMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        
        Time.timeScale = 0;

        CheckContinueButton();

        Debug.Log("Saved & Returned to Menu");
    }

    private void LoadGameData()
    {
        if (PlayerPrefs.HasKey("SaveExists"))
        {
            // Načtení pozice
            float x = PlayerPrefs.GetFloat("SaveX");
            float y = PlayerPrefs.GetFloat("SaveY");
            player.transform.position = new Vector2(x, y);

            // Načtení času
            float time = PlayerPrefs.GetFloat("SaveTime", 0f);
            if(stopky != null) stopky.NacistCas(time);
        }
    }
    private void CheckContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = PlayerPrefs.HasKey("SaveExists");
        }
    }
}