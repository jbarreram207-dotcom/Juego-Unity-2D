using UnityEngine;

public class JefeCaminarBehavior : StateMachineBehaviour
{
    private Guardian jefe;
    private Rigidbody2D rb2D;

    [SerializeField] private float velocidadMovimiento = 2f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        jefe = animator.GetComponent<Guardian>();
        rb2D = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (jefe == null || jefe.Player == null) return;

        // 🔥 NO SE MUEVE SI NO PUEDE
        if (!jefe.CanMove)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            return;
        }

        // Dirección hacia el jugador (-1 o 1)
        float direccion = Mathf.Sign(jefe.Player.position.x - animator.transform.position.x);

        // Movimiento
        rb2D.velocity = new Vector2(direccion * velocidadMovimiento, rb2D.velocity.y);

        // 🔄 Flip correcto (tu sprite mira a la izquierda por default)
        if (direccion > 0)
            animator.transform.localScale = new Vector3(-1, 1, 1);
        else
            animator.transform.localScale = new Vector3(1, 1, 1);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 🔥 detenerse al salir del estado
        if (rb2D != null)
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }
}