using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Прогрес рівнів (той самий PlayerPrefs ключ, що в GameManager).
/// </summary>
public static class LevelProgress
{
    public const string LevelKey = "UnlockedLevel";

    public static int GetUnlockedLevel(int defaultLevel = 1)
    {
        return PlayerPrefs.GetInt(LevelKey, defaultLevel);
    }

    public static void EnsureInitialized(int firstUnlockedLevel = 1)
    {
        if (!PlayerPrefs.HasKey(LevelKey))
        {
            PlayerPrefs.SetInt(LevelKey, firstUnlockedLevel);
            PlayerPrefs.Save();
        }
    }

    public static void UnlockLevel(int level, int maxLevelInGame = 10)
    {
        level = Mathf.Clamp(level, 1, maxLevelInGame);
        int current = GetUnlockedLevel();

        if (level > current)
        {
            PlayerPrefs.SetInt(LevelKey, level);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Повертає номер рівня з імені сцени "Level1" → 1. Якщо не вдалось — 0.
    /// </summary>
    public static int GetLevelNumberFromScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) || !sceneName.StartsWith("Level"))
        {
            return 0;
        }

        string numberPart = sceneName.Substring("Level".Length);
        return int.TryParse(numberPart, out int level) ? level : 0;
    }

    public static int GetCurrentLevelNumber()
    {
        return GetLevelNumberFromScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Завершення поточного рівня: відкриває наступний.
    /// </summary>
    public static void CompleteCurrentLevel(int maxLevelInGame = 10)
    {
        int current = GetCurrentLevelNumber();
        if (current <= 0)
        {
            return;
        }

        UnlockLevel(current + 1, maxLevelInGame);
    }
}
