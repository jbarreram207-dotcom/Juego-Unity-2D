using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class BeastSwamp : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Detection")]
    public DetectionZone detectionZone; // seguir
    public DetectionZone attackRange;   // decidir ataque

    [Header("Attack")]
    public GameObject attackZone; // hitbox de daño

    Rigidbody2D rb;
    Animator animator;
    Damageable damageable;

    Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Start()
    {
        if (attackZone != null)
            attackZone.SetActive(false);
    }

    void Update()
    {
        FindTarget();

        bool hasTarget = target != null;
        bool inRange = IsInAttackRange();

        animator.SetBool("hasTarget", hasTarget);
        animator.SetBool("inRange", inRange);
    }

    void FixedUpdate()
    {
        if (damageable.LockVelocity) return;

        if (target != null && !IsInAttackRange())
        {
            Vector2 direction = (target.position - transform.position).normalized;

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // 🔄 flip para sprite que mira a la izquierda
            if (direction.x > 0 && transform.localScale.x > 0)
                Flip();
            else if (direction.x < 0 && transform.localScale.x < 0)
                Flip();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void FindTarget()
    {
        target = null;

        foreach (Collider2D col in detectionZone.detectedColliders)
        {
            if (col.CompareTag("Player"))
            {
                target = col.transform;
                break;
            }
        }
    }

    bool IsInAttackRange()
    {
        foreach (Collider2D col in attackRange.detectedColliders)
        {
            if (col.CompareTag("Player"))
                return true;
        }
        return false;
    }

    void Flip()
    {
        transform.localScale = new Vector2(
            transform.localScale.x * -1,
            transform.localScale.y
        );
    }

    public void EnableAttack()
    {
        if (attackZone != null)
            attackZone.SetActive(true);
    }

    public void DisableAttack()
    {
        if (attackZone != null)
            attackZone.SetActive(false);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}