using UnityEngine;
using System.Collections;

public class Torch : Interactable 
{
    public Material litMaterial;
    public Material unlitMaterial;

    public GameObject torchObject;
    public Light light;
    public bool powered;

    public override void OnInteract()
    {
        if (powered)
        {
            this.torchObject.GetComponent<MeshRenderer>().material = unlitMaterial;
            this.light.enabled = false;
            powered = false;
        }
        else
        {
            this.torchObject.GetComponent<MeshRenderer>().material = litMaterial;
            this.light.enabled = true;
            powered = true;
        }
    }
}
