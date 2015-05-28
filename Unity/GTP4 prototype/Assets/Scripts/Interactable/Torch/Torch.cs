using UnityEngine;
using System.Collections;

public class Torch : Interactable 
{
    public Material litMaterial;
    public Material unlitMaterial;

    public GameObject torchObject;
    public Light light;

    public override void OnInteract()
    {
        if (this.light.enabled)
        {
            this.torchObject.GetComponent<MeshRenderer>().material = unlitMaterial;
            this.light.enabled = false;
        }
        else
        {
            this.torchObject.GetComponent<MeshRenderer>().material = litMaterial;
            this.light.enabled = true;
        }
    }
}
