using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Manager;

    [Header("Collecitibles")]
    public int totalCollectibles = 8;
    public int collected = 0;

    [Header("UI")]
    public TextMeshProUGUI collectibleText;
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Enemy Control")]
    public GameObject enemy;
    public int unlockEnemyAt = 2;


    private void Awake() {
        Manager = this;
    }
    private void Start()
    {
        UpdateUI();
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        enemy.gameObject.SetActive(false); // LOCK MONSTER
    }

    public void AddCollectible()
    {
        collected++;
        UpdateUI();

        // Release monster after 2 items
        if(collected == unlockEnemyAt)
        {
            enemy.gameObject.SetActive(true);
        }

        // WIN CONDITION
        if(collected >= totalCollectibles)
        {
            WinGame();
        }
    }
    private void UpdateUI()
    {
        collectibleText.text = "Journals Found: " + collected + " / " + totalCollectibles;
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoseGame()
    {
        loseScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
