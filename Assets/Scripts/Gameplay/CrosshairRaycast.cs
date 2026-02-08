using UnityEngine;
using TMPro;

public class CrosshairRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float raycastDistance = 5f;
    public LayerMask interactableLayer;
    
    [Header("UI References")]
    public TextMeshProUGUI interactionText;
    
    private Camera playerCamera;
    private InteractableObject currentInteractable;
    private bool isLookingAtInteractable = false;
    
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Force hide text at start
        HideInteractionPrompt();
    }
    
    void Update()
    {
        CheckForInteractable();
        CheckForInteractionInput();
    }
    
    void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Check if raycast hits something on the interactable layer
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null)
            {
                // We are looking at an interactable object
                currentInteractable = interactable;
                isLookingAtInteractable = true;
                ShowInteractionPrompt(interactable.interactionPrompt);
                return;
            }
        }
        
        // Not looking at anything interactable
        currentInteractable = null;
        isLookingAtInteractable = false;
        HideInteractionPrompt();
    }
    
    void CheckForInteractionInput()
    {
        // Only allow interaction if we're looking at an interactable
        if (isLookingAtInteractable && currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.OnInteract();
        }
    }
    
    void ShowInteractionPrompt(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.gameObject.SetActive(true);
        }
    }
    
    void HideInteractionPrompt()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}