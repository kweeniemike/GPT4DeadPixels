using UnityEngine;
using System.Collections;

public class Door : Interactable 
{
    public GameObject doorObject;

    public override void OnInteract()
    {
        if(this.doorObject !=  null)
        {
            if(this.doorObject.activeSelf)
            {
                this.doorObject.SetActive(false);
            }
            else
            {
                this.doorObject.SetActive(true);
            }
        }
    }
}
