using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Text coinText;
    public int currentCoin = 0;
    public int maxHealth = 3;
    public Text health;
    public Animator animator;
    public Rigidbody2D rb;
    public float jumpForce = 5f;
    public bool isGround = true;

    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;
    public bool damage;
    public int empuje;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    void Start()
    {
    }

    void Update()
    {
        Damage();
        if (!damage)
        {
            if (maxHealth <= 0)
            {
                Die();
            }

            coinText.text = currentCoin.ToString();
            health.text = maxHealth.ToString();

            movement = Input.GetAxis("Horizontal");

            if (movement < 0f && facingRight)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingRight = false;
            }
            else if (movement > 0f && facingRight == false)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingRight = true;
            }

            if (Input.GetKey(KeyCode.Space) && isGround)
            {
                Jump();
                isGround = false;
                animator.SetBool("Jump", true);
            }

            if (Mathf.Abs(movement) > .1f)
            {
                animator.SetFloat("Run", 1f);
            }
            else if (movement < .1f)
            {
                animator.SetFloat("Run", 0f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    private void FixedUpdate()
    {
        if (!damage)
        {
            transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
        }
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    public void Attack()
    {
        // Detecta objetos en el área de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, attackLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Intenta llamar al método TakeDamage en cualquier script que tenga el enemigo
            enemy.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damageAmount)
    {
        if (maxHealth <= 0) return;
        maxHealth -= damageAmount;
    }

    public void Damage()
    {
        if (damage)
        {
            transform.Translate(Vector3.right * empuje * Time.deltaTime, Space.World);
        }
    }

    public void Finish_Damage()
    {
        damage = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            currentCoin++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Colllected");
            Destroy(other.gameObject, 1f);
        }
    }

    void Die()
    {
        Debug.Log("Player Died.");
        // Asegúrate de que GameManagement exista en tu escena para que esto no de error
        if (FindObjectOfType<GameManagement>() != null)
        {
            FindObjectOfType<GameManagement>().isGameActive = false;
        }
        Destroy(this.gameObject);
    }
}
//este es el original