using UnityEngine;
using System.Collections;

public class LeverPuzzle : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip finishedSound;

    public Lever lever1;


    private bool prevLever1 = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (lever1.leverActivated != prevLever1)
        //{
        //    AudioSource.PlayClipAtPoint(this.clickSound, this.transform.position);
        //}

        //if (lever1.leverActivated)
        //{
        //    AudioSource.PlayClipAtPoint(this.finishedSound, this.transform.position);
        //    this.gameObject.SetActive(false);
        //}
    }
}