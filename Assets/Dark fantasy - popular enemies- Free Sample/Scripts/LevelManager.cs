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

    private const string LEVEL_KEY = LevelProgress.LevelKey;

    // =====================================================
    // START
    // =====================================================
    private void Start()
    {
        InitializeData();

        UpdateLevelObjects();
        UpdateCoinsUI();
    }

    private void OnEnable()
    {
        CoinWallet.OnCoinsChanged += OnCoinsChanged;
    }

    private void OnDisable()
    {
        CoinWallet.OnCoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int _)
    {
        UpdateCoinsUI();
    }

    // =====================================================
    // �??�?��?�
    // =====================================================
    private void InitializeData()
    {
        // ������ ����� ��������
        LevelProgress.EnsureInitialized(1);

        // ��������� ������
        CoinWallet.EnsureInitialized(startCoins);
        PlayerPrefs.Save();
    }

    // =====================================================
    // LEVEL SYSTEM
    // =====================================================

    public int GetUnlockedLevel()
    {
        return LevelProgress.GetUnlockedLevel(1);
    }

    public void UnlockLevel(int level)
    {
        LevelProgress.UnlockLevel(level, maxLevelInGame);
        UpdateLevelObjects();
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
            Debug.Log("?�� �� �� ��������!");
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
    // SHOP UPGRADES
    // =====================================================

    public void BuyDamageUpgrade()
    {
        if (PlayerUpgrades.TryBuyDamageUpgrade())
        {
            UpdateCoinsUI();
            Debug.Log($"Урон: +{PlayerUpgrades.DamagePerLevel} (рівень {PlayerUpgrades.GetDamageLevels()})");
        }
        else
        {
            Debug.Log("Недостатньо монет для прокачки урону!");
        }
    }

    public void BuyHealthUpgrade()
    {
        if (PlayerUpgrades.TryBuyHealthUpgrade())
        {
            UpdateCoinsUI();
            Debug.Log($"HP +10% (рівень {PlayerUpgrades.GetHealthLevels()})");
        }
        else
        {
            Debug.Log("Недостатньо монет для прокачки HP!");
        }
    }

    // =====================================================
    // COINS SYSTEM
    // =====================================================

    public int GetCoins()
    {
        return CoinWallet.GetCoins(startCoins);
    }

    public void AddCoins(int amount)
    {
        CoinWallet.AddCoins(amount);
        UpdateCoinsUI();
    }

    public bool SpendCoins(int amount)
    {
        if (CoinWallet.SpendCoins(amount))
        {
            UpdateCoinsUI();
            return true;
        }

        Debug.Log("����������� �����!");
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
    // RESET (��� �����)
    // =====================================================

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelProgress.LevelKey);
        PlayerPrefs.DeleteKey(CoinWallet.CoinsKey);
        PlayerUpgrades.Reset();

        InitializeData();

        UpdateLevelObjects();
        UpdateCoinsUI();
    }
}