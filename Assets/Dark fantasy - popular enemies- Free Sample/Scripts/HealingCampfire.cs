using TMPro;
using UnityEngine;

/// <summary>
/// Місце зцілення (костер). Потрібен Collider2D з Is Trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HealingCampfire : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private int healCost = 1;
    [SerializeField] private string playerTag = "Player";

    [Header("UI (optional)")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptFormat = "E — зцілитися ({0} монета)";

    private PlayerController2D playerInRange;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

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
        player.SetHealingCampfire(this);
        UpdatePrompt();
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

        player.ClearHealingCampfire(this);
        playerInRange = null;
        HidePrompt();
    }

    public bool TryHealPlayer(Health playerHealth)
    {
        if (playerHealth == null || playerHealth.IsDead)
        {
            return false;
        }

        if (playerHealth.IsFullHealth)
        {
            return false;
        }

        if (!CoinWallet.SpendCoins(healCost))
        {
            return false;
        }

        playerHealth.HealToFull();
        UpdatePrompt();
        return true;
    }

    public bool CanHeal(Health playerHealth)
    {
        if (playerHealth == null || playerHealth.IsDead || playerHealth.IsFullHealth)
        {
            return false;
        }

        return CoinWallet.GetCoins() >= healCost;
    }

    public int HealCost => healCost;

    private void UpdatePrompt()
    {
        if (promptRoot == null)
        {
            return;
        }

        bool show = playerInRange != null;
        promptRoot.SetActive(show);

        if (show && promptText != null)
        {
            promptText.text = string.Format(promptFormat, healCost);
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
