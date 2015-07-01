using UnityEngine;
using System.Collections;

public class Torch : Interactable 
{
    public Material litMaterial;
    public Material unlitMaterial;

    public GameObject torchObject;
    public Light pointLight;
    public bool powered;

    new void Start()
    {
        base.Start();

        if (!powered)
        {
            this.torchObject.GetComponent<MeshRenderer>().material = unlitMaterial;
            this.pointLight.enabled = false;
        }
        else
        {
            this.torchObject.GetComponent<MeshRenderer>().material = litMaterial;
            this.pointLight.enabled = true;
        }
    }

    public override void OnInteract()
    {
        if (powered)
        {
            this.torchObject.GetComponent<MeshRenderer>().material = unlitMaterial;
            this.pointLight.enabled = false;
            powered = false;
        }
        else
        {
            this.torchObject.GetComponent<MeshRenderer>().material = litMaterial;
            this.pointLight.enabled = true;
            powered = true;
        }
    }
}
