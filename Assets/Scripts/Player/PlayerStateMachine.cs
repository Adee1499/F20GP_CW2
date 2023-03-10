using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerStateMachine : MonoBehaviour
{
    // Component references
    Animator _animator;
    CharacterController _characterController;
    Controls _controls;
    PlayerInput _playerInput;
    CinemachineVirtualCamera _virtualCamera;

    // Animator hashed variables
    int _animIsWalkingHash;
    int _animIsRunningHash;

    // Movement variables
    [Header("Controls & Movement")]
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 2f;
    [SerializeField] float _rotationSpeed = 30f;
    Vector2 _currentMovementInput;
    Vector2 _currentAimInput;
    Vector2 _lastAimInput;
    Transform _currentTarget;
    Vector3 _currentTargetPosition;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    // HP & MP variables
    [Header("Health & Mana")]
    [SerializeField] float _maxPlayerHP;
    [SerializeField] float _maxPlayerMP;
    float _playerHP;
    float _playerMP;
    [Tooltip("Mana Points regenerated per second")]
    [SerializeField] float _manaRegenRate;

    // State Machine variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters & Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }}
    public Animator Animator { get { return _animator; }}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; }}
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsRunPressed { get { return _isRunPressed; }}
    public float WalkSpeed { get { return _walkSpeed; }}
    public float RunSpeed { get { return _runSpeed; }}
    public int AnimIsWalkingHash { get { return _animIsWalkingHash; }}
    public int AnimIsRunningHash { get { return _animIsRunningHash; }}
    public float PlayerHealth { get { return _playerHP; }}
    public float PlayerMana { get { return _playerMP; }}

    // Events
    public static event Action<float> OnPlayerHealthChange;
    public static event Action<float> OnPlayerManaChange;
    public static event Action OnPlayerDead;

    
    void Awake()
    {
        // Initialize reference variables
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _controls = new Controls();
        _playerInput = GetComponent<PlayerInput>();
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_virtualCamera == null)
            Debug.LogError("Cinemachine virtual camera not found in the scene!");

        // Setup FSM
        _states = new PlayerStateFactory(this);
        _currentState = _states.Default();
        _currentState.EnterState();

        _playerHP = _maxPlayerHP;
        _playerMP = _maxPlayerMP;

        // Set animator hash variables
        _animIsWalkingHash = Animator.StringToHash("isWalking");
        _animIsRunningHash = Animator.StringToHash("isRunning");

        // Set PlayerInput callbacks
        _controls.Player.Move.started += OnMovementInput;
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Run.started += OnRunInput;
        _controls.Player.Run.canceled += OnRunInput;
    }
   
    void OnMovementInput (InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput != Vector2.zero;
    }

    void OnRunInput (InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void HandleRotation()
    {
        // Quaternion currentRotation = transform.rotation;
        // Vector3 positionToLookAt = new Vector3(_appliedMovement.x, 0f, _appliedMovement.z);
        // Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

        // // Different for KBM and gamepad control scheme
        // if (!_isGamepad) {
        //     Ray mouseTarget = Camera.main.ScreenPointToRay(_currentAimInput);

        //     if (Physics.Raycast(mouseTarget, out var hitInfo, Mathf.Infinity, groundMask)) {
        //         _currentTargetPosition = new Vector3(hitInfo.point.x, 0f, hitInfo.point.z);
        //         if (hitInfo.transform.tag.Equals("Enemy")) {
        //             // If mouse over enemy -> get position and highlight
        //             _currentTarget = hitInfo.transform;
        //             _currentTargetPosition = hitInfo.transform.position;
        //             hitInfo.transform.gameObject.GetComponent<RaycastHighlight>()?.ToggleHighlight(true);
        //         } else {
        //             _currentTarget = null;
        //         }
        //         // Rotate towards mouse/enemy only if not moving
        //         positionToLookAt = _currentTargetPosition;
        //     }
        //     if (!_isMovementPressed) {
        //         targetRotation = Quaternion.LookRotation(positionToLookAt - transform.position);
        //     }
        // } else {
        //     if (Mathf.Abs(_currentAimInput.x) > _controllerDeadzone || Mathf.Abs(_currentAimInput.y) > _controllerDeadzone) {
        //         positionToLookAt = Vector3.right * _currentAimInput.x + Vector3.forward * _currentAimInput.y;

        //         if (positionToLookAt.sqrMagnitude > 0f) {
        //             targetRotation = Quaternion.LookRotation(positionToLookAt, Vector3.up);
        //         }
        //     }
        // }
        // transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
        // _lastAimInput = _currentAimInput;
    }

    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();
        // Transform the movement vector relative to the camera
        TransformMovementVector();
        _characterController.Move(_appliedMovement * Time.deltaTime);
        RegenerateMana();
    }

    void TransformMovementVector()
    {
        if (_virtualCamera != null) {
            Vector3 cameraForward = _virtualCamera.transform.forward;
            Vector3 cameraRight = _virtualCamera.transform.right;

            // Ignore thr y components
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize the vectors to get the direction
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction based on the orientation
            _appliedMovement = cameraForward * _appliedMovement.z + cameraRight * _appliedMovement.x;
            // _appliedMovement.y -= 9.8f;
        }
    }

    void OnEnable() 
    {
        _controls.Player.Enable();
    }

    void OnDisable()
    {
        _controls.Player.Disable();
    }

    void TakeDamage(int damage)
    {
        _playerHP -= damage;
        OnPlayerHealthChange?.Invoke(_playerHP);
        if (_playerHP <= 0) {
            PlayerDead();
        }
    }

    void UseMana(int amount)
    {
        _playerMP -= amount;
        OnPlayerManaChange?.Invoke(_playerMP);
    }

    void RegenerateMana()
    {
        if (_playerMP < _maxPlayerMP) {
            _playerMP += _manaRegenRate * Time.deltaTime;
            OnPlayerManaChange?.Invoke(_playerMP);
        }
    }

    void PlayerDead()
    {
        OnPlayerDead?.Invoke();
        _controls.Disable();
        _characterController.enabled = false;
        // _animator.SetTrigger(_animDyingHash);
    }
}
