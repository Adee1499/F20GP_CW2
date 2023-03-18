using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerStateMachine : MonoBehaviour
{
    // Component references
    [SerializeField] Animator _animator;
    CharacterController _characterController;
    Controls _controls;
    PlayerInput _playerInput;
    CinemachineVirtualCamera _virtualCamera;

    // Animator hashed variables
    int _animSpeedHash;
    int _animAttackHash;
    int _animPickUpHash;

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
    bool _isAttackPressed;
    bool _isInteractPressed;

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
    public int AnimAttackHash { get { return _animAttackHash; }}
    public int AnimPickUpHash { get { return _animPickUpHash; }}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; }}
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsRunPressed { get { return _isRunPressed; }}
    public bool IsAttackPressed { get { return _isAttackPressed; }}
    public bool IsInteractPressed { get { return _isInteractPressed; }}
    public float WalkSpeed { get { return _walkSpeed; }}
    public float RunSpeed { get { return _runSpeed; }}
    public float PlayerHealth { get { return _playerHP; }}
    public float PlayerMana { get { return _playerMP; }}

    // Events
    public static event Action<float> OnPlayerHealthChange;
    public static event Action<float> OnPlayerManaChange;
    public static event Action OnPlayerDead;

    
    void Awake()
    {
        // Initialize reference variables
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
        _animSpeedHash = Animator.StringToHash("Speed");
        _animAttackHash = Animator.StringToHash("Attack");
        _animPickUpHash = Animator.StringToHash("PickUp");

        // Set PlayerInput callbacks
        _controls.Player.Move.started += OnMovementInput;
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Run.started += OnRunInput;
        _controls.Player.Run.canceled += OnRunInput;

        _controls.Player.Interact.started += OnInteractInput;
        _controls.Player.Interact.canceled += OnInteractInput;

        _controls.Player.Attack.started += OnAttackInput;
        _controls.Player.Attack.canceled += OnAttackInput;
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

    void OnInteractInput (InputAction.CallbackContext context)
    {
        _isInteractPressed = context.ReadValueAsButton();
    }

    void OnAttackInput (InputAction.CallbackContext context)
    {
        _isAttackPressed = context.ReadValueAsButton();
    }

    void HandleRotation()
    {
        Quaternion currentRotation = transform.rotation;

        if (new Vector2(_appliedMovement.x, _appliedMovement.z).magnitude > 0f) {
            Vector3 positionToLookAt = new Vector3(_appliedMovement.x, 0f, _appliedMovement.z);
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
        } else {
            transform.rotation = currentRotation;
        }
    }

    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();
        // Transform the movement vector relative to the camera
        TransformMovementVector();
        _characterController.Move(_appliedMovement * Time.deltaTime);
        // Ignore the y component for the animator speed value
        _animator.SetFloat(_animSpeedHash, new Vector2(_appliedMovement.x, _appliedMovement.z).magnitude);
        RegenerateMana();
    }

    void TransformMovementVector()
    {
        if (_virtualCamera != null) {
            Vector3 cameraForward = _virtualCamera.transform.forward;
            Vector3 cameraRight = _virtualCamera.transform.right;

            // Ignore the y components
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize the vectors to get the direction
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction based on the orientation
            _appliedMovement = cameraForward * _appliedMovement.z + cameraRight * _appliedMovement.x;
            if (_appliedMovement != Vector3.zero) {
                _animator.ResetTrigger(_animPickUpHash);
            }
            _appliedMovement.y -= 9.8f;
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
