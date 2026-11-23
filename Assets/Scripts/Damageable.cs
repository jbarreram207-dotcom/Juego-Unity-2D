using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;

    public Image healthImg;
    private Animator animator;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;

    public int Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health <= 0)
            {
                animator.SetBool("isAlive", false);
            }
        }
    }

    [SerializeField] private float invincibilityTime = 0.25f;

    private bool isInvincible = false;
    private float timeSinceHit = 0f;

    public bool IsAlive => animator.GetBool("isAlive");

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        if (animator != null)
            animator.SetBool("isAlive", true);
    }

    private void Update()
    {
        // 🔹 SOLO actualiza la barra si existe (jugador)
        if (healthImg != null)
        {
            healthImg.fillAmount = (float)health / maxHealth;
        }

        // 🔹 Manejo de invencibilidad
        if (isInvincible)
        {
            if (timeSinceHit >= invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            health -= damage;
            isInvincible = true;

            if (animator != null)
            {
                animator.SetTrigger(AnimationStrings.hit);
                LockVelocity = true;
            }

            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            if (health <= 0)
            {
                if (animator != null)
                    animator.SetBool("isAlive", false);

                if (GameManagement.Instance != null)
                {
                    PlayerController player = GetComponent<PlayerController>();

                    if (player != null)
                    {
                        GameManagement.Instance.RespawnJugador();
                    }
                }

                damageableDeath?.Invoke();
            }

            return true;
        }

        return false;
    }

    public void ResetHealth()
    {
        health = maxHealth;

        if (animator != null)
            animator.SetBool("isAlive", true);
    }

    public bool Heal(int healthRestore)
    {
        if (IsAlive && health < maxHealth)
        {
            int maxHeal = Mathf.Max(maxHealth - health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            health += actualHeal;

            CharacterEvents.characterHealed(gameObject, actualHeal);
            return true;
        }
        return false;
    }
} //original sin barra de vida de boss