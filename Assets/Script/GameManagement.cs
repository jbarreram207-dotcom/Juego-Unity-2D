using System.Collections;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public bool isGameActive = true;

    [Header("Checkpoint")]
    public Vector3 checkpointPosicion;
    private CheckPoint checkpointActual;

    public static GameManagement Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            checkpointPosicion = player.transform.position;
        }
    }

    public void ActualizarCheckpoint(Vector3 nuevaPosicion, CheckPoint nuevoCheckpoint)
    {
        checkpointPosicion = nuevaPosicion;
        checkpointActual = nuevoCheckpoint;
    }

    public void RespawnJugador()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(1f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>().Respawn(checkpointPosicion);
        }
    }
}