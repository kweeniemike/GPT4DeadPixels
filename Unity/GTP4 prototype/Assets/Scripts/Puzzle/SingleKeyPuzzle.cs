using UnityEngine;
using System.Collections;

public class SingleKeyPuzzle : Interactable
{
    private bool keyActivated = false;
    public GameObject doorObject;
    public AudioClip finishedSound;


    public override void OnInteract()
    {
        if (!keyActivated) 
        {
            AudioSource.PlayClipAtPoint(this.finishedSound, this.transform.position);
            doorObject.SetActive(false);
            this.gameObject.SetActive(false);
            keyActivated = true;
        }
    }

}


