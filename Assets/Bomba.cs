using UnityEngine;

public class Bomba : MonoBehaviour
{
    [Header("Explosión")]
    [SerializeField] private float radioExplosion = 2f;
    [SerializeField] private float daño = 20f;

    [Header("Capas")]
    [SerializeField] private LayerMask capasAfectadas;

    [Header("Tiempo")]
    [SerializeField] private float tiempoVida = 5f;

    private bool haExplotado = false;

    void Start()
    {
        // Por si no toca nada
        Invoke(nameof(Explotar), tiempoVida);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (haExplotado) return;

        // 🔥 SOLO explota con suelo o player
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Player"))
        {
            Explotar();
        }
    }

    private void Explotar()
    {
        haExplotado = true;

        // 🔥 detectar en área
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, radioExplosion, capasAfectadas);

        foreach (Collider2D obj in objetos)
        {
            Damageable d = obj.GetComponent<Damageable>();

            if (d != null)
            {
                Vector2 knockback = (obj.transform.position - transform.position).normalized;
                d.Hit((int)daño, knockback);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}