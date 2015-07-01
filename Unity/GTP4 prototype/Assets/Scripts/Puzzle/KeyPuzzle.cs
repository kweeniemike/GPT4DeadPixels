using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyPuzzle : Interactable
{
    public AudioClip finishedSound;
    public GameObject doorObject;
    public List<Key> keys;
    public List<GameObject> keyObjects;
    private bool puzzleComplete = false;
    private PhotonView photonView;

    // Use this for initialization
    void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public override void OnInteract()
    {
        if (!puzzleComplete)
        {
            foreach (Key key in keys)
            {
                if (key.gameObject.activeSelf)
                {
                    InputKey();
                    return;
                }
            }
            KeySolved();
        }
    }

    public void InputKey()
    {
        foreach (GameObject keyObject in keyObjects)
        {
            if (!keyObject.activeSelf)
            {
                //wil hier kijken of een object al disabled is, dus een key is opgepakt. Op dat moment slechts 1 key per disabled object enablen op de deur. Maybe counteR?
            }
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

