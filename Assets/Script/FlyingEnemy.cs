using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed;
    public bool chase = false;
    public float stoppingDistance = 1.5f;
    public DetectionZone biteDetectionZone; 
    public Transform startingPoint;
    private GameObject player;
    Animator animator;
    Rigidbody2D rb;

public bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player=GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null)
         return;
        if(chase==true)
        Chase();
        else
        //go to starting position
            ReturnStartPoint();
        Flip();

        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

private void Chase()
{
    float distance = Vector2.Distance(transform.position, player.transform.position);

    if (distance > stoppingDistance)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            speed * Time.deltaTime
        );
    }
}

    private void ReturnStartPoint()
    {
        transform.position=Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        if(transform.position.x<player.transform.position.x)
        transform.rotation=Quaternion.Euler(0,0,0);
        else
        transform.rotation=Quaternion.Euler(0,180,0);
    }
    
}

    