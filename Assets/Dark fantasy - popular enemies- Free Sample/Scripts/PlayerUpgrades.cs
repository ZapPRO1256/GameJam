using System;
using UnityEngine;

/// <summary>
/// Постійні прокачки гравця (меню + рівні), як монети в CoinWallet.
/// </summary>
public static class PlayerUpgrades
{
    public const string DamageBonusKey = "PlayerDamageBonus";
    public const string HealthLevelsKey = "PlayerHealthLevels";

    public const int DamageUpgradeCost = 1;
    public const int HealthUpgradeCost = 1;
    public const int DamagePerLevel = 1;
    public const float HealthPercentPerLevel = 0.1f;

    public static event Action OnUpgradesChanged;

    public static int GetDamageLevels()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(DamageBonusKey, 0));
    }

    public static int GetHealthLevels()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(HealthLevelsKey, 0));
    }

    public static int GetTotalAttackDamage(int baseDamage)
    {
        return baseDamage + GetDamageLevels() * DamagePerLevel;
    }

    public static int GetTotalMaxHealth(int baseMaxHealth)
    {
        float multiplier = 1f + HealthPercentPerLevel * GetHealthLevels();
        return Mathf.Max(1, Mathf.RoundToInt(baseMaxHealth * multiplier));
    }

    public static bool TryBuyDamageUpgrade()
    {
        if (!CoinWallet.SpendCoins(DamageUpgradeCost))
        {
            return false;
        }

        PlayerPrefs.SetInt(DamageBonusKey, GetDamageLevels() + 1);
        SaveAndNotify();
        return true;
    }

    public static bool TryBuyHealthUpgrade()
    {
        if (!CoinWallet.SpendCoins(HealthUpgradeCost))
        {
            return false;
        }

        PlayerPrefs.SetInt(HealthLevelsKey, GetHealthLevels() + 1);
        SaveAndNotify();
        return true;
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(DamageBonusKey);
        PlayerPrefs.DeleteKey(HealthLevelsKey);
        PlayerPrefs.Save();
        OnUpgradesChanged?.Invoke();
    }

    private static void SaveAndNotify()
    {
        PlayerPrefs.Save();
        OnUpgradesChanged?.Invoke();
    }
}
