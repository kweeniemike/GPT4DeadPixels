using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour 
{
    public Transform AIPrefab;
    public Transform[] AISpawnLocs;

    public Transform[] MoveWaypoints;
    public Transform[] DeathWaypoints;

    private List<AISphere> AIs = new List<AISphere>();

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void SpawnAI()
    {
        AISphere AI = PhotonNetwork.Instantiate(this.AIPrefab.name, this.AISpawnLocs[Random.Range(0, this.AISpawnLocs.Length)].position, Quaternion.identity, 0).GetComponent<AISphere>();
        AI.SearchWayPoints = this.MoveWaypoints;
        AI.DeathPoints = this.DeathWaypoints;

        AIs.Add(AI);
    }
}
