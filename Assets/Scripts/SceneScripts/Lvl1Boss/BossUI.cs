using System.Collections;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    public GameObject bossPanel;
    public GameObject muros;

    public static BossUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (bossPanel != null)
            bossPanel.SetActive(false);

        if (muros != null)
            muros.SetActive(false);
    }

    public void BossActivator()
    {
        if (bossPanel != null)
            bossPanel.SetActive(true);

        if (muros != null)
            muros.SetActive(true);
    }

    public void BossDeactivator()
    {
        if (bossPanel != null)
            bossPanel.SetActive(false);

        if (muros != null)
            muros.SetActive(false);

        StartCoroutine(BossDefeated());
    }

    IEnumerator BossDefeated()
    {
        if (PlayerController.instance == null)
            yield break;

        Rigidbody2D rb = PlayerController.instance.GetComponent<Rigidbody2D>();

        if (rb == null)
            yield break;

        Vector2 originalSpeed = rb.velocity;

        rb.velocity = Vector2.zero;

        PlayerController.instance.enabled = false;

        yield return new WaitForSeconds(3f);

        PlayerController.instance.enabled = true;
        rb.velocity = originalSpeed;
    }
}