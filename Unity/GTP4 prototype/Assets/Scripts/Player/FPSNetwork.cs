using UnityEngine;
using System.Collections;

public class FPSNetwork : Photon.MonoBehaviour 
{
    private GameObject model;
    private GameObject cameraRig;
    private AudioListener listener;
    private Light pointLight;
    private FPSController fpsController;

    void Awake()
    {
        this.FindCameraRig();
        this.FindModel();

        this.listener = GetComponentInChildren<AudioListener>();
        this.pointLight = GetComponentInChildren<Light>();
        this.fpsController = GetComponent<FPSController>();

        if (photonView.isMine)
        {
            Debug.Log("Mine");

            //MINE: local player, simply enable the local scripts
            this.model.SetActive(false);
            this.cameraRig.SetActive(true);
            this.listener.enabled = true;
            this.pointLight.intensity = 0.2f;
            this.pointLight.flare = null;
            this.fpsController.enabled = true;
        }
        else
        {
            Debug.Log("Others");

            this.cameraRig.SetActive(false);
            this.model.SetActive(true);
            this.listener.enabled = false;

            this.pointLight.intensity = 0.5f;
            this.fpsController.enabled = true;
            this.fpsController.isControllable = false;
        }

        gameObject.name = gameObject.name + photonView.viewID;
    }

    private void FindCameraRig()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "CameraRig")
            {
                this.cameraRig = child.gameObject;
                break;
            }
        }
    }

    private void FindModel()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Model")
            {
                this.model = child.gameObject;
                break;
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	// Update is called once per frame
	void Update () 
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
	}
}
