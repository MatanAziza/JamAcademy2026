using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerTimeManager : MonoBehaviour
{
    [Header("État du Jeu")]
    public bool gameStarted = false; 
    public bool isDead = false;

    [Header("Paramètres de Temps")]
    public float currentTime = 120f;
    public Text timerText; 

    [Header("Effet de Danger (Rouge)")]
    public Image dangerVignette;
    public float dangerTimeThreshold = 10f;
    public float pulseSpeed = 5f;
    public float maxAlpha = 0.5f;

    [Header("Menus")]
    public GameObject pauseMenuPanel;
    public GameObject helperMessagePanel;

    private Animator _animator;

    void Awake()
    {
        if (transform.parent == null) DontDestroyOnLoad(gameObject);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnLevelFinishedLoading; }
    void OnDisable() { SceneManager.sceneLoaded -= OnLevelFinishedLoading; }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GameObject canvas = GameObject.Find("GameCanvas");
        if (canvas != null)
        {
            Transform v = canvas.transform.Find("DangerVignette");
            if (v != null) dangerVignette = v.GetComponent<Image>();

            Transform t = canvas.transform.Find("TimerText");
            if (t != null) timerText = t.GetComponent<Text>();

            Transform p = canvas.transform.Find("PauseMenuPanel");
            if (p != null) pauseMenuPanel = p.gameObject;

            Transform h = canvas.transform.Find("HelperMessagePanel");
            if (h != null) helperMessagePanel = h.gameObject;
        }
        if (dangerVignette != null) dangerVignette.color = new Color(1, 0, 0, 0);
    }

    void Start() { _animator = GetComponent<Animator>(); }

    void Update()
    {
        if (!gameStarted || isDead) return;

        if (Time.timeScale > 0) currentTime -= Time.deltaTime;

        if (timerText != null) timerText.text = Mathf.Ceil(currentTime).ToString() + "s";

        if (currentTime <= 0) { currentTime = 0; Die(); }

        HandleRedEffect();
    }

    // --- RÉPARATION DES FONCTIONS DE COMBAT ---

    public void AddTimeReward(float amount) 
    { 
        if (!isDead) currentTime += amount; 
    }

    // Cette fonction doit accepter un argument et renvoyer un booléen
    public bool TryUseWeapon(float cost = 0f) 
    {
        if (isDead || !gameStarted) return false;

        // Si tu veux que tirer coûte du temps, on le soustrait ici
        if (currentTime > cost)
        {
            currentTime -= cost;
            return true; // Autorise le tir
        }

        return false; // Pas assez de temps pour tirer
    }

    public void TakeDamage(float amount) { if(!isDead) currentTime -= amount; }

    // ------------------------------------------

    void HandleRedEffect()
    {
        if (dangerVignette == null) return;
        bool isMenuOpen = (pauseMenuPanel != null && pauseMenuPanel.activeInHierarchy) || 
                          (helperMessagePanel != null && helperMessagePanel.activeInHierarchy);

        if (currentTime <= dangerTimeThreshold && !isMenuOpen && Time.timeScale > 0)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f;
            dangerVignette.color = new Color(1f, 0f, 0f, pulse * maxAlpha);
        }
        else dangerVignette.color = new Color(1f, 0f, 0f, 0f);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        DeathManager dm = Object.FindFirstObjectByType<DeathManager>();
        if (dm != null) dm.TriggerDeath();
        if (_animator != null) _animator.SetTrigger("Death");
    }
}