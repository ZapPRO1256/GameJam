using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private int maxLevelInGame = 10;

    [Tooltip("������/��'���� �����")]
    [SerializeField] private List<GameObject> levelObjects;

    [Header("Level Panel")]
    [SerializeField] private GameObject levelPanel;

    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;

    [Header("Currency")]
    [SerializeField] private int startCoins = 10;

    [Tooltip("����� ������� (������'������)")]
    [SerializeField] private TMP_Text coinsText;

    private const string LEVEL_KEY = "UnlockedLevel";
    private const string COINS_KEY = "PlayerCoins";

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        InitializeData();

        UpdateLevelObjects();
        UpdateCoinsUI();
    }

    // =====================================================
    // ІНІЦІАЛІЗАЦІЯ
    // =====================================================
    private void InitializeData()
    {
        // Перший рівень відкритий
        if (!PlayerPrefs.HasKey(LEVEL_KEY))
        {
            PlayerPrefs.SetInt(LEVEL_KEY, 1);
        }

        // Стартовий баланс
        if (!PlayerPrefs.HasKey(COINS_KEY))
        {
            PlayerPrefs.SetInt(COINS_KEY, startCoins);
        }

        PlayerPrefs.Save();
    }

    // =====================================================
    // LEVEL SYSTEM
    // =====================================================

    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    public void UnlockLevel(int level)
    {
        int currentUnlocked = GetUnlockedLevel();

        level = Mathf.Clamp(level, 1, maxLevelInGame);

        if (level > currentUnlocked)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, level);
            PlayerPrefs.Save();

            UpdateLevelObjects();
        }
    }

    public void LoadHighestUnlockedLevel()
    {
        int unlockedLevel = GetUnlockedLevel();

        string sceneName = "Level" + unlockedLevel;

        SceneManager.LoadScene(sceneName);
    }

    public void LoadLevel(int level)
    {
        if (level <= GetUnlockedLevel())
        {
            SceneManager.LoadScene("Level" + level);
        }
        else
        {
            Debug.Log("Рівень ще не відкритий!");
        }
    }

    private void UpdateLevelObjects()
    {
        int unlockedLevel = GetUnlockedLevel();

        for (int i = 0; i < levelObjects.Count; i++)
        {
            bool isUnlocked = (i + 1) <= unlockedLevel;

            if (levelObjects[i] != null)
            {
                levelObjects[i].SetActive(isUnlocked);
            }
        }
    }

    // =====================================================
    // LEVEL PANEL
    // =====================================================

    public void OpenLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(true);
        }
    }

    public void CloseLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(false);
        }
    }

    public void ToggleLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(!levelPanel.activeSelf);
        }
    }

    // =====================================================
    // SHOP PANEL
    // =====================================================

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void ToggleShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }

    // =====================================================
    // COINS SYSTEM
    // =====================================================

    public int GetCoins()
    {
        return PlayerPrefs.GetInt(COINS_KEY, startCoins);
    }

    public void AddCoins(int amount)
    {
        int coins = GetCoins();

        coins += amount;

        PlayerPrefs.SetInt(COINS_KEY, coins);
        PlayerPrefs.Save();

        UpdateCoinsUI();
    }

    public bool SpendCoins(int amount)
    {
        int coins = GetCoins();

        if (coins >= amount)
        {
            coins -= amount;

            PlayerPrefs.SetInt(COINS_KEY, coins);
            PlayerPrefs.Save();

            UpdateCoinsUI();

            return true;
        }

        Debug.Log("Недостатньо монет!");
        return false;
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = GetCoins().ToString();
        }
    }

    // =====================================================
    // RESET (для тестів)
    // =====================================================

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.DeleteKey(COINS_KEY);

        InitializeData();

        UpdateLevelObjects();
        UpdateCoinsUI();
    }
}