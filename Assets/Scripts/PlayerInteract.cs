using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float interactDistance = 8f;
    [SerializeField] LayerMask interactLayer;
    
    [SerializeField] Image crosshair;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color hoverColor = Color.green;
    
    Camera cam;
    IInteractable currentTarget;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        crosshair.color = normalColor;
    }

   void Update()
    {
        CheckHover();

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    void CheckHover()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentTarget = interactable;
                crosshair.color = hoverColor;
                return;
            }
        }

        // If we reach here, nothing interactable was hit
        currentTarget = null;
        crosshair.color = normalColor;
    }
}