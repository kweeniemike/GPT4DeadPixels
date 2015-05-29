using UnityEngine;
using System.Collections;

public class HallPuzzle : MonoBehaviour 
{
    public AudioClip clickSound;
    public AudioClip finishedSound;

    public Lever lever1;
    public Lever lever2;
    public Lever lever3;

    private bool prevLever1 = false;
    private bool prevLever2 = false; 
    private bool prevLever3 = false;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(lever1.leverActivated != prevLever1 ||
           lever2.leverActivated != prevLever2 ||
           lever3.leverActivated != prevLever3)
        {
            AudioSource.PlayClipAtPoint(this.clickSound, this.transform.position);
        }

        if (lever1.leverActivated && lever2.leverActivated && lever3.leverActivated)
        {
            AudioSource.PlayClipAtPoint(this.finishedSound, this.transform.position);
            this.gameObject.SetActive(false);
        }

        prevLever1 = lever1.leverActivated;
        prevLever2 = lever2.leverActivated;
        prevLever3 = lever3.leverActivated;
	}
}
