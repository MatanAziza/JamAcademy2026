using UnityEngine;

public class PlayerTimeManager : MonoBehaviour
{
    [Header("Paramètres de Temps")]
    public float currentTime = 60f; // Le joueur commence avec 60 secondes (à équilibrer)
    public bool isDead = false;

    private Animator _animator;
    private CharacterController _controller;

    void Start()
    {
        // On récupère les composants du joueur
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isDead) return;

        // Le temps s'écoule naturellement (1 seconde par seconde)
        currentTime -= Time.deltaTime;

        // Si le temps tombe à zéro ou en dessous, c'est la mort
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            Die();
        }
    }

    // --- FONCTIONS APPELÉES PAR LES ENNEMIS ET LES ARMES ---

    // Quand l'ennemi nous touche
    public void TakeDamage(float timeLost = 10f)
    {
        if (isDead) return;
        
        Debug.Log("Le joueur a été touché ! -" + timeLost + " secondes !");
        ModifyTime(-timeLost);
    }

    // Quand le joueur veut tirer (Vérifie s'il a assez de temps)
    public bool TryUseWeapon(float timeCost)
    {
        if (isDead) return false;

        // Optionnel : Empêcher de tirer si le coût nous tue instantanément
        // if (currentTime - timeCost <= 0) return false; 

        ModifyTime(-timeCost);
        return true; // Le tir est autorisé
    }

    // Quand le joueur tue un ennemi
    public void AddTimeReward(float timeReward)
    {
        if (isDead) return;

        Debug.Log("Ennemi éliminé ! +" + timeReward + " secondes !");
        ModifyTime(timeReward);
    }

    // --- LOGIQUE INTERNE ---

    private void ModifyTime(float amount)
    {
        currentTime += amount;

        // On revérifie si une attaque ou un coût en temps nous a tué
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Temps écoulé ! GAME OVER.");

        // On lance l'animation de mort (Assure-toi d'avoir un "Trigger" nommé "Death" dans ton Animator)
        if (_animator != null)
        {
            _animator.SetTrigger("Death");
        }

        // On désactive le contrôle du joueur pour qu'il ne puisse plus bouger pendant qu'il meurt
        if (_controller != null)
        {
            _controller.enabled = false;
        }
        
        // Optionnel : désactiver le script de déplacement des Starter Assets
        // GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
    }
}