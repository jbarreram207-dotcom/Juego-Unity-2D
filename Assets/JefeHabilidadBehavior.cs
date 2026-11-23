using UnityEngine;

public class JefeHabilidadBehavior : StateMachineBehaviour
{
    [SerializeField] private GameObject habilidad;

    private Guardian guardian;
    private Transform player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        guardian = animator.GetComponent<Guardian>();

        if (guardian == null) return;

        player = guardian.Player;

        if (player == null) return;

        // 🔥 GIRAR HACIA EL JUGADOR
        float direccion = Mathf.Sign(player.position.x - animator.transform.position.x);

        if (direccion > 0)
            animator.transform.localScale = new Vector3(-1, 1, 1);
        else
            animator.transform.localScale = new Vector3(1, 1, 1);

        // 🔥 INSTANCIAR BOMBA
        GameObject bomba = Instantiate(
            habilidad,
            guardian.transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = bomba.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 dir = player.position - guardian.transform.position;

            float fuerzaX = dir.x * 1.5f; // 🔥 AJUSTA
            float fuerzaY = 8f;           // 🔥 ALTURA DEL ARCO

            rb.velocity = new Vector2(fuerzaX, fuerzaY);
        }
    }
}