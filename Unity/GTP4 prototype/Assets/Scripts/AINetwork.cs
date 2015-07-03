using UnityEngine;
using System.Collections;

public class AINetwork : Photon.MonoBehaviour  
{
    private AISphere AIScript;

    void Awake()
    {
        this.AIScript = GetComponentInChildren<AISphere>();

        if (photonView.isMine)
        {
            Debug.Log("Local AI");
            //MINE: local player, simply enable the local scripts
            this.AIScript.enabled = true;
        }
        else
        {
            Debug.Log("Remote AI");
            this.AIScript.enabled = false;
        }

        gameObject.name = gameObject.name + photonView.viewID;
    }

    public void dissapear()
    {
        this.photonView.RPC("DissapearRPC", PhotonTargets.All);
    }

    [RPC]
    void DissapearRPC()
    {
        GameObject.Destroy(this.gameObject);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
        }
    }
}
