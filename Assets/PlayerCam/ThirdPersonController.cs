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
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();
        }
        
        private bool lastDashState = false;
        public float dashSpeed = 5.0f;

        public float dashDuration = 0.5f;
        public float dashCooldown = 2f;
        private float dashTimer = 0f;
        private float cooldownTimer = 0f;
        public float dashBuffer = 0.3f;

        public bool isDashing = false;
        private bool canDash = true;
        private Vector3 dashDirection;

        private bool lastAtkState = false;

        public float attackDuration = 1f;
        public float attackCooldown = 0.5f;
        private float attackTimer = 0f;
        private float attackCdTimer = 0f;
        public float attackBuffer = 0.3f;

        public bool isAttacking = false;  
        private bool canAttack = true;
        public float slowSpeed = 4f;

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (!canAttack){
                attackCdTimer += Time.deltaTime;
                if (attackCdTimer >= attackCooldown) {
                    canAttack = true;
                    attackCdTimer = 0f;
                    lastAtkState = false;
                }
            }
            if (!canDash) {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer >= dashCooldown) {
                    canDash = true;
                    cooldownTimer = 0f;
                    lastDashState = false;
                }
            }
            if (attackCdTimer != 0f && attackCdTimer < attackCooldown - attackBuffer){
                _input.attack = false;
            }
            if (_input.attack!=lastAtkState && canAttack && !isAttacking)
                Attack();
            if (isAttacking) {
                attackTimer += Time.deltaTime;

                if (attackTimer > attackDuration) {
                    isAttacking = false;
                    attackTimer = 0f;
                    canAttack = false;
                    attackCdTimer = 0f;
                    _input.attack = false;
                    canDash = true;
                    cooldownTimer = 0f;
                    dashTimer = 0f;
                    lastDashState = false;
                    _inventory.canSwitch = true;
                    _inventory.switchCdTimer = 0f;
                    _inventory.switchTimer = 0f;
                    _inventory.lastSwitch_state = false;
                    SprintSpeed *= slowSpeed;
                } else
                    Debug.Log("I Attack the enemy!");
            }

            if (cooldownTimer != 0f && cooldownTimer < dashCooldown - dashBuffer)
                _input.jump = false;
            if (_input.jump!=lastDashState && canDash && !isDashing && !isAttacking){
                StartDash();}
            if (isDashing && !isAttacking) {
                dashTimer += Time.deltaTime;
        
                if (dashTimer > dashDuration) {
                    isDashing = false;
                    dashTimer = 0f;
                    canDash = false;
                    cooldownTimer = 0f;
                    _input.jump = false;
                    _inventory.canSwitch = true;
                    _inventory.switchCdTimer = 0f;
                    _inventory.switchTimer = 0f;
                    _inventory.lastSwitch_state = false;
                } else{
                    _controller.Move(dashDirection * (dashSpeed * Time.deltaTime));
                }
            }
            if (!isDashing)
                Move();
            lastAtkState = _input.attack;
            lastDashState = _input.jump;
        }

        private void Attack() {
            canDash = false;
            _inventory.canSwitch = false;
            isAttacking = true;
            canAttack = false;
            attackTimer = 0f;
            SprintSpeed /= slowSpeed;
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
            _inventory.canSwitch = false;
            dashTimer = 0f;
            canDash = false;
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

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}