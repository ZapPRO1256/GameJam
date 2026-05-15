using TMPro;
using UnityEngine;

/// <summary>
/// UI монет на рівні. Признач Coins Text (TMP) у Inspector.
/// </summary>
public class CoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private string prefix = "Coins: ";
    [SerializeField] private int defaultStartCoins = 10;

    private void OnEnable()
    {
        CoinWallet.OnCoinsChanged += Refresh;
    }

    private void OnDisable()
    {
        CoinWallet.OnCoinsChanged -= Refresh;
    }

    private void Start()
    {
        CoinWallet.EnsureInitialized(defaultStartCoins);
        Refresh(CoinWallet.GetCoins(defaultStartCoins));
    }

    private void Refresh(int total)
    {
        if (coinsText != null)
        {
            coinsText.text = prefix + total;
        }
    }
}
