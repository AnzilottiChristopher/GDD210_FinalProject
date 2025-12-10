using UnityEngine;
using UnityEngine.SceneManagement;

public class OutOfMapKillboxScript : MonoBehaviour
{
    // Reset the currently active scene
    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 3D trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            ResetScene();
    }

    // 3D collision (non-trigger)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
            ResetScene();
    }

    // 2D trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
            ResetScene();
    }

    // 2D collision (non-trigger)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
            ResetScene();
    }
}
