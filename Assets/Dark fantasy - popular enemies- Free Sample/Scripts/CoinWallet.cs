using System;
using UnityEngine;

/// <summary>
/// Спільний баланс монет (меню + рівні). Ключ PlayerPrefs той самий, що в GameManager.
/// </summary>
public static class CoinWallet
{
    public const string CoinsKey = "PlayerCoins";

    public static event Action<int> OnCoinsChanged;

    public static int GetCoins(int defaultCoins = 10)
    {
        return PlayerPrefs.GetInt(CoinsKey, defaultCoins);
    }

    public static void EnsureInitialized(int startCoins)
    {
        if (!PlayerPrefs.HasKey(CoinsKey))
        {
            SetCoins(startCoins);
        }
    }

    public static void SetCoins(int amount)
    {
        amount = Math.Max(0, amount);
        PlayerPrefs.SetInt(CoinsKey, amount);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(amount);
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        SetCoins(GetCoins() + amount);
    }

    public static bool SpendCoins(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        int coins = GetCoins();
        if (coins < amount)
        {
            return false;
        }

        SetCoins(coins - amount);
        return true;
    }
}
