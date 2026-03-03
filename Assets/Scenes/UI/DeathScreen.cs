using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [Header("Réglages UI")]
    public GameObject deathPanel;

    public void TriggerDeath()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}