using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitFlashDuration = 0.2f;
    [SerializeField] private int flashRepeats = 4;

    [Header("Optional")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathAnimationState = "";
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.8f;

    [Header("Audio")]
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool playDamageSound = true;
    [SerializeField] private bool playHealSound = true;

    private int baseMaxHealth;
    private int currentHealth;
    private bool isDead;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine hitFlashRoutine;

    public event Action<Health> OnDied;
    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int BaseMaxHealth => baseMaxHealth;
    public bool IsDead => isDead;
    public bool IsFullHealth => !isDead && currentHealth >= maxHealth;

    private void Awake()
    {
        baseMaxHealth = maxHealth;
        currentHealth = maxHealth;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void Start()
    {
        if (isPlayer)
        {
            SetMaxHealth(PlayerUpgrades.GetTotalMaxHealth(BaseMaxHealth), fillToMax: true);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (hitFlashRoutine != null)
        {
            StopCoroutine(hitFlashRoutine);
        }
        hitFlashRoutine = StartCoroutine(HitFlashRoutine());

        if (playDamageSound && isPlayer)
        {
            AudioManager.Instance?.PlayDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (playHealSound && isPlayer)
        {
            AudioManager.Instance?.PlayHeal();
        }
    }

    public void HealToFull()
    {
        if (isDead || IsFullHealth)
        {
            return;
        }

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (playHealSound && isPlayer)
        {
            AudioManager.Instance?.PlayHeal();
        }
    }

    /// <summary>
    /// Встановлює максимум HP (прокачка). baseMaxHealth не змінюється.
    /// </summary>
    public void SetMaxHealth(int newMaxHealth, bool fillToMax = true)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);

        if (!isDead)
        {
            currentHealth = fillToMax ? maxHealth : Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null && !string.IsNullOrWhiteSpace(deathAnimationState))
        {
            animator.Play(deathAnimationState, 0, 0f);
        }

        OnDied?.Invoke(this);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, Mathf.Max(0f, destroyDelay));
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        for (int i = 0; i < flashRepeats; i++)
        {
            SetColor(Color.red);
            yield return new WaitForSeconds(hitFlashDuration);
            RestoreOriginalColors();
            yield return new WaitForSeconds(hitFlashDuration);
        }
    }

    private void SetColor(Color color)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = color;
        }
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }
}
