using UnityEngine;

public class IInteractable : MonoBehaviour
{
    public bool isCollectible = false;
    public void Interact()
    {
        
        if(isCollectible)
        {
            GameManager.Manager.AddCollectible();
            Destroy(gameObject);
        }
    }
}
