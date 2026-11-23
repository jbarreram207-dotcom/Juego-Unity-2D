using UnityEngine;

public class DisparoEnemigo : MonoBehaviour
{
    public Transform controladorDisparo;
    public float distanciaLinea;
    public LayerMask Player;
    public bool jugadorEnRango;
    public GameObject balaEnemigo;

    // 🔥 NUEVO: control desde el enemigo
    public float velocidadBala;
    public int dañoBala;

    public float tiempoUltimoDisparo;
    public float tiempoEntreDisparos;
    public float tiempoEsperaDisparo;

    void Update()
    {
        jugadorEnRango = Physics2D.Raycast(
            controladorDisparo.position,
            -transform.right,
            distanciaLinea,
            Player
        );

        if (jugadorEnRango)
        {
            if (Time.time > tiempoEntreDisparos + tiempoUltimoDisparo)
            {
                tiempoUltimoDisparo = Time.time;
                Invoke(nameof(Disparar), tiempoEsperaDisparo);
            }
        }
    }

    private void Disparar()
    {
        GameObject bala = Instantiate(
            balaEnemigo,
            controladorDisparo.position,
            controladorDisparo.rotation
        );

        BalaEnemigo scriptBala = bala.GetComponent<BalaEnemigo>();

        if (scriptBala != null)
        {
            // 🔥 Aquí mandas TODO
            scriptBala.velocidad = velocidadBala;
            scriptBala.daño = dañoBala;
            scriptBala.distanciaMaxima = distanciaLinea;
        }
    }

    void OnDrawGizmos()
    {
        if (controladorDisparo == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            controladorDisparo.position,
            controladorDisparo.position + -transform.right * distanciaLinea
        );
    }
}