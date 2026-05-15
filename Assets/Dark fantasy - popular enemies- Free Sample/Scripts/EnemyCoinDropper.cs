using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyCoinDropper : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 2;
    [SerializeField] private int maxCoins = 5;
    [SerializeField] private float spawnDelay = 0.12f;
    [SerializeField] private float horizontalSpread = 0.45f;

    private Health health;
    private bool dropped;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        health.OnDied -= OnEnemyDied;
    }

    private void OnEnemyDied(Health _)
    {
        if (dropped || coinPrefab == null)
        {
            return;
        }

        dropped = true;
        AudioManager.Instance?.PlayEnemyDeath();
        StartCoroutine(DropRoutine());
    }

    private IEnumerator DropRoutine()
    {
        int count = Random.Range(minCoins, maxCoins + 1);
        Vector3 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-horizontalSpread, horizontalSpread), 0.2f, 0f);
            GameObject coin = Instantiate(coinPrefab, origin + offset, Quaternion.identity);

            CoinPickup pickup = coin.GetComponent<CoinPickup>();
            pickup?.LaunchFrom(origin);

            if (spawnDelay > 0f)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
