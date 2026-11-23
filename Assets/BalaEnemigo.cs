using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    public float velocidad;
    public int daño;
    public float distanciaMaxima;

    private Vector3 posicionInicial;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * -transform.right);

        float distanciaRecorrida = Vector3.Distance(posicionInicial, transform.position);

        if (distanciaRecorrida >= distanciaMaxima)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.Hit(daño, Vector2.zero);
            Destroy(gameObject);
        }
    }
}