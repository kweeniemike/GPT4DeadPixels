using UnityEngine;
using System.Collections;

public class Key : Interactable {
    public override void OnInteract()
    {
        this.gameObject.SetActive(false);
    }
}
