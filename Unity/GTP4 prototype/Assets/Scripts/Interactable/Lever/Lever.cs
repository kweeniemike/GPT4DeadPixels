using UnityEngine;
using System.Collections;

public class Lever : Interactable
{
    public bool leverActivated;
    public GameObject leverUpObject;
    public GameObject leverDownObject;

    public override void OnInteract()
    {
        if (leverActivated)
        {
            leverUpObject.SetActive(true);
            leverDownObject.SetActive(false);
            leverActivated = false;
        }
        else
        {
            leverUpObject.SetActive(false);
            leverDownObject.SetActive(true);
            leverActivated = true;
        }
    }

}

