using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchPuzzle : MonoBehaviour {
    public AudioClip finishedSound;
    public GameObject doorObject;
    public List<Torch> torches;
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
        if (!puzzleComplete)
        {
            foreach (Torch torch in torches)
            {
                if (!torch.powered)
                {
                    return;
                }
            }
            Solved();
        }
    }

    public void Solved()
    {
        this.photonView.RPC("SolvedRPC", PhotonTargets.All);
    }

    [RPC]
    public void SolvedRPC()
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
