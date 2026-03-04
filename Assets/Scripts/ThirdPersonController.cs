using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 4.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 1.0f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Header("Combat Settings")]
        public Transform attackPoint; // L'origine de l'attaque
        public float attackRange = 1.5f; // Rayon de la zone d'impact
        public LayerMask enemyLayers; // Filtre pour ne toucher que les ennemis
        
        public int attackDamage = 35;
        public int specialDamage = 75;
        
        public float timeRewardNormal = 6f; // Temps gagné (attaque normale)
        public float timeRewardSpecial = 12f; // Temps gagné (attaque spéciale)
        
        [Header("Coûts en Temps")]
        public float attackTimeCost = 0f;  // L'attaque normale coûte 1 sec
        public float specialTimeCost = 5f; // L'attaque spéciale coûte 3 sec
        
        private PlayerTimeManager _timeManager; // Le lien vers ton chrono

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private Inventory _inventory;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
            _inventory = GetComponent<Inventory>();
            _timeManager = GetComponent<PlayerTimeManager>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();
        }
        
        private bool lastDashState = false;
        private bool lastAtkState = false;
        private bool lastSpcState = false;

        public float dashSpeed = 5.0f;
        public float dashDuration = 0.5f;
        public float dashCooldown = 2f;
        private float dashTimer = 0f;
        public float dashBuffer = 0.3f;
        private Vector3 dashDirection;

        public float slowSpeed = 4f;
        public float attackDuration = 1f;
        public float attackCooldown = 0.5f;
        private float attackTimer = 0f;
        public float attackBuffer = 0.3f;

        public float superSlowSpeed = 6f;
        public float specialDuration = 2f;
        public float specialCooldown = 1.2f;
        public float specialTimer = 0f;
        public float specialBuffer = 0.1f;

        public bool isDashing = false;
        private bool canDash = true;
        public bool isAttacking = false;  
        private bool canAttack = true;
        public bool isSpecialing = false;
        private bool canSpecial = true;

        private void Update()
        {
            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_input.attack != lastAtkState && canAttack && !isAttacking && !isSpecialing && !isDashing)
            {
                Debug.Log("J'attaque");
    
                if (_hasAnimator) 
                {
                    _animator.Play(atk);
                }
    
                    Attack();
            }
            if (_input.special != lastSpcState && canSpecial && !isSpecialing && !isAttacking && !isDashing){
                Debug.Log("Je special");
                Special();
            }
            if (_input.jump != lastDashState && canDash && !isDashing && !isAttacking && !isSpecialing){
                Debug.Log("Je dash");
                StartDash();
            }
            if (isAttacking)
                attackTimer += Time.deltaTime;
            if (isSpecialing)
                specialTimer += Time.deltaTime;
            if (isDashing)
                dashTimer += Time.deltaTime;
                // maybe below
            if (attackTimer < attackDuration - attackBuffer|| specialTimer < specialDuration - specialBuffer || dashTimer < dashDuration - dashBuffer){
                _input.attack = false;
                _input.special = false;
                _input.jump = false;
            }
            if (dashTimer < dashDuration)
                _controller.Move(dashDirection * (dashSpeed * Time.deltaTime));
            if (attackTimer >= attackDuration - attackBuffer && isAttacking){
                isAttacking = false;
                SprintSpeed *= slowSpeed;
                _inventory.canSwitch = true;
                _inventory.switchCdTimer = 0f;
                _inventory.switchTimer = 0f;
                _inventory.lastSwitch_state = false;
            }
            if (specialTimer >= specialDuration - specialBuffer && isSpecialing){
                isSpecialing = false;
                SprintSpeed *= superSlowSpeed;
                _inventory.canSwitch = true;
                _inventory.switchCdTimer = 0f;
                _inventory.switchTimer = 0f;
                _inventory.lastSwitch_state = false;
            }
            if (dashTimer >= dashDuration - dashBuffer && isDashing){
                isDashing = false;
                _inventory.canSwitch = true;
                _inventory.switchCdTimer = 0f;
                _inventory.switchTimer = 0f;
                _inventory.lastSwitch_state = false;
            }
            if (!isAttacking && !canAttack && attackTimer + attackDuration < attackCooldown)
                attackTimer += Time.deltaTime;
            if (!isSpecialing && !canSpecial && specialTimer + specialDuration < specialCooldown)
                specialTimer += Time.deltaTime;
            if (!isDashing && !canDash && dashTimer + dashDuration < dashCooldown)
                dashTimer += Time.deltaTime;
            if (!isAttacking && !canAttack && attackTimer + attackDuration >= attackCooldown){
                canAttack = true;
                Debug.Log("merde");}
            if (!isSpecialing && !canSpecial && specialTimer + specialDuration >= specialCooldown)
                canSpecial = true;
            if (!isDashing && !canDash && dashTimer + dashDuration >= dashCooldown)
                canDash =true;
            if (!isDashing){
                Move();
            }
            lastAtkState = _input.attack;
            lastSpcState = _input.special;
            lastDashState = _input.jump;
        }
        private void Special(){
            if (_timeManager != null && !_timeManager.TryUseWeapon(specialTimeCost)) return;
            isSpecialing = true;
            canSpecial = false;
            specialTimer = 0f;
            _inventory.canSwitch = false;
            //animation
            SprintSpeed /= superSlowSpeed;
            PerformHitDetection(specialDamage, timeRewardSpecial);
        }

        private void Attack() {
            isAttacking = true;
            canAttack = false;
            attackTimer = 0f;
            _inventory.canSwitch = false;
            //animation
            SprintSpeed /= slowSpeed;
            PerformHitDetection(attackDamage, timeRewardNormal);
        }

        private void PerformHitDetection(int damage, float timeReward)
        {
            if (attackPoint == null) return; 

            // Crée une bulle invisible et chope tout ce qui est dans "enemyLayers"
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage, timeReward);
                }
            }
        }

        private void StartDash() {
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (inputDirection != Vector3.zero){
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            dashDirection = transform.rotation * Vector3.forward;
            isDashing = true;
            canDash = false;
            dashTimer = 0f;
            _inventory.canSwitch = false;
        }

        private void Move()
        {
            float targetSpeed = SprintSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_input.move.x * SprintSpeed , 0.0f, _input.move.y * SprintSpeed).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
                _speed = targetSpeed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnDrawGizmos()
        {
            if (attackPoint == null) return;
            Gizmos.color = new Color(1, 0, 0, 0.5f); // Rouge un peu transparent
            Gizmos.DrawSphere(attackPoint.position, attackRange);
        }
    }
}