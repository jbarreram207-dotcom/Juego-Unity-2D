using UnityEngine;

public class JefeIdleBehavior : StateMachineBehaviour
{
    [SerializeField] private float tiempoDecision = 0.3f; // 🔥 MÁS RÁPIDO (antes era más lento)

    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= tiempoDecision)
        {
            animator.SetInteger("numeroAleatorio", Random.Range(0, 2)); // mismo sistema que ya usas
            timer = 0f;
        }
    }
}