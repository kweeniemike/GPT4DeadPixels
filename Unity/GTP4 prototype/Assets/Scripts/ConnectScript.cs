using UnityEngine;
using System.Collections;
using System;

public class ConnectScript : MonoBehaviour 
{
    public string PlayerName = "Player";
    public int CurrentLevelId = 0;
    public int CurrentSubLevelId = 0;
    public string PlayerType = "A";

    public Transform PlayerPrefab;

    public Transform[] SpawnLocs;

    private bool connectFailed = false;

    private int nextSpawn = 0;

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

        if (!PhotonNetwork.inRoom && PhotonNetwork.connectedAndReady)
        {
            this.JoinRoom();
        }
    }

    private void JoinRoom()
    {
        Debug.Log("trying to join a room");
        // Try to join a game with the given custom room properties.
        PhotonNetwork.JoinRandomRoom();
    }

    private void SpawnPlayer()
    {
        // Spawns a player at a random spawn location
        PhotonNetwork.Instantiate(this.PlayerPrefab.name, this.SpawnLocs[nextSpawn].position, Quaternion.identity, 0);
        nextSpawn++;
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("Room: " + PhotonNetwork.room.name);
        this.SpawnPlayer();

        AIManager.Instance.Init();
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
        Debug.Log("Failed to join");

        // If the join fails, create a new room with the custom properties
        //ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "LevelId", this.CurrentLevelId }, { "SubLevelId", this.CurrentSubLevelId } };
        PhotonNetwork.CreateRoom("Room" + UnityEngine.Random.Range(0, 9999), new RoomOptions() { maxPlayers = 2, isOpen = true, isVisible = true }, TypedLobby.Default);
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

    }
}
