using UnityEngine;
using System.Collections;

public class AISphere : MonoBehaviour 
{
    public float WayPointDistance = 3.0f;
    public float VisionRadius = 10.0f;
    public float WalkAroundPlayerRadius = 5.0f;
    public float InnerLeadDistance = 7.5f;
    public float OuterLeadDistance = 15.0f;

    public Transform[] DeathPoints;
    public Transform[] SearchWayPoints;

    private GameObject targetPlayer = null;
    private AIState state;
    private AISubState subState;

    private Transform targetSearchWayPoint = null;
    private Transform targetDeathPoint = null;
    private Vector3 targetWalkPos = Vector3.zero;

    private NavMeshAgent navAgent;


	// Use this for initialization
	void Start () 
    {
        this.state = AIState.Searching;
        this.subState = AISubState.Walking;
        this.navAgent = this.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (this.state == AIState.Searching)
        {
            if (this.targetSearchWayPoint == null)
            {
                this.PickRandomSearchWaypoint(null);
            }
            else if (Vector3.Distance(this.transform.position, this.targetSearchWayPoint.position) < WayPointDistance)
            {
                this.PickRandomSearchWaypoint(this.targetSearchWayPoint);
            }

            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, this.VisionRadius);
            foreach(Collider col in hitColliders)
            {
                if (col.tag == "Player")
                {
                    this.targetPlayer = col.gameObject;
                    this.state = AIState.GoingToPlayer;
                    break;
                }
            }
        }
        else if (this.state == AIState.GoingToPlayer)
        {
            this.navAgent.SetDestination(this.targetPlayer.transform.position);
            if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) < this.WayPointDistance)
            {
                this.state = AIState.WalkingAround;
                this.subState = AISubState.ChoosingTarget;
                this.StartCoroutine(this.WalkAroundFor(Random.Range(5.0f, 21.0f)));
            }
        }
        else if (this.state == AIState.WalkingAround)
        {
            if (this.subState == AISubState.ChoosingTarget)
            {
                this.targetWalkPos = GenerateRandomPosAroundPlayer(this.targetPlayer.transform.position);
                this.navAgent.SetDestination(this.targetWalkPos);
                this.subState = AISubState.Walking;

                Debug.Log("Find place to walk");
            }
            else if (this.subState == AISubState.Walking)
            {
                if (Vector3.Distance(this.transform.position, this.targetWalkPos) < this.WayPointDistance)
                {
                    this.subState = AISubState.Waiting;
                    this.StartCoroutine(GoToNextSubStateIn(Random.Range(1.5f, 5.0f), AISubState.ChoosingTarget));
                }
            }
        }
        else if (this.state == AIState.LeadingAway)
        {
            if (this.subState == AISubState.Walking)
            {
                if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) > this.OuterLeadDistance)
                {
                    this.navAgent.SetDestination(this.transform.position);
                    this.StartCoroutine(GoToNextSubStateIn(Random.Range(1.5f, 5.0f), AISubState.Returning));           
                }

                if (Vector3.Distance(this.transform.position, this.targetDeathPoint.position) < this.WayPointDistance)
                {
                    Debug.Log("End reached.");

                    // TODO: Dissapear
                }
            }
            else if (this.subState == AISubState.Returning)
            {
                this.navAgent.SetDestination(this.targetPlayer.transform.position);

                if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) < this.WayPointDistance)
                {
                    this.state = AIState.WalkingAround;
                    this.subState = AISubState.ChoosingTarget;
                    this.StartCoroutine(this.WalkAroundFor(Random.Range(5.0f, 21.0f)));
                }
            }
        }
	}

    IEnumerator WalkAroundFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        this.FindDeathPoint();
    }

    private void FindDeathPoint()
    {
        Debug.Log("Find death point");

        this.state = AIState.LeadingAway;
        this.subState = AISubState.Walking;

        Transform closestDeahPoint = null;
        foreach (Transform deathPoint in this.DeathPoints)
        {
            if (closestDeahPoint == null) closestDeahPoint = deathPoint;
            else if (Vector3.Distance(this.transform.position, deathPoint.position) <
                     Vector3.Distance(this.transform.position, closestDeahPoint.position))
            {
                closestDeahPoint = deathPoint;
            }
        }

        this.targetDeathPoint = closestDeahPoint;
        this.navAgent.SetDestination(closestDeahPoint.position);
    }

    IEnumerator GoToNextSubStateIn(float seconds, AISubState nextState)
    {
        yield return new WaitForSeconds(seconds);
        this.subState = nextState;
    }

    private Vector3 GenerateRandomPosAroundPlayer(Vector3 playerPos)
    {
        Vector3 walkToPos = Vector3.zero;
        do
        {
            Vector2 randomPos = Random.insideUnitCircle * 5.0f;
            walkToPos = new Vector3(randomPos.x, 0, randomPos.y) + playerPos;
        }
        while (!Physics.Raycast(this.transform.position, walkToPos - this.transform.position, Vector3.Distance(this.transform.position, walkToPos)));
        
        return walkToPos;
    }

    private void PickRandomSearchWaypoint(Transform curSearchWaypoint)
    {
        Transform newTarget = null;
        while(newTarget == null || newTarget == curSearchWaypoint)
        {
            newTarget = this.SearchWayPoints[Random.Range(0, this.SearchWayPoints.Length)];
        }

        this.targetSearchWayPoint = newTarget;
        this.navAgent.SetDestination(this.targetSearchWayPoint.position);
    }
}

public enum AIState
{
    Searching, GoingToPlayer, WalkingAround, LeadingAway, 
}

public enum AISubState
{
    Waiting, Walking, ChoosingTarget, Returning
}
