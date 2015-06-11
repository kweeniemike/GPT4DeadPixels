using UnityEngine;
using System.Collections;

public class SpawnAI : MonoBehaviour
{
    public Transform SpawnLocation;
    public Transform AIPrefab;

    public Transform[] MoveWaypoints;
    public Transform[] DeathWaypoints;

	public void Spawn()
    {
        AISphere AI = PhotonNetwork.Instantiate(this.AIPrefab.name, this.SpawnLocation.position, Quaternion.identity, 0).GetComponent<AISphere>();
        AI.SearchWayPoints = this.MoveWaypoints;
        AI.DeathPoints = this.DeathWaypoints;
    }
}
