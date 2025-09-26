using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _mainMenuUI;
    private bool gameHasEnded = false;
    private bool isPaused = false;
    private static bool firstLoad = true;

    private void Start()
    {
        if (firstLoad)
        {
            if (_mainMenuUI != null)
                _mainMenuUI.SetActive(true);

            Time.timeScale = 0f;
            firstLoad = false;
        }

        else
        {
            // Restart veya sahne reload ise paneli gizle
            if (_mainMenuUI != null)
                _mainMenuUI.SetActive(false);

            Time.timeScale = 1f;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameHasEnded)
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }
    
    public void EndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            _gameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void StartTheGame()
    {
        if (_mainMenuUI != null)
            _mainMenuUI.SetActive(false);
        gameHasEnded = false;
        isPaused = false;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void RestartTheGame()
    {
        Time.timeScale = 1f; // oyunu tekrar başlat
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        gameHasEnded = false;
        isPaused = false;
        Time.timeScale = 0f;
        _mainMenuUI.SetActive(true);
    }

     public void PauseGame()
    {
        isPaused = true;
        if (_pauseUI != null) _pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (_pauseUI != null) _pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }
    

}
