using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Damageable))]
public class Guardian : MonoBehaviour
{
    private Animator animator;
    public Rigidbody2D rb2D;
    public Transform Player;

    private Damageable damageable;
    private bool muriendo = false;

    [Header("Ataque")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float dañoAtaque;

    [Header("Daño por contacto")]
    [SerializeField] private float dañoContacto = 10f;
    [SerializeField] private float cooldownDaño = 1f;
    private float tiempoUltimoDaño;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            Player = playerObj.transform;
    }

    private void Update()
    {
        // 🔥 bloquear TODO si no puede moverse
        if (!CanMove)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            return;
        }

        if (Player != null && !muriendo)
        {
            float distanciaJugador = Vector2.Distance(transform.position, Player.position);
            animator.SetFloat("distanciaJugador", distanciaJugador);
        }

        // 🔥 detectar muerte UNA SOLA VEZ
        if (damageable != null && !damageable.IsAlive && !muriendo)
        {
            muriendo = true;
            animator.SetTrigger("Muerte");
        }
    }

    // 🟢 ATAQUE NORMAL
    public void Ataque()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);

        foreach (Collider2D collision in objetos)
        {
            Damageable d = collision.GetComponent<Damageable>();

            if (d != null)
            {
                d.Hit((int)dañoAtaque, Vector2.zero);
            }
        }
    }

    // 🔒 CONTROL DE MOVIMIENTO DESDE ANIMATOR
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    // 💥 DAÑO POR CONTACTO
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (muriendo) return;

        if (collision.CompareTag("Player"))
        {
            if (Time.time >= tiempoUltimoDaño + cooldownDaño)
            {
                Damageable d = collision.GetComponent<Damageable>();

                if (d != null)
                {
                    Vector2 direccion = (collision.transform.position - transform.position).normalized;

                    // 🔥 mejor knockback
                    Vector2 knockback = new Vector2(direccion.x * 6f, 4f);

                    d.Hit((int)dañoContacto, knockback);
                }

                tiempoUltimoDaño = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorAtaque == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb2D.velocity = new Vector2(knockback.x, rb2D.velocity.y + knockback.y);
    }
}