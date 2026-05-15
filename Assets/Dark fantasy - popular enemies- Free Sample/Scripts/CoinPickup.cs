using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    [Header("Drop")]
    [SerializeField] private float popUpForce = 5f;
    [SerializeField] private float spreadForce = 2.5f;
    [SerializeField] private float settleDelay = 0.35f;

    [Header("Pickup")]
    [SerializeField] private float magnetRadius = 2.2f;
    [SerializeField] private float magnetSpeed = 12f;
    [SerializeField] private string playerTag = "Player";

    [Header("Visual")]
    [SerializeField] private float spinSpeed = 220f;
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobSpeed = 6f;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isSettled;
    private bool isCollected;
    private float bobOffset;
    private Vector3 basePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        rb.gravityScale = 2.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Викликай після Instantiate, якщо монета спавниться з ворога.
    /// </summary>
    public void LaunchFrom(Vector3 origin)
    {
        transform.position = origin + Vector3.up * 0.35f;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        rb.linearVelocity = new Vector2(
            Random.Range(-spreadForce, spreadForce),
            Random.Range(popUpForce * 0.75f, popUpForce));

        StartCoroutine(SettleRoutine());
    }

    private IEnumerator SettleRoutine()
    {
        yield return new WaitForSeconds(settleDelay);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        isSettled = true;
        basePosition = transform.position;
    }

    private void Update()
    {
        if (isCollected)
        {
            return;
        }

        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        if (!isSettled)
        {
            return;
        }

        FindPlayer();
        if (playerTransform != null &&
            Vector2.Distance(transform.position, playerTransform.position) <= magnetRadius)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                magnetSpeed * Time.deltaTime);
        }
        else
        {
            float bob = Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmplitude;
            transform.position = basePosition + Vector3.up * bob;
        }
    }

    private void FindPlayer()
    {
        if (playerTransform != null)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag(playerTag))
        {
            return;
        }

        isCollected = true;
        CoinWallet.AddCoins(coinValue);
        AudioManager.Instance?.PlayCoin();
        Destroy(gameObject);
    }
}
