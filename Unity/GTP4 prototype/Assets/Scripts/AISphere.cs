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

    private Transform targetSearchWayPoint = null;
    private Transform targetDeathPoint = null;

    private NavMeshAgent navAgent;

	// Use this for initialization
	void Start () 
    {
        this.state = AIState.Searching;
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
            }
        }
        else if (this.state == AIState.WalkingAround)
        {
            // TODO: Randomly walk around for a little while

            Debug.Log("Find death point");

            this.state = AIState.LeadingAway;

            Transform closestDeahPoint = null;
            foreach(Transform deathPoint in this.DeathPoints)
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
        else if (this.state == AIState.LeadingAway)
        {
            if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) > this.OuterLeadDistance)
            {
                this.state = AIState.GoingToPlayer;
                this.navAgent.SetDestination(this.targetPlayer.transform.position);
            }

            if (Vector3.Distance(this.transform.position, this.targetDeathPoint.position) < this.WayPointDistance)
            {
                Debug.Log("End reached.");
            }
        }
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
    Searching, GoingToPlayer, WalkingAround, LeadingAway
}
