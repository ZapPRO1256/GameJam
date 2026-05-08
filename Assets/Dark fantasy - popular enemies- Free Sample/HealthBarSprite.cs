using UnityEngine;

public class HealthBarSprite : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Transform fillTransform;

    private Vector3 initialScale;

    private void Awake()
    {
        if (fillTransform == null)
        {
            fillTransform = transform;
        }

        initialScale = fillTransform.localScale;
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    public void SetTarget(Health health)
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= HandleHealthChanged;
        }

        targetHealth = health;

        if (isActiveAndEnabled && targetHealth != null)
        {
            targetHealth.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        }
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (fillTransform == null)
        {
            return;
        }

        float normalized = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        normalized = Mathf.Clamp01(normalized);

        fillTransform.localScale = new Vector3(initialScale.x * normalized, initialScale.y, initialScale.z);
    }
}
