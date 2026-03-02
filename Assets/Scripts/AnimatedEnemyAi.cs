using UnityEngine;
using UnityEngine.AI; // NOUVEAU : On importe l'intelligence artificielle !

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))] // Ajoute automatiquement l'agent s'il manque
public class AnimatedEnemyAI : MonoBehaviour
{
    [Header("Cible & IA")]
    public Transform targetPlayer;
    
    [Header("Portée des attaques")]
    public float normalAttackDistance = 1.5f; 
    public float jumpAttackDistance = 4.0f;   

    [Header("Paramètres d'attaque")]
    public float attackCooldown = 2.5f; 
    public float normalAttackDuration = 1.0f; 
    public float jumpAttackDuration = 1.5f;   
    
    private float nextAttackTime = 0f;
    private float attackEndTime = 0f; 
    private bool _hasRolledForJump = false; 

    [Header("Déplacement")]
    public float MoveSpeed = 2.0f;
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;

    [Header("Gravité & Sol")]
    public float Gravity = -15.0f;
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    // Variables internes
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private Animator _animator;
    private CharacterController _controller;
    private NavMeshAgent _agent; // NOUVEAU : Le cerveau IA
    private bool _hasAnimator;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();

        // IMPORTANT : On dit à l'Agent de calculer le chemin, mais on garde le contrôle du mouvement pour nos animations !
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        AssignAnimationIDs();
    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        ApplyGravity();
        GroundedCheck();
        EnemyLogicAndMovement();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        if (_hasAnimator) _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void EnemyLogicAndMovement()
    {
        if (targetPlayer == null) return;

        PlayerTimeManager playerTime = targetPlayer.GetComponent<PlayerTimeManager>();
        
        if (playerTime != null && playerTime.isDead)
        {
            _speed = 0f;
            _animationBlend = 0f;
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, 0f);
                _animator.SetFloat(_animIDMotionSpeed, 0f);
            }
            return; 
        }

        Vector3 playerPosPlane = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
        
        // --- LA MAGIE DU NAVMESH EST ICI ---
        // On synchronise la position de l'esprit (l'agent) avec celle du corps (le perso)
        _agent.nextPosition = transform.position; 
        // On lui demande de trouver un chemin vers le joueur
        _agent.SetDestination(playerPosPlane);

        // AU LIEU d'une ligne droite, on regarde la "distance du chemin à parcourir" (qui prend les murs en compte !)
        float distanceToPlayer = _agent.pathPending ? Vector3.Distance(transform.position, playerPosPlane) : _agent.remainingDistance;

        float targetSpeed = 0f;
        Vector3 directionToPlayer = Vector3.zero;

        bool isAttacking = Time.time < attackEndTime;

        if (isAttacking)
        {
            targetSpeed = 0f;
        }
        else
        {
            if (distanceToPlayer > jumpAttackDistance)
            {
                targetSpeed = MoveSpeed;
                directionToPlayer = _agent.desiredVelocity.normalized; // Il suit le chemin calculé !
                _hasRolledForJump = false;
            }
            else if (distanceToPlayer <= jumpAttackDistance && distanceToPlayer > normalAttackDistance)
            {
                targetSpeed = MoveSpeed;
                directionToPlayer = _agent.desiredVelocity.normalized;

                if (Time.time >= nextAttackTime && !_hasRolledForJump)
                {
                    _hasRolledForJump = true;
                    int roll = Random.Range(0, 2); 

                    if (roll == 1)
                    {
                        Debug.Log("L'ennemi fait une ATTAQUE SAUTÉE !");
                        targetSpeed = 0f;
                        attackEndTime = Time.time + jumpAttackDuration;
                        nextAttackTime = attackEndTime + attackCooldown;
                        if (playerTime != null) playerTime.TakeDamage(10f);
                    }
                }
            }
            else if (distanceToPlayer <= normalAttackDistance)
            {
                targetSpeed = 0f;
                // S'il est au corps à corps, il te regarde directement
                directionToPlayer = (playerPosPlane - transform.position).normalized;

                if (Time.time >= nextAttackTime)
                {
                    Debug.Log("L'ennemi fait une ATTAQUE NORMALE !");
                    attackEndTime = Time.time + normalAttackDuration;
                    nextAttackTime = attackEndTime + attackCooldown;
                    _hasRolledForJump = false;
                    if (playerTime != null) playerTime.TakeDamage(10f);
                }
            }
        }

        // LISSAGE ET DÉPLACEMENT
        if (isAttacking)
        {
            _speed = 0f;
            _animationBlend = 0f;
        }
        else
        {
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            if (currentHorizontalSpeed < targetSpeed - 0.1f || currentHorizontalSpeed > targetSpeed + 0.1f)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
        }

        // ROTATION
        if (directionToPlayer != Vector3.zero && !isAttacking)
        {
            _targetRotation = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // APPLICATION DU DÉPLACEMENT
        if (targetSpeed > 0f && !isAttacking)
        {
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1f);
        }
    }

    private void ApplyGravity()
    {
        if (Grounded)
        {
            if (_hasAnimator) _animator.SetBool(_animIDFreeFall, false);
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else
        {
            if (_hasAnimator) _animator.SetBool(_animIDFreeFall, true);
        }
        if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Gravity * Time.deltaTime;
    }

    private void OnFootstep(AnimationEvent animationEvent) { }
    private void OnLand(AnimationEvent animationEvent) { }
}