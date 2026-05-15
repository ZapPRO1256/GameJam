using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Портал завершення рівня. Collider2D з Is Trigger, взаємодія клавішею E.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelExitPortal : MonoBehaviour
{
    [Header("Level")]
    [Tooltip("0 = визначити автоматично з імені сцени (Level1 → 1)")]
    [SerializeField] private int levelNumber;
    [SerializeField] private int maxLevelInGame = 10;

    [Header("Scene")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private bool useMenuBuildIndex;
    [SerializeField] private int menuBuildIndex;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";

    [Header("UI (optional)")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "E — завершити рівень";

    private PlayerController2D playerInRange;
    private bool isCompleting;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (levelNumber <= 0)
        {
            levelNumber = LevelProgress.GetCurrentLevelNumber();
        }

        if (promptRoot != null)
        {
            promptRoot.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player == null)
        {
            return;
        }

        playerInRange = player;
        player.SetExitPortal(this);
        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player == null || playerInRange != player)
        {
            return;
        }

        player.ClearExitPortal(this);
        playerInRange = null;
        HidePrompt();
    }

    public bool TryCompleteLevel()
    {
        if (isCompleting || playerInRange == null)
        {
            return false;
        }

        Health health = playerInRange.GetComponent<Health>();
        if (health != null && health.IsDead)
        {
            return false;
        }

        isCompleting = true;

        if (levelNumber > 0)
        {
            LevelProgress.UnlockLevel(levelNumber + 1, maxLevelInGame);
        }
        else
        {
            LevelProgress.CompleteCurrentLevel(maxLevelInGame);
        }

        Time.timeScale = 1f;
        LoadMenu();
        return true;
    }

    private void LoadMenu()
    {
        if (useMenuBuildIndex)
        {
            SceneManager.LoadScene(menuBuildIndex);
            return;
        }

        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private void ShowPrompt()
    {
        if (promptRoot != null)
        {
            promptRoot.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }

    private void HidePrompt()
    {
        if (promptRoot != null)
        {
            promptRoot.SetActive(false);
        }
    }
}
