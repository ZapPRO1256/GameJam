using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Health playerHealth;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private bool pauseGameOnDeath = true;

    private bool deathHandled;

    private void Awake()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandlePlayerDied;
        }
    }

    private void HandlePlayerDied(Health deadHealth)
    {
        if (deathHandled || deadHealth != playerHealth)
        {
            return;
        }

        deathHandled = true;

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        AudioManager.Instance?.PlayFail();

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
