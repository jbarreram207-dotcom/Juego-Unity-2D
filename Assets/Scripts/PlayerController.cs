using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpImpulse = 10f;
    public bool blockInput = false;

    public static PlayerController instance;

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    [Header("Dash")]
    [SerializeField] private float velocidadDash = 12f;
    [SerializeField] private float tiempoDash = 0.2f;
    [SerializeField] private float cooldownDash = 1.2f;
    [SerializeField] private TrailRenderer trailRenderer;

    private float gravedadInicial;
    private bool puedeHacerDash = true;
    private bool isDashing = false;

    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    Rigidbody2D rb;
    Animator animator;

    [Header("Wall")]
    private bool deslizando;
    private bool saltandoDePared;

    [SerializeField] private float velocidadDeslizar = 2f;
    [SerializeField] private float fuerzaSaltoParedX = 10f;
    [SerializeField] private float fuerzaSaltoParedY = 12f;
    [SerializeField] private float tiempoSaltoPared = 0.25f;

    [Header("Knockback")]
    public float knockbackMultiplier = 0.5f;

    [Header("Shield")]
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private float blockKnockbackReduction = 0.2f;

    private bool enShieldHit = false; // 🔥 FIX anti-spam

    private int comboCounter = 0;
    private bool enCooldownCombo = false;
    private float tiempoUltimoAtaque = 0f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !deslizando && !saltandoDePared && !isBlocking)
                    return IsRunning ? runSpeed : walkSpeed;
            }
            return 0;
        }
    }

    [SerializeField] private bool _isMoving;
    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField] private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        private set
        {
            if (_isFacingRight != value)
                transform.localScale *= new Vector2(-1, 1);

            _isFacingRight = value;
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    private int playerLayer;
    private int enemyLayer;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        damageable.damageableHit.AddListener(OnDamageTaken);

        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void Start()
    {
        jumpsRemaining = maxJumps;
        gravedadInicial = rb.gravityScale;
    }

    void Update()
    {
        // BLOQUEO
        if (Input.GetKey(KeyCode.C) && IsAlive && !isDashing)
        {
            isBlocking = true;
            animator.SetBool("isBlocking", true);
        }
        else
        {
            isBlocking = false;
            animator.SetBool("isBlocking", false);
        }

        // ATAQUE
        if (Input.GetKeyDown(KeyCode.X) && IsAlive && !isDashing && !enCooldownCombo && !isBlocking)
        {
            animator.SetTrigger("attack");
        }

        // SALTO
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && CanMove)
        {
            if (deslizando)
            {
                saltandoDePared = true;

                float direccion = IsFacingRight ? -1f : 1f;
                rb.velocity = new Vector2(direccion * fuerzaSaltoParedX, fuerzaSaltoParedY);

                animator.SetTrigger(AnimationStrings.jump);
                StartCoroutine(WallJumpLock());
            }
            else if (jumpsRemaining > 0)
            {
                animator.SetTrigger(AnimationStrings.jump);

                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.velocity += new Vector2(0, jumpImpulse);

                jumpsRemaining--;
            }
        }

        if (touchingDirections.IsGrounded)
            jumpsRemaining = maxJumps;

        // DASH
        if (Input.GetKeyDown(KeyCode.Z) && puedeHacerDash && !isBlocking)
            StartCoroutine(Dash());

        deslizando =
            !touchingDirections.IsGrounded &&
            touchingDirections.IsOnWall &&
            moveInput.x != 0 &&
            !saltandoDePared;

        animator.SetBool("Deslizando", deslizando);
    }

    private void FixedUpdate()
    {
        if (blockInput)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (isDashing || saltandoDePared) return;

        if (deslizando)
        {
            rb.velocity = new Vector2(0,
                Mathf.Clamp(rb.velocity.y, -velocidadDeslizar, float.MaxValue));

            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
            return;
        }

        if (!damageable.LockVelocity)
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        IsRunning = context.started;
        if (context.canceled) IsRunning = false;
    }

   private IEnumerator Dash()
{
    isDashing = true;
    puedeHacerDash = false;

    rb.gravityScale = 0;
    rb.velocity = Vector2.zero; // limpia cualquier movimiento previo

    float direccion = IsFacingRight ? 1f : -1f;

    animator.SetTrigger("Dash");

    Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

    float tiempo = 0f;

    while (tiempo < tiempoDash)
    {
        rb.velocity = new Vector2(velocidadDash * direccion, 0f); // 🔥 FORZADO
        tiempo += Time.deltaTime;
        yield return null;
    }

    Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

    rb.gravityScale = gravedadInicial;
    isDashing = false;

    yield return new WaitForSeconds(cooldownDash);
    puedeHacerDash = true;
}

    private IEnumerator WallJumpLock()
    {
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoDePared = false;
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight) IsFacingRight = true;
        else if (moveInput.x < 0 && IsFacingRight) IsFacingRight = false;
    }

    // 🔥 FIX REAL
    private void OnDamageTaken(int damage, Vector2 knockback)
    {
        if (isBlocking && !enShieldHit)
        {
            StartCoroutine(ShieldHitCooldown());

            animator.ResetTrigger("shieldHit");
            animator.SetTrigger("shieldHit");

            rb.velocity = new Vector2(
                knockback.x * knockbackMultiplier * blockKnockbackReduction,
                rb.velocity.y
            );

            damageable.Heal(damage);
        }
    }

    IEnumerator ShieldHitCooldown()
    {
        enShieldHit = true;
        yield return new WaitForSeconds(0.2f);
        enShieldHit = false;
    }

    // ✅ RESTAURADO
    public void Respawn(Vector3 posicionCheckpoint)
    {
        blockInput = false;

        transform.position = posicionCheckpoint;
        rb.velocity = Vector2.zero;

        damageable.ResetHealth();
        animator.SetBool(AnimationStrings.isAlive, true);
    }
} //original