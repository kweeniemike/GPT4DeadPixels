using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour 
{
    private PhotonView photonView;

	// Use this for initialization
	public void Start () 
    {
        this.photonView = this.GetComponent<PhotonView>();
	}

    public PhotonView getPhotonView()
    {
        return photonView;
    }
	
	public void Interact()
    {
        this.photonView.RPC("InteractRPC", PhotonTargets.All);
    }

    [RPC]
    public void InteractRPC()
    {
        this.OnInteract();
    }

    public abstract void OnInteract();

    public virtual void OnLookAt() { }
}
