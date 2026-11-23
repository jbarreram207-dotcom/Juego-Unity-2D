using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class ThingEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Damageable damageable;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.left;

    [Header("Jugador")]
    public Transform player; // 🔥 asignar en inspector

    [Header("Límites")]
    public Transform leftLimit;
    public Transform rightLimit;

    [Header("Daño por contacto")]
    [SerializeField] public float dañoContacto = 10f;
    [SerializeField] public float cooldownDaño = 0.5f;
    private float tiempoUltimoDaño;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            _walkDirection = value;

            if (value == WalkableDirection.Right)
            {
                walkDirectionVector = Vector2.right;

                transform.localScale = new Vector2(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y
                );
            }
            else
            {
                walkDirectionVector = Vector2.left;

                transform.localScale = new Vector2(
                    -Mathf.Abs(transform.localScale.x),
                    transform.localScale.y
                );
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        WalkDirection = WalkableDirection.Left;
    }

    void FixedUpdate()
    {
        if (damageable.LockVelocity) return;

        if (PlayerDentroDeLimites())
        {
            SeguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    // 🔥 PERSEGUIR
    void SeguirJugador()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        if (direction.x > 0)
            WalkDirection = WalkableDirection.Right;
        else if (direction.x < 0)
            WalkDirection = WalkableDirection.Left;
    }

    // 🔁 PATRULLAR (tu lógica original)
    void Patrullar()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        rb.velocity = new Vector2(moveSpeed * walkDirectionVector.x, rb.velocity.y);
    }

    // 📏 PLAYER ENTRE LÍMITES
    bool PlayerDentroDeLimites()
    {
        if (player == null) return false;

        return player.position.x >= leftLimit.position.x &&
               player.position.x <= rightLimit.position.x;
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else
            WalkDirection = WalkableDirection.Right;
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    // 🔥 DAÑO POR CONTACTO (sin bugs)
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (Time.time >= tiempoUltimoDaño + cooldownDaño)
        {
            Damageable d = collision.GetComponent<Damageable>();

            if (d != null)
            {
                Vector2 direccion = (collision.transform.position - transform.position).normalized;
                Vector2 knockback = new Vector2(direccion.x * 6f, 4f);

                d.Hit((int)dañoContacto, knockback);
            }

            tiempoUltimoDaño = Time.time;
        }
    }
}