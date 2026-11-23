using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BossActivation : MonoBehaviour
{   
    public GameObject bossGO;

    private void Start()
    {
        bossGO.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
{
    if(collision.CompareTag("Player"))
    {
        BossUI.instance.BossActivator();
        StartCoroutine(WaitForBoss(collision));
    }
}

 IEnumerator WaitForBoss(Collider2D collision)
{
    var player = collision.GetComponent<PlayerController>();
    var playerInput = collision.GetComponent<UnityEngine.InputSystem.PlayerInput>();
    var rb = collision.GetComponent<Rigidbody2D>();

    playerInput.enabled = false;
    rb.velocity = Vector2.zero;
    bossGO.SetActive(true);

    yield return new WaitForSeconds(3f);
    playerInput.enabled = true;
    Destroy(gameObject);
}
}
