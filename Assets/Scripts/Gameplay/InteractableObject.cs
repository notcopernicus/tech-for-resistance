using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string interactionPrompt = "Press E to interact";
    
    public virtual void OnInteract()
    {
        Debug.Log("Player interacted with " + gameObject.name);
        // Add your interaction code here later
    }
}