using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public Image healthImg;
    public float maxHealth = 100f;

    private Damageable damageable;

    private void Start()
    {
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        if (damageable != null && healthImg != null)
        {
            healthImg.fillAmount = damageable.Health / maxHealth;
        }
    }

    private void OnDestroy()
    {
        if (BossUI.instance != null)
        {
            BossUI.instance.BossDeactivator();
        }
    }
}