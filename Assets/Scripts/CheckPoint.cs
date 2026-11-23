using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Sprite spriteActivo;
    private SpriteRenderer spriteRenderer;
    private bool checkpointActivo = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.CompareTag("Player") && !checkpointActivo)
        {
            ActivarCheckPoint();
        }
    }

    void ActivarCheckPoint()
    {
        if(GameManagement.Instance != null)
        {
            GameManagement.Instance.ActualizarCheckpoint(transform.position, this);
        }
        checkpointActivo = true;

        if(spriteRenderer != null && spriteActivo != null)
        {
            spriteRenderer.sprite = spriteActivo;
        }

        spriteRenderer.color = new Color(0.615f, 0.611f, 0.611f);
    }
}
