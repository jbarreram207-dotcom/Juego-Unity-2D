using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyAi : MonoBehaviour
{
    public float moveSpeed;
    public GameObject[] wayPoints;

    int nextWaypoints = 1;
    float distToPoint;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        distToPoint = Vector2.Distance(transform.position, wayPoints[nextWaypoints].transform.position);

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[nextWaypoints].transform.position,
                                moveSpeed * Time.deltaTime);

        if(distToPoint < 0.2f)
        {
            TakeTurn();
        }
    }

    void TakeTurn()
    {
        Vector3 currRot = transform.eulerAngles;
        currRot.z += wayPoints [nextWaypoints].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
        ChooseNextWayPoint();
    }

    void ChooseNextWayPoint()
    {
        nextWaypoints++;

        if(nextWaypoints == wayPoints.Length)
        {   
            nextWaypoints = 0;
        }
    }
}
