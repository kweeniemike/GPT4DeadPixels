using UnityEngine;
using System.Collections;
using System;

public class ConnectScript : MonoBehaviour 
{
    public string PlayerName = "Player";
    public string RoomName = "test_room";
    public Transform PlayerPrefab;
    public Transform AIPrefab;

    public Transform SpawnLoc1;
    public Transform SpawnLoc2;
    public Transform AISpawnLoc;

    public SpawnAI SpawnAI;

    private bool connectFailed = false;
    private bool readyToConnect = false;

    public void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("0.9");
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = this.PlayerName;
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }

    public void Update()
    {
        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connecting)
            {
                Debug.Log("Connecting to: " + PhotonNetwork.ServerAddress);
            }
            else
            {
                Debug.Log("Not connected. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
            }
            if (this.connectFailed)
            {
                Debug.Log("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                Debug.Log(String.Format("Server: {0}", new object[] { PhotonNetwork.ServerAddress }));
                Debug.Log("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);

                Debug.Log("Trying to reconnect..");

                this.connectFailed = false;
                PhotonNetwork.ConnectUsingSettings("0.9");
            }
            return;
        }

        if (!PhotonNetwork.inRoom && this.readyToConnect)
        {
            PhotonNetwork.JoinOrCreateRoom(this.RoomName, new RoomOptions() { maxPlayers = 10, isVisible = false }, TypedLobby.Default);
        }
    }

    private void SpawnPlayer()
    {
        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        if(PhotonNetwork.room.playerCount == 1 && this.SpawnLoc2 != null)
        {
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, this.SpawnLoc1.position, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, this.SpawnLoc2.position, Quaternion.identity, 0);
        }
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("Room: " + PhotonNetwork.room.name);
        this.SpawnPlayer();
        this.SpawnAI.Spawn();
    }

    public void OnPhotonCreateRoomFailed()
    {
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }
    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.networkingPeer.ServerAddress);
    }

    public void OnJoinedLobby()
    {
        this.readyToConnect = true;
    }
}
