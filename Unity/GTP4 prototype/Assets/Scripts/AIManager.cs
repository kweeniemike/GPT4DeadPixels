using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour 
{
    public static AIManager Instance;

    public Transform AIPrefab;
    public Transform[] AISpawnLocs;

    public Transform[] MoveWaypoints;
    public Transform[] DeathWaypoints;

    private List<AISphere> AIs = new List<AISphere>();
    private bool inited = false;

	public void Awake()
    {
        if (AIManager.Instance == null)
        {
            AIManager.Instance = this;
        }
    }

    public void Init()
    {
        this.inited = true;

        if (!PhotonNetwork.isMasterClient) this.enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.inited)
        {
            // Loop through all AIs and check if they are still alive, if not, remove them
            for (int i = 0; i < AIs.Count; i++)
            {
                if (!AIs[i].IsAlive)
                {
                    AIs[i].Network.dissapear();
                    AIs.Remove(AIs[i]);
                    i--;
                }
            }

            if (AIs.Count == 0)
            {
                this.SpawnAI();
            }
        }
	}

    public void SpawnAI()
    {
        AISphere AI = PhotonNetwork.Instantiate(this.AIPrefab.name, this.AISpawnLocs[Random.Range(0, this.AISpawnLocs.Length)].position, Quaternion.identity, 0).GetComponent<AISphere>();
        AI.SearchWayPoints = this.MoveWaypoints;
        AI.DeathPoints = this.DeathWaypoints;

        AIs.Add(AI);
    }
}
