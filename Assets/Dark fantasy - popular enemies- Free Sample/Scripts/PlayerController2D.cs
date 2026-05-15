using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private string enemyTag = "enemy";

    [Header("Animation State Names")]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleState = "Skeleton_Idle";
    [SerializeField] private string moveState = "Skeleton_Running";
    [SerializeField] private string attackState = "Skeleton_Attack";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private Rigidbody2D rb;
    private Health health;
    private HealingCampfire nearbyCampfire;
    private LevelExitPortal nearbyPortal;
    private float moveInput;
    private bool isGrounded;
    private float nextAttackTime;
    private bool isFacingRight = true;
    private bool isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }

        if (!isAttacking)
        {
            UpdateMovementAnimation();
        }

        HandleFlip();
    }

    private void FixedUpdate()
    {
        if (health.IsDead || isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        PlayState(attackState);
        AudioManager.Instance?.PlayAttack();

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!IsTagMatch(hits[i], enemyTag))
            {
                continue;
            }

            Health enemyHealth = hits[i].GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }

        Invoke(nameof(ResetAttack), attackCooldown * 0.7f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void SetHealingCampfire(HealingCampfire campfire)
    {
        nearbyCampfire = campfire;
    }

    public void ClearHealingCampfire(HealingCampfire campfire)
    {
        if (nearbyCampfire == campfire)
        {
            nearbyCampfire = null;
        }
    }

    public void SetExitPortal(LevelExitPortal portal)
    {
        nearbyPortal = portal;
    }

    public void ClearExitPortal(LevelExitPortal portal)
    {
        if (nearbyPortal == portal)
        {
            nearbyPortal = null;
        }
    }

    private void TryInteract()
    {
        if (nearbyPortal != null)
        {
            nearbyPortal.TryCompleteLevel();
            return;
        }

        TryHealAtCampfire();
    }

    private void TryHealAtCampfire()
    {
        if (nearbyCampfire == null)
        {
            return;
        }

        if (health.IsFullHealth)
        {
            return;
        }

        if (!nearbyCampfire.CanHeal(health))
        {
            Debug.Log("Недостатньо монет для зцілення!");
            return;
        }

        nearbyCampfire.TryHealPlayer(health);
    }

    private bool CheckGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

    private void HandleFlip()
    {
        if (moveInput > 0f && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0f && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void UpdateMovementAnimation()
    {
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            PlayState(moveState);
        }
        else
        {
            PlayState(idleState);
        }
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

    private bool IsTagMatch(Component target, string expectedTag)
    {
        if (target == null || string.IsNullOrWhiteSpace(expectedTag))
        {
            return false;
        }

        return string.Equals(target.tag, expectedTag, System.StringComparison.OrdinalIgnoreCase);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
