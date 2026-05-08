using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class GoblinEnemyController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool startMovingRight = false;
    [SerializeField] private bool spriteFacesRightByDefault = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallTurnCooldown = 0.2f;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string playerTag = "player";

    [Header("Animation State Names")]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleState = "Goblin_Idle";
    [SerializeField] private string moveState = "Goblin_Running";
    [SerializeField] private string attackState = "Goblin_Attack";

    private Rigidbody2D rb;
    private Health health;
    private bool isFacingRight;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextWallTurnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        isFacingRight = startMovingRight;
        ApplyVisualFacing();
    }

    private void Update()
    {
        if (health.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (TryAttackPlayer())
        {
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (isAttacking)
        {
            return;
        }

        bool blockedByWall = IsWallAhead();
        if (blockedByWall && Time.time >= nextWallTurnTime)
        {
            JumpFromWall();
            Flip();
            nextWallTurnTime = Time.time + wallTurnCooldown;
        }

        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            PlayState(moveState);
        }
        else
        {
            PlayState(idleState);
        }
    }

    private bool TryAttackPlayer()
    {
        if (Time.time < nextAttackTime || attackPoint == null)
        {
            return false;
        }

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hit == null || !IsTagMatch(hit, playerTag))
        {
            return false;
        }

        Health playerHealth = hit.GetComponent<Health>();
        if (playerHealth == null || playerHealth.IsDead)
        {
            return false;
        }

        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        nextAttackTime = Time.time + attackCooldown;

        Vector2 targetDirection = hit.transform.position - transform.position;
        if ((targetDirection.x > 0f && !isFacingRight) || (targetDirection.x < 0f && isFacingRight))
        {
            Flip();
        }

        PlayState(attackState);
        playerHealth.TakeDamage(attackDamage);
        Invoke(nameof(ResetAttack), attackCooldown * 0.7f);
        return true;
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

    private bool IsWallAhead()
    {
        if (wallCheck == null)
        {
            return false;
        }

        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void JumpFromWall()
    {
        if (!IsGrounded())
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallJumpForce);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        ApplyVisualFacing();
    }

    private void ApplyVisualFacing()
    {
        float absX = Mathf.Abs(transform.localScale.x);
        float absY = transform.localScale.y;
        float absZ = transform.localScale.z;

        bool useDefaultOrientation = isFacingRight == spriteFacesRightByDefault;
        float xSign = useDefaultOrientation ? 1f : -1f;
        transform.localScale = new Vector3(absX * xSign, absY, absZ);
    }

    private bool IsTagMatch(Component target, string expectedTag)
    {
        if (target == null || string.IsNullOrWhiteSpace(expectedTag))
        {
            return false;
        }

        return string.Equals(target.tag, expectedTag, System.StringComparison.OrdinalIgnoreCase);
    }

    private void PlayState(string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName))
        {
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            animator.Play(stateName, 0, 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }
    }
}
