using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damagePerTick = 20;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private bool instantKill;

    [Header("Target Filter")]
    [SerializeField] private string targetTag = "player";

    private readonly Dictionary<Health, float> nextDamageTime = new Dictionary<Health, float>();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsTagMatch(other, targetTag))
        {
            return;
        }

        Health health = other.GetComponent<Health>();
        if (health == null || health.IsDead)
        {
            return;
        }

        if (!nextDamageTime.TryGetValue(health, out float allowedTime))
        {
            allowedTime = 0f;
        }

        if (Time.time < allowedTime)
        {
            return;
        }

        int damage = instantKill ? health.CurrentHealth : Mathf.Max(1, damagePerTick);
        health.TakeDamage(damage);
        nextDamageTime[health] = Time.time + Mathf.Max(0.01f, damageInterval);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            nextDamageTime.Remove(health);
        }
    }

    private bool IsTagMatch(Component target, string expectedTag)
    {
        if (target == null || string.IsNullOrWhiteSpace(expectedTag))
        {
            return false;
        }

        return string.Equals(target.tag, expectedTag, System.StringComparison.OrdinalIgnoreCase);
    }
}
