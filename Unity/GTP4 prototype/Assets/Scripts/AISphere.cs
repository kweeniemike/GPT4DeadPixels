﻿using UnityEngine;
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
    public AINetwork Network;

    public bool IsAlive { get; private set; }

    private RoomData currentRoom;

    private GameObject targetPlayer = null;
    private AIState state;
    private AISubState subState;

    private Transform targetSearchWayPoint = null;
    private Transform targetDeathPoint = null;
    private Vector3 targetWalkPos = Vector3.zero;
    private Interactable targetInteractable = null;

    private NavMeshAgent navAgent;

    private int leadAwayAttempts = 0;

	// Use this for initialization
	void Start () 
    {
        this.state = AIState.Searching;
        this.subState = AISubState.Walking;
        this.navAgent = this.GetComponent<NavMeshAgent>();
        this.IsAlive = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (this.state == AIState.Searching)
        {
            this.HandleSearching();
        }
        else if (this.state == AIState.GoingToPlayer)
        {
            this.HandleGoingTo();
        }
        else if (this.state == AIState.WalkingAround)
        {
            this.HandleWalkingAround();
        }
        else if (this.state == AIState.LeadingAway)
        {
            this.HandleLeadingAway();
        }
	}

    private void HandleSearching()
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
        foreach (Collider col in hitColliders)
        {
            if (col.tag == "Player")
            {
                this.targetPlayer = col.gameObject;
                this.state = AIState.GoingToPlayer;
                break;
            }
        }
    }

    private void HandleGoingTo()
    {
        this.navAgent.SetDestination(this.targetPlayer.transform.position);
        if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) < this.WayPointDistance)
        {
            this.state = AIState.WalkingAround;
            this.subState = AISubState.ChoosingTarget;
            this.StartCoroutine(this.WalkAroundFor(Random.Range(5.0f, 21.0f)));
        }
    }

    private void HandleWalkingAround()
    {
        if (this.subState == AISubState.ChoosingTarget)
        {
            // Choose whether or not we're going to interact with something
            if (this.currentRoom.interactables != null && this.currentRoom.interactables.Length != 0 && Random.Range(0, 10) >= 3)
            {
                // Interact with something
                // Pick something to interact with
                this.targetInteractable = this.currentRoom.interactables[Random.Range(0, this.currentRoom.interactables.Length)];
                // Set out target walk position to the position of the interactable
                this.targetWalkPos = this.targetInteractable.transform.position;
            }
            else
            {
                // Just walk around the player
                this.targetWalkPos = GenerateRandomPosAroundPlayer(this.targetPlayer.transform.position);
                this.targetInteractable = null;

                Debug.Log("Find place to walk");
            }

            // Move to target location
            this.navAgent.SetDestination(this.targetWalkPos);
            this.subState = AISubState.Walking;
        }
        else if (this.subState == AISubState.Walking)
        {
            if (Vector3.Distance(this.transform.position, this.targetWalkPos) < this.WayPointDistance)
            {
                // Were we going to interact with something?
                if (this.targetInteractable != null)
                {
                    this.targetInteractable.Interact();
                }

                // Wait a little bit before we do anything else
                float waitTime = this.targetInteractable == null ? Random.Range(1.5f, 5.0f) : Random.Range(0.25f, 1.0f);

                this.subState = AISubState.Waiting;
                this.navAgent.SetDestination(this.transform.position);
                this.StartCoroutine(GoToNextSubStateIn(waitTime, AISubState.ChoosingTarget, AIState.WalkingAround));
            }
        }
    }

    private void HandleLeadingAway()
    {
        if (this.subState == AISubState.Walking)
        {
            if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) > this.OuterLeadDistance)
            {
                this.leadAwayAttempts++;
                this.navAgent.SetDestination(this.transform.position);
                this.StartCoroutine(GoToNextSubStateIn(Random.Range(1.5f, 5.0f), AISubState.Returning, AIState.LeadingAway));

                this.subState = AISubState.Waiting;
            }

            if (Vector3.Distance(this.transform.position, this.targetDeathPoint.position) < this.WayPointDistance)
            {
                this.IsAlive = false;
            }
        }
        else if (this.subState == AISubState.Returning)
        {
            this.navAgent.SetDestination(this.targetPlayer.transform.position);

            if (Vector3.Distance(this.transform.position, this.targetPlayer.transform.position) < this.WayPointDistance)
            {
                if (this.leadAwayAttempts < 2)
                {
                    this.FindDeathPoint();
                }
                else
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

        this.leadAwayAttempts = 0;
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

    IEnumerator GoToNextSubStateIn(float seconds, AISubState nextState, AIState startState)
    {
        yield return new WaitForSeconds(seconds);

        if (this.state == startState)
        {
            this.subState = nextState;
        }
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

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);

        if (other.tag == "Room")
        {
            this.currentRoom = other.GetComponent<RoomData>();
        }
        else if (other.tag == "Interactable")
        {
            Door door = other.GetComponent<Door>();
            if (door != null)
            {
                if (!door.IsOpen)
                {
                    door.Interact();
                }
            }
        }
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
