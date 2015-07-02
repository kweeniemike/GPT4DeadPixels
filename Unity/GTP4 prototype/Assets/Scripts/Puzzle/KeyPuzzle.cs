using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyPuzzle : Interactable
{
    public AudioClip finishedSound;
    public GameObject doorObject;
    public List<GameObject> keyObjects;
    public List<GameObject> keyInDoorObjects;
    private bool puzzleComplete = false;
    private PhotonView photonView;
     
    // Use this for initialization
    void Start()
    {
        base.Start();
        this.photonView = base.getPhotonView();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void OnInteract()
    {
        if (!puzzleComplete)
        {
            bool notCompleted = false;
            foreach (GameObject keyObject in keyObjects)
            {
                if (!keyObject.activeSelf)
                {
                    keyInDoorObjects[keyObjects.IndexOf(keyObject)].SetActive(true);
                }
                else
                {
                    notCompleted = true;
                }
            }

            if (notCompleted)
            {
                return;
            }
            KeySolved();
        }
    }

    public void KeySolved()
    {
        this.photonView.RPC("KeySolvedRPC", PhotonTargets.All);
    }

    [RPC]
    public void KeySolvedRPC()
    {
        this.OnSolved();
    }

    public void OnSolved()
    {
        AudioSource.PlayClipAtPoint(this.finishedSound, this.transform.position);
        doorObject.SetActive(false);
        this.puzzleComplete = true;
    }
}

